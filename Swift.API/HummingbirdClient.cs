using RestSharp;
using Swift.API.Clients;
using Swift.API.Clients.Interfaces;
using Swift.API.Http;
using Swift.API.Http.Interfaces;

namespace Swift.API {
    public class HummingbirdClient : IHummingbirdClient {
        private const string HummingbirdApiUrl = "http://hummingbird.me/api/v1/";

        public HummingbirdClient() {
            var client = new RestClient(HummingbirdApiUrl) {
                UserAgent = "swift-windows"
            };

            Connection = new Connection(client);
            Users = new UsersClient(Connection);
        }

        #region IHummingbirdClient Members

        public IConnection Connection { get; private set; }
        public IUsersClient Users { get; private set; }

        #endregion
    }
}