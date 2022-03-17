using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Peperino_Api.Libs
{
    public static class FirebaseHelper
    {
        public static IServiceCollection AddFirebase(this IServiceCollection services)
        {
            services.AddSingleton(Init());
            return services;
        }

        public static FirebaseApp Init()
        {
            var firebaseConfig = Environment.GetEnvironmentVariable("FIREBASE_CONFIG");

            Console.WriteLine("Trying to get firebase config from env 'FIREBASE_CONFIG'");

            if (firebaseConfig is null)
            {
                Console.WriteLine("Trying to get firebase config from file 'firebase.json'");
                firebaseConfig = File.ReadAllText("firebase.json");
            }

            if (firebaseConfig is null)
            {
                throw new Exception("Firebase config not found.");
            }
            else
            {
                var firebase = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromJson(firebaseConfig),
                });

                Console.WriteLine("firebase initialized");
                return firebase;
            }
        }
    }
}
