namespace Peperino_Api.Models
{
    public class List
    {
        public string Name { get; set; } = "";
        public List<ListItem> ListItems { get; set; } = new List<ListItem>();
    }
}
