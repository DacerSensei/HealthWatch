using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Database;
using System.Threading.Tasks;

namespace HealthMonitoring.Config
{
    public static class Database
    {
        private static readonly string AuthenticationFirebase = "MlZDkbcfIINVzR9mHPwRuwLLzNNJBL0TuqHhUlXY";
        private static readonly string WebAPI = "AIzaSyCFA6hO9I08HWRgnkqfVrKb7pIBYJSVl4M";
        private static readonly string AuthDomain = "healthwatch-f6e17.firebaseapp.com";
        private static readonly string DatabaseURL = "https://healthwatch-f6e17-default-rtdb.asia-southeast1.firebasedatabase.app/";

        private static FirebaseAuthConfig FirebaseConfig = new FirebaseAuthConfig()
        {
            ApiKey = WebAPI,
            AuthDomain = AuthDomain,
            Providers = new FirebaseAuthProvider[]
            {
                new GoogleProvider().AddScopes("email"),
                new FacebookProvider().AddScopes("email"),
                new EmailProvider()
            }
        };
        public static FirebaseClient FirebaseClient = new FirebaseClient(DatabaseURL, new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(AuthenticationFirebase) });
        public static FirebaseAuthClient FirebaseAuthClient = new FirebaseAuthClient(FirebaseConfig);

    }
}
