using System.Collections.Generic;
using Peperino_Api.Models;

namespace Peperino_Api.Models.List
{
    public partial class ListDto
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public List<ListItemDto> ListItems { get; set; }
    }
}