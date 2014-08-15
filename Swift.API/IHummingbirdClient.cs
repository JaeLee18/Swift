using Swift.API.Clients;
using Swift.API.Http;

namespace Swift.API {
    public interface IHummingbirdClient {
        IConnection Connection { get; }
        IUsersClient Users { get; }
    }
}