using System;
using System.Collections.Generic;
using RestSharp;
using Swift.API.Extensions;
using Swift.API.Helpers;

namespace Swift.API.Http {
    public class Connection {
        public IRestClient Client { get; private set; }

        public Connection(IRestClient client) {
            Client = client;
        }

        public IObservable<IRestResponse> ExecuteRaw(string endpoint, IList<Parameter> parameters,
            Method method = Method.GET) {
            var request = BuildRequest(endpoint, parameters);
            request.Method = method;
            return Client.ExecuteRxRaw(request);
        }

        public IObservable<T> ExecuteRequest<T>(string endpoint, IList<Parameter> parameters,
            object data = null, string expectedRoot = null, Method method = Method.GET)
            where T : new() {
            var request = BuildRequest(endpoint, parameters);
            request.RootElement = expectedRoot;
            request.Method = method;

            if (data != null && method != Method.GET) {
                request.RequestFormat = DataFormat.Json;
                request.JsonSerializer = new JsonNetSerializer();
                request.AddBody(data);
            }

            return Client.ExecuteRx<T>(request);
        }

        private static IRestRequest BuildRequest(string endpoint, IEnumerable<Parameter> parameters) {
            var request = new RestRequest(endpoint);

            if (parameters == null) {
                return request;
            }
            foreach (var parameter in parameters) {
                request.AddParameter(parameter);
            }

            return request;
        }
    }
}