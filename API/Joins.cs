using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Need4Protocol
{
    public partial class ActionDetails
    {
        partial void OnConstruction()
        {
            Permissions = new HashSet<Permission>();
        }
        public ICollection<Permission> Permissions { get; set; } 
    }

    public partial class ItemList
    {
        partial void OnConstruction()
        {
            ItemList_Item = new HashSet<ItemList_Item>();
        }

        public ICollection<ItemList_Item> ItemList_Item { get; set; }
    }

    public partial class ItemList_Item
    {
        public int ItemListId { get; set; }
        public int ItemId { get; set; }

        public virtual Item Item { get; set; }
        public virtual ItemList ItemList { get; set; }
    }


    public partial class Item
    {
        partial void OnConstruction()
        {
            ItemList_Item = new HashSet<ItemList_Item>();
            TradeItemDetails = new HashSet<TradeItemDetails>();
        }

        public  ICollection<ItemList_Item> ItemList_Item { get; set; }
        public  ICollection<TradeItemDetails> TradeItemDetails { get; set; }
    }

    public partial class Permission
    {
        public  int ActionDetailsId { get; set; }
        public  int PermissionTypeId { get; set; }
    }

    public partial class PermissionType
    {
        partial void OnConstruction()
        {
            Permissions = new HashSet<Permission>();
        }

        public ICollection<Permission> Permissions { get; set; }
    }

    public partial class TradeItemDetails
    {
        partial void OnConstruction()
        {
        }

        public int ItemId { get; set; }
        public TradeItemList TradeItemList { get; set; }
        public int TradeItemListId { get; set; }
    }

    public partial class TradeItemList
    {
        partial void OnConstruction()
        {
            Trades = new HashSet<Trade>();
        }

        public ICollection<Trade> Trades { get; set; }
    }

    public partial class Trade
    { 
        public  int TradeItemListId { get; set; }
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