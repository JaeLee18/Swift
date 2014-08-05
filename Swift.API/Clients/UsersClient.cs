using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using RestSharp;
using Swift.API.Extensions;
using Swift.API.Http;
using Swift.API.Models.Responses;

namespace Swift.API.Clients {
    public class UsersClient {
        private readonly Connection _connection;

        public UsersClient(Connection connection) {
            _connection = connection;
        }

        public IObservable<User> GetInfo(string username) {
            var parameters = new List<Parameter> {
                new Parameter { Name = "username", Value = username, Type = ParameterType.UrlSegment }
            };
            var ret = _connection.ExecuteRequest("users/{username}", parameters)
                .SelectMany(x => x.ExpectStatus(HttpStatusCode.OK));
            return ret.Deserialize<User>();
        }

        public IObservable<string> Authenticate(string username, string password) {
            var parameters = new List<Parameter> {
                new Parameter { Name = "username", Value = username, Type = ParameterType.QueryString },
                new Parameter { Name = "password", Value = password, Type = ParameterType.QueryString }
            };
            return _connection.ExecuteRaw("users/authenticate", parameters, Method.POST)
                .SelectMany(x => x.ExpectStatus(HttpStatusCode.Created))
                .SelectMany(x => Observable.Return(x.Content));
        }
    }
}