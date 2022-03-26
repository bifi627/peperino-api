using Mapster;
using System.Text.Json.Serialization;

namespace Peperino_Api.Models.List
{
    [GenerateMapper]
    [AdaptTwoWays("[name]Dto")]
    public class ListItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; } = "";
        public bool Checked { get; set; } = false;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemType Type { get; set; } = ItemType.Text;
    }

    public enum ItemType
    {
        Text,
        Link,
        Picture,
        Document,
    }
}
