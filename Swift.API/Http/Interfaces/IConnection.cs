using System;
using System.Collections.Generic;
using RestSharp;

namespace Swift.API.Http.Interfaces {
    public interface IConnection {
        IObservable<IRestResponse> ExecuteRaw(string endpoint, IList<Parameter> parameters,
            Method method = Method.GET);

        IObservable<IRestResponse> ExecuteRequest(string endpoint, IList<Parameter> parameters,
            object data = null, string expectedRoot = null, Method method = Method.GET);


    }
}