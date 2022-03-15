using Mapster;

namespace Peperino_Api.Models.List
{
    [GenerateMapper]
    [AdaptTwoWays("[name]Dto")]
    public class ListItem
    {
        public string Text { get; set; } = "";
    }
}
