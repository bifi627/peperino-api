namespace Peperino_Api.Models
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; } = "";
        public string DatabaseName { get; set; } = "";
        public string UsersCollectionName { get; set; } = "";
        public string ItemsCollectionName { get; set; } = "";
    }
}
