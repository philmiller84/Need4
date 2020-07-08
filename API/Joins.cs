using System.Collections.Generic;

namespace Need4Protocol
{
    //JOIN TABLE FOR ITEMLIST TO WORK
    public partial class ItemList { public List<ItemList_Item> Joins { get; set; } }
    public partial class Item { public List<ItemList_Item> Joins { get; set; } }
    public class ItemList_Item
    {
        public int ItemListId { get; set; }
        public ItemList ItemList { get; set; }

        public int ItemId { get; set; }
        public Item Item { get; set; }
    }

    public partial class TradeItemList { public List<TradeItemList_TradeItemDetails> Joins { get; set; } }
    public partial class TradeItemDetails { public List<TradeItemList_TradeItemDetails> Joins { get; set; } }
    public class TradeItemList_TradeItemDetails
    {
        public int TradeItemListId { get; set; }
        public TradeItemList TradeItemList { get; set; }
        public int TradeItemDetailsId { get; set; }
        public TradeItemDetails TradeItemDetails { get; set; }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////
    //  DON'T DO THIS!!!! DON'T BE LAZY!!!! NO NEED FOR PERSISTENCE, JUST MAKE THE HTML MANUALLY!!!
    //////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////
    ////JOIN TABLE FOR TRADEACTIONLIST TO WORK
    //////////////////////////////////////////////////////////////////////////////////////////////////
    //public partial class TradeAction { public List<TradeActionList_TradeAction> Joins { get; set; } }
    //public partial class TradeActionList { public List<TradeActionList_TradeAction> Joins { get; set; } }
    //public class TradeActionList_TradeAction
    //{
    //    public int TAL_Id { get; set; }
    //    public TradeActionList TradeActionList { get; set; }
    //    public int TA_Id { get; set; }
    //    public TradeAction TradeAction { get; set; }
    //}
    //////////////////////////////////////////////////////////////////////////////////////////////////

}