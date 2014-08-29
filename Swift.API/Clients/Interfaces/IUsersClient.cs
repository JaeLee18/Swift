using System;
using Swift.API.Models.Responses;

namespace Swift.API.Clients.Interfaces {
    public interface IUsersClient {
        IObservable<User> GetInfo(string username);
        IObservable<string> Authenticate(string username, string password);
    }
}