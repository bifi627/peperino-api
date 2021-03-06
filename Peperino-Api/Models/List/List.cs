using Mapster;

namespace Peperino_Api.Models.List
{
    [GenerateMapper]
    [AdaptTwoWays("[name]Dto")]
    public class List
    {
        public string Name { get; set; } = "";
        public string Slug { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Created { get; set; } = default;
        public string OwnerName { get; set; } = "";
        public List<ListItem> ListItems { get; set; } = new List<ListItem>();
    }
}
