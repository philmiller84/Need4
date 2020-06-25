using System.Collections.Generic;

namespace Need4Protocol
{
    public partial class Item
    {
        public List<ItemListItem> joins { get; set; }
    }

    public partial class ItemList
    {
        public List<ItemListItem> joins { get; set; }
    }

    public class ItemListItem
    {
        public int Id { get; set; }
        public ItemList ItemList { get; set; }

        public string Name { get; set; }
        public Item Item { get; set; }
    }

}