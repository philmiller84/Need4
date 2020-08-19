using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Need4Protocol
{
    //public partial class ActionDetails
    //{
    //    partial void OnConstruction()
    //    {
    //        Permissions = new HashSet<Permission>();
    //    }
    //    public ICollection<Permission> Permissions { get; set; } 
    //}

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
        public virtual Item Item { get; set; }
        public int ItemId { get; set; }

        public virtual ItemList ItemList { get; set; }
        public int ItemListId { get; set; }
    }


    public partial class Item
    {
        partial void OnConstruction()
        {
            ItemList_Item = new HashSet<ItemList_Item>();
            //TradeItemDetails = new HashSet<TradeItemDetails>();
        }
        public  ICollection<ItemList_Item> ItemList_Item { get; set; }
        //public  ICollection<TradeItemDetails> TradeItemDetails { get; set; }
    }

    public partial class Permission
    {
        public  int PermissionTypeId { get; set; }
        public  int RelationshipTypeId { get; set; }
        //public RelationshipType RelationshipType { get; set; }
    }

    public partial class Requirement
    {
        public  int PermissionTypeId { get; set; }
        public int RelationshipTypeId { get; set; }
        //public RelationshipType RelationshipType { get; set; }
    }

    public partial class RelationshipType
    {
        //public int Id;
        //public string Name;
    }
    public partial class PermissionType
    {
        partial void OnConstruction()
        {
            Permissions = new HashSet<Permission>();
            Requirements = new HashSet<Requirement>();
        }

        public ICollection<Permission> Permissions { get; set; }
        public ICollection<Requirement> Requirements { get; set; }
    }

    public partial class TradeItemDetails
    {
        public int ItemId { get; set; }
        public TradeItemList TradeItemList { get; set; }
        public int TradeItemListId { get; set; }
    }

    public class TradeUser
    {
        public int Id { get; set; }
        public int TradeId { get; set; }
        public int UserId { get; set; }
        public Trade Trade { get; set; }
        public User User { get; set; }
    }

    //public partial class TradeUserList
    //{
    //    partial void OnConstruction()
    //    {
    //        Trades = new HashSet<Trade>();
    //    }
    //    public ICollection<Trade> Trades { get; set; }
    //}

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
        partial void OnConstruction()
        {
            TradeUser = new HashSet<TradeUser>();
        }
        public ICollection<TradeUser> TradeUser { get; set; }
        public int TradeItemListId { get; set; }
    }

    public partial class SaleItemDetails
    {
        public int ItemId { get; set; }
        public SaleItemList SaleItemList { get; set; }
        public int SaleItemListId { get; set; }

    }

    public partial class Sale
    {
        //public int SaleItemListId { get; set; }
    }
    public partial class SaleItemList
    {
        public Sale Sale { get; set; }
        public int SaleId { get; set; }
    }


    public partial class User
    {
        partial void OnConstruction()
        {
            TradeUser = new HashSet<TradeUser>();
        }

        public ICollection<TradeUser> TradeUser { get; set; }
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