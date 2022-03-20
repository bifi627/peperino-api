namespace Peperino_Api.Startup
{
    public static class MongoDb
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, ConfigurationManager configuration)
        {
            // Add mongodb config
            var mongoSettingsJson = Environment.GetEnvironmentVariable("MONGO_SETTINGS_JSON");

            Console.WriteLine("[MongoDb] Trying to get mongodb config from env 'MONGO_SETTINGS_JSON'");

            if (mongoSettingsJson is not null)
            {
                var mongoSettings = Newtonsoft.Json.JsonConvert.DeserializeObject<MongoSettings>(mongoSettingsJson);

                Console.WriteLine($"[MongoDb] ${mongoSettings.ConnectionString}");

                services.Configure<MongoSettings>(settings =>
                {
                    settings = mongoSettings;
                });
            }
            else
            {
                Console.WriteLine("[MongoDb] Trying to get mongodb config from appsettings 'MongoSettings'");
                IConfigurationSection config = configuration.GetSection("MongoSettings");

                var connectionString = config.GetSection("ConnectionString");
                Console.WriteLine($"[MongoDb] initialized: {connectionString.Value}");

                services.Configure<MongoSettings>(config);
            }

            return services;
        }
    }

    public class MongoSettings
    {
        public string ConnectionString { get; set; } = "";
        public string DatabaseName { get; set; } = "";
        public string UsersCollectionName { get; set; } = "";
        public string ItemsCollectionName { get; set; } = "";
    }
}
