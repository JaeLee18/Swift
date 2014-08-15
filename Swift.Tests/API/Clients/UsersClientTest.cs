using System.Collections.Generic;
using NSubstitute;
using RestSharp;
using Swift.API.Clients;
using Swift.API.Http;
using Xunit;

namespace Swift.Tests.API.Clients {
    public class UsersClientTest {
        [Fact]
        public void CorrectRequestForAuthenticate() {
            var connection = Substitute.For<IConnection>();
            Substitute.For<UsersClient>(connection).Authenticate("foo", "bar");

            var @params =
                Arg.Is<List<Parameter>>(list => (string)list[0].Value == "foo" && (string)list[1].Value == "bar");
            connection.Received().ExecuteRaw("users/authenticate", @params, Method.POST);
        }

        [Fact]
        public void CorrectRequestForGetInfo() {
            var connection = Substitute.For<IConnection>();
            Substitute.For<UsersClient>(connection).GetInfo("foobar");

            var @params =
                Arg.Is<List<Parameter>>(list => (string)list[0].Value == "foobar");
            connection.Received().ExecuteRequest("users/{username}", @params);
        }
    }
}