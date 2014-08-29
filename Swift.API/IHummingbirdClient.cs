using Swift.API.Clients.Interfaces;
using Swift.API.Http.Interfaces;

namespace Swift.API {
    public interface IHummingbirdClient {
        IConnection Connection { get; }
        IUsersClient Users { get; }
    }
}