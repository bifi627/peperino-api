namespace Peperino_Api.Models.Request
{
    public class CheckItemRequest
    {
        public Guid Id { get; set; }
        public bool Checked { get; set; } = false;
    }
}
