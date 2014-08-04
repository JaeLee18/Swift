using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using Akavache;
using ReactiveUI;
using Swift.Extensions;

namespace Swift.Models {
    [DataContract]
    public class Account : ReactiveObject {
        private const string CacheKey = "__account__";
        private string _token;
        private string _username;

        [DataMember]
        public string Username {
            get { return _username; }
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        [DataMember]
        public string Token {
            get { return _token; }
            set { this.RaiseAndSetIfChanged(ref _token, value); }
        }

        public bool HasData() {
            return !(Username.Empty() && Token.Empty());
        }

        public IObservable<Unit> Clear() {
            return BlobCache.Secure.Invalidate(CacheKey);
        }

        public IObservable<Unit> Save() {
            return BlobCache.Secure.InsertObject(CacheKey, this).Retry(3);
        }

        public IObservable<Account> UpdateFromCache() {
            return Observable.StartAsync(async () => {
                var cachedAccount =
                    await BlobCache.Secure.GetObject<Account>(CacheKey).Catch(Observable.Return(this));
                Username = cachedAccount.Username;
                Token = cachedAccount.Token;
                return this;
            });
        }
    }
}