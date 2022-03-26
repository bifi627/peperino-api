using System.Collections.Generic;
using Peperino_Api.Models.List;

namespace Peperino_Api.Models.List
{
    public static partial class ListMapper
    {
        public static List AdaptToList(this ListDto p1)
        {
            return p1 == null ? null : new List()
            {
                Name = p1.Name,
                Slug = p1.Slug,
                ListItems = funcMain1(p1.ListItems)
            };
        }
        public static List AdaptTo(this ListDto p3, List p4)
        {
            if (p3 == null)
            {
                return null;
            }
            List result = p4 ?? new List();
            
            result.Name = p3.Name;
            result.Slug = p3.Slug;
            result.ListItems = funcMain2(p3.ListItems, result.ListItems);
            return result;
            
        }
        public static ListDto AdaptToDto(this List p7)
        {
            return p7 == null ? null : new ListDto()
            {
                Name = p7.Name,
                Slug = p7.Slug,
                ListItems = funcMain3(p7.ListItems)
            };
        }
        public static ListDto AdaptTo(this List p9, ListDto p10)
        {
            if (p9 == null)
            {
                return null;
            }
            ListDto result = p10 ?? new ListDto();
            
            result.Name = p9.Name;
            result.Slug = p9.Slug;
            result.ListItems = funcMain4(p9.ListItems, result.ListItems);
            return result;
            
        }
        
        private static List<ListItem> funcMain1(List<ListItemDto> p2)
        {
            if (p2 == null)
            {
                return null;
            }
            List<ListItem> result = new List<ListItem>(p2.Count);
            
            int i = 0;
            int len = p2.Count;
            
            while (i < len)
            {
                ListItemDto item = p2[i];
                result.Add(item == null ? null : new ListItem()
                {
                    Id = item.Id,
                    Text = item.Text,
                    Checked = item.Checked,
                    Type = item.Type
                });
                i++;
            }
            return result;
            
        }
        
        private static List<ListItem> funcMain2(List<ListItemDto> p5, List<ListItem> p6)
        {
            if (p5 == null)
            {
                return null;
            }
            List<ListItem> result = new List<ListItem>(p5.Count);
            
            int i = 0;
            int len = p5.Count;
            
            while (i < len)
            {
                ListItemDto item = p5[i];
                result.Add(item == null ? null : new ListItem()
                {
                    Id = item.Id,
                    Text = item.Text,
                    Checked = item.Checked,
                    Type = item.Type
                });
                i++;
            }
            return result;
            
        }
        
        private static List<ListItemDto> funcMain3(List<ListItem> p8)
        {
            if (p8 == null)
            {
                return null;
            }
            List<ListItemDto> result = new List<ListItemDto>(p8.Count);
            
            int i = 0;
            int len = p8.Count;
            
            while (i < len)
            {
                ListItem item = p8[i];
                result.Add(item == null ? null : new ListItemDto()
                {
                    Id = item.Id,
                    Text = item.Text,
                    Checked = item.Checked,
                    Type = item.Type
                });
                i++;
            }
            return result;
            
        }
        
        private static List<ListItemDto> funcMain4(List<ListItem> p11, List<ListItemDto> p12)
        {
            if (p11 == null)
            {
                return null;
            }
            List<ListItemDto> result = new List<ListItemDto>(p11.Count);
            
            int i = 0;
            int len = p11.Count;
            
            while (i < len)
            {
                ListItem item = p11[i];
                result.Add(item == null ? null : new ListItemDto()
                {
                    Id = item.Id,
                    Text = item.Text,
                    Checked = item.Checked,
                    Type = item.Type
                });
                i++;
            }
            return result;
            
        }
    }
}