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
        public ItemType Type { get; set; } = ItemType.Text;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ItemType
    {
        Text,
        Link,
        Picture,
        Document,
    }
}
