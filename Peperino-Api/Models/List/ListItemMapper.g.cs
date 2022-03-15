using Peperino_Api.Models.List;

namespace Peperino_Api.Models.List
{
    public static partial class ListItemMapper
    {
        public static ListItem AdaptToListItem(this ListItemDto p1)
        {
            return p1 == null ? null : new ListItem() {Text = p1.Text};
        }
        public static ListItem AdaptTo(this ListItemDto p2, ListItem p3)
        {
            if (p2 == null)
            {
                return null;
            }
            ListItem result = p3 ?? new ListItem();
            
            result.Text = p2.Text;
            return result;
            
        }
        public static ListItemDto AdaptToDto(this ListItem p4)
        {
            return p4 == null ? null : new ListItemDto() {Text = p4.Text};
        }
        public static ListItemDto AdaptTo(this ListItem p5, ListItemDto p6)
        {
            if (p5 == null)
            {
                return null;
            }
            ListItemDto result = p6 ?? new ListItemDto();
            
            result.Text = p5.Text;
            return result;
            
        }
    }
}