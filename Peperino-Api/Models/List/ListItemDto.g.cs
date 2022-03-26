using System;
using Peperino_Api.Models.List;

namespace Peperino_Api.Models.List
{
    public partial class ListItemDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
        public ItemType Type { get; set; }
    }
}