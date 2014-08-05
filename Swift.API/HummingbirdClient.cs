using RestSharp;
using Swift.API.Clients;
using Swift.API.Http;

namespace Swift.API {
    public class HummingbirdClient {
        private const string HummingbirdApiUrl = "http://hummingbird.me/api/v1/";

        public UsersClient Users { get; private set; }

        public HummingbirdClient() {
            var client = new RestClient(HummingbirdApiUrl) {
                UserAgent = "swift-windows"
            };

            var connection = new Connection(client);
            Users = new UsersClient(connection);
        }
    }
}