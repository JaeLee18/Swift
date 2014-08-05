using System;
using System.Net;
using System.Reactive.Linq;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Extensions;
using Swift.API.Exceptions;

namespace Swift.API.Extensions {
    public static class RestSharpExtensions {
        public static IObservable<IRestResponse> ExecuteRxRaw(this IRestClient client, IRestRequest request) {
            var ret = Observable.StartAsync(async () => await client.ExecuteTaskAsync(request).ConfigureAwait(false));
            return ret.ThrowIfException();
        }

        private static IObservable<IRestResponse> ThrowIfException(this IObservable<IRestResponse> response) {
            return response.SelectMany(resp => {
                if (resp.ErrorException != null) {
                    return Observable.Throw<IRestResponse>(resp.ErrorException);
                }

                if (resp.ResponseStatus != ResponseStatus.Completed) {
                    return Observable.Throw<IRestResponse>(resp.ResponseStatus.ToWebException());
                }

                return (int)resp.StatusCode >= 400
                    ? Observable.Throw<IRestResponse>(new ApiException(resp.StatusCode)) : Observable.Return(resp);
            });
        }

        public static IObservable<T> Deserialize<T>(this IObservable<IRestResponse> response) {
            return response.SelectMany(resp => {
                resp.Request.OnBeforeDeserialization(resp);
                var deserialize = new JsonDeserializer {
                    RootElement = resp.Request.RootElement,
                    DateFormat = resp.Request.DateFormat
                };
                return Observable.Return(deserialize.Deserialize<T>(resp));
            });
        }

        public static IObservable<IRestResponse> ExpectStatus(this IRestResponse response, HttpStatusCode code) {
            return response.StatusCode != code
                ? Observable.Throw<IRestResponse>(new ApiException(response.StatusCode))
                : Observable.Return(response);
        }
    }
}