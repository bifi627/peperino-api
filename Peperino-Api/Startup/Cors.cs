using System.Linq;
namespace Peperino_Api.Startup
{
    public static class Cors
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options => options.AddPolicy("DEBUG", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            services.AddCors(options => options.AddPolicy("PROD", policy =>
            {
                var allowedOrigins = new List<string>
                {
                    "https://peperino.vercel.app",
                    "https://peperino-bifi627.vercel.app"
                };

                Console.WriteLine($"[CORS] Allowed origins direct: {string.Join(", ", allowedOrigins)}");

                var allowed = Environment.GetEnvironmentVariable("ORIGINS_JSON");
                if (allowed is not null)
                {
                    var envCors = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(allowed);

                    Console.WriteLine($"[CORS] Allowed origins env: {string.Join(", ", envCors)}");

                    allowedOrigins.AddRange(envCors);
                }

                policy.WithOrigins(allowedOrigins.ToArray());
                //policy.WithHeaders("Authorization");
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            }));

            return services;
        }
    }
}
