using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace Peperino_Api.Startup
{
    public static class Firebase
    {
        public static IServiceCollection AddFirebase(this IServiceCollection services)
        {
            services.AddSingleton(Init());
            return services;
        }

        public static FirebaseApp Init()
        {
            var firebaseConfig = Environment.GetEnvironmentVariable("FIREBASE_CONFIG");

            Console.WriteLine("[Firebase] Trying to get firebase config from env 'FIREBASE_CONFIG'");

            if (firebaseConfig is null)
            {
                Console.WriteLine("[Firebase] Trying to get firebase config from file 'firebase.json'");
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


                if(firebase.Options.Credential.UnderlyingCredential is ServiceAccountCredential projectCredential)
                {
                    Console.WriteLine($"[Firebase] Firebase initialized: {projectCredential.ProjectId}");
                }

                return firebase;
            }
        }
    }
}
