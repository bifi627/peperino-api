using Peperino_Api.Models.List;

namespace Peperino_Api.Models.List
{
    public static partial class ListItemMapper
    {
        public static ListItem AdaptToListItem(this ListItemDto p1)
        {
            return p1 == null ? null : new ListItem()
            {
                Id = p1.Id,
                Text = p1.Text,
                Checked = p1.Checked,
                Type = p1.Type
            };
        }
        public static ListItem AdaptTo(this ListItemDto p2, ListItem p3)
        {
            if (p2 == null)
            {
                return null;
            }
            ListItem result = p3 ?? new ListItem();
            
            result.Id = p2.Id;
            result.Text = p2.Text;
            result.Checked = p2.Checked;
            result.Type = p2.Type;
            return result;
            
        }
        public static ListItemDto AdaptToDto(this ListItem p4)
        {
            return p4 == null ? null : new ListItemDto()
            {
                Id = p4.Id,
                Text = p4.Text,
                Checked = p4.Checked,
                Type = p4.Type
            };
        }
        public static ListItemDto AdaptTo(this ListItem p5, ListItemDto p6)
        {
            if (p5 == null)
            {
                return null;
            }
            ListItemDto result = p6 ?? new ListItemDto();
            
            result.Id = p5.Id;
            result.Text = p5.Text;
            result.Checked = p5.Checked;
            result.Type = p5.Type;
            return result;
            
        }
    }
}