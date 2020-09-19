﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Need4Protocol;
using System;
using System.Security.Cryptography.X509Certificates;
using StaticData;

namespace Models
{
    public class Need4Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string ConnectionString = "Data Source=C:\\Users\\Phil\\Repo\\Need4\\Models\\need4.db";

                try
                {
                    optionsBuilder.UseSqlite(ConnectionString);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // PERSISTED ENTITIES
            OnCreateStates(modelBuilder.Entity<State>());
            OnCreatePermissionTypes(modelBuilder.Entity<PermissionType>());
            OnCreateActionDetails(modelBuilder.Entity<ActionDetails>());
            OnCreateActivityDetails(modelBuilder.Entity<ActivityDetails>());
            OnCreateCommunities(modelBuilder.Entity<CommunityDetails>());
            OnCreateUsers(modelBuilder.Entity<User>());
            //OnCreateRelationshipStates(modelBuilder.Entity<RelationshipState>());
            OnCreateRelationshipTypes(modelBuilder.Entity<RelationshipType>());
            OnCreatePermissions(modelBuilder.Entity<Permission>());
            OnCreateRequirements(modelBuilder.Entity<Requirement>());
            OnCreateItems(modelBuilder.Entity<Item>());
            OnCreateItemList(modelBuilder.Entity<ItemList>());
            OnCreateItemList_Item(modelBuilder.Entity<ItemList_Item>());
            OnCreateTrade(modelBuilder.Entity<Trade>());
            OnCreateTradeUser(modelBuilder.Entity<TradeUser>());
            OnCreateTradeItemLists(modelBuilder.Entity<TradeItemList>());
            OnCreateTradeItemDetails(modelBuilder.Entity<TradeItemDetails>());
            OnCreateSaleItemDetails(modelBuilder.Entity<SaleItemDetails>());
            OnCreateSaleItemList(modelBuilder.Entity<SaleItemList>());
            OnCreateSale(modelBuilder.Entity<Sale>());
        }


        public DbSet<CommunityDetails> CommunityDetails { get; set; }
        private void OnCreateCommunities(EntityTypeBuilder<CommunityDetails> e)
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
            e.HasData(
                new { Id = 1, Name = "Newmarket", Tier = 1 },
                new { Id = 2, Name = "Millside", Tier = 2 },
                new { Id = 3, Name = "Bramble", Tier = 3 },
                new { Id = 4, Name = "The Harbour", Tier = 4 },
                new { Id = 5, Name = "Weaversfield", Tier = 5 }
                );
        }

        public DbSet<ActivityDetails> ActivityDetails { get; set; }
        private void OnCreateActivityDetails(EntityTypeBuilder<ActivityDetails> e)
        {
            e.HasKey(x => x.Id);
            //e.Property(x => x.Id).ValueGeneratedOnAdd();

            e.HasData(

                new { Id = 10, Name = "Create New Listing", Category = Constants.SALES_CATEGORY, Method = "/sales/new" },
                new { Id = 11, Name = "My Sales", Category = Constants.SALES_CATEGORY, Method = "/sales" },
                new { Id = 12, Name = "Public Sales", Category = Constants.SALES_CATEGORY, Method = "/sales/public" },

                new { Id = 20, Name = "Create New Trade", Category = Constants.TRADES_CATEGORY, Method = "/trades/new" },
                new { Id = 21, Name = "My Trades", Category = Constants.TRADES_CATEGORY, Method = "/trades" },
                new { Id = 22, Name = "Public Trades", Category = Constants.TRADES_CATEGORY, Method = "/trades/public" },
                new { Id = 23, Name = "Open Trades", Category = Constants.TRADES_CATEGORY, Method = "/trades/open" },
                new { Id = 24, Name = "Recent Trades", Category = Constants.TRADES_CATEGORY, Method = "/trades/recent" },

                new { Id = 30, Name = "Open Chat", Category = Constants.CHAT_CATEGORY, Method = "/chat" },
                new { Id = 31, Name = "History", Category = Constants.CHAT_CATEGORY, Method = "/chat/history" },

                new { Id = 40, Name = "Invite New Member", Category = Constants.MEMBERS_CATEGORY, Method = "/members/invite" },
                new { Id = 41, Name = "Show Member List", Category = Constants.MEMBERS_CATEGORY, Method = "/members" },
                new { Id = 42, Name = "Vote on Decisions", Category = Constants.MEMBERS_CATEGORY, Method = "/members/vote" },
                new { Id = 43, Name = "Report Scammer/Spammer", Category = Constants.MEMBERS_CATEGORY, Method = "/members/report" },

                new { Id = 51, Name = "About Communities", Category = Constants.COMMUNITIES_CATEGORY, Method = "/communities/about" },
                new { Id = 52, Name = "Show Communities", Category = Constants.COMMUNITIES_CATEGORY, Method = "/communities" },
                new { Id = 53, Name = "Join a Community", Category = Constants.COMMUNITIES_CATEGORY, Method = "/communities/join" }

                );
        }

        public DbSet<SaleItemDetails> SaleItemDetails { get; set; }
        private void OnCreateSaleItemDetails(EntityTypeBuilder<SaleItemDetails> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.HasOne(d => d.Item);

            e.HasOne(d => d.SaleItemList)
                .WithMany(p => p.SaleItemDetails)
                .HasForeignKey(d => d.SaleItemListId);

            e.HasData(
                new { Id = -1, ItemId = -1, AvailableQuantity = 2, Price = 2.5, SaleItemListId = -1 }
                );
        }

        public DbSet<SaleItemList> SaleItemLists { get; set; }
        private void OnCreateSaleItemList(EntityTypeBuilder<SaleItemList> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.Ignore(r => r.SaleItemDetails);
            e.HasOne(d => d.Sale)
                .WithOne(p => p.SaleItemList)
                .HasForeignKey("Sale", "Id");
            e.HasData(
                new {Id = -1, SaleId = -1}
                );
        }

        public DbSet<Sale> Sales { get; set; }
        private void OnCreateSale(EntityTypeBuilder<Sale> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
    
            e.HasData(
                new { Id = -1, TimeStarted = "20200707" }
                );
        }


        public DbSet<ActionDetails> ActionDetails { get; set; }
        private void OnCreateActionDetails(EntityTypeBuilder<ActionDetails> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(r => r.Id);
            entityTypeBuilder.HasData(
                new { Id = 1, Name = "GetTradeData", Category = Constants.VIEW_CATEGORY, Method = "TradeViewDetails/{0}" },
                new { Id = 2, Name = "ExcludeUser", Category = Constants.TRADE_ACTION_CATEGORY, Method = "/trade/exclude/{0}/{1}" },
                new { Id = 3, Name = "SplitTrade", Category = Constants.TRADE_ACTION_CATEGORY, Method = "/trade/split/{0}" },
                new { Id = 4, Name = "FinalizeTrade", Category = Constants.TRADE_ACTION_CATEGORY, Method = "/trade/finalize/{0}" },
                new { Id = 5, Name = "WithdrawFromTrade", Category = Constants.TRADE_ACTION_CATEGORY, Method = "/trade/withdraw/{0}/{1}" },
                new { Id = 6, Name = "JoinTrade", Category = Constants.TRADE_ACTION_CATEGORY, Method = Constants.JOIN_TRADE_ROUTE }
                ); 
        }

        public DbSet<User> Users { get; set; }
        private void OnCreateUsers(EntityTypeBuilder<User> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(r => r.Id);
            entityTypeBuilder.Property(r => r.Id).ValueGeneratedOnAdd();
            entityTypeBuilder.HasData( new { Id = -1, Name = "Phil", Created = true, Email="phil.miller84@gmail.com" } );
        }
        public DbSet<Requirement> Requirements { get; set; }
        private void OnCreateRequirements(EntityTypeBuilder<Requirement> entity)
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
            entity.Ignore(r => r.RelationshipType);

            //entity.HasOne(d => d.ActionDetails)
            //    .WithMany(p => p.Permissions)
            //    .HasForeignKey(d => d.ActionDetailsId);

            entity.HasOne(d => d.PermissionType)
                .WithMany(p => p.Requirements)
                .HasForeignKey(d => d.PermissionTypeId);

            entity.HasOne(d => d.RelationshipType);

            entity.HasData(
                new { Id = -1, PermissionTypeId = 4, RelationshipTypeId = 1, ActionId = 2 },
                new { Id = -2, PermissionTypeId = 4, RelationshipTypeId = 1, ActionId = 3 },
                new { Id = -3, PermissionTypeId = 4, RelationshipTypeId = 1, ActionId = 4 },
                new { Id = -4, PermissionTypeId = 4, RelationshipTypeId = 1, ActionId = 5 },
                new { Id = -5, PermissionTypeId = 6, RelationshipTypeId = 1, ActionId = 6 }
                );
        }
        
        public DbSet<PermissionType> PermissionTypes { get; set; }
        private void OnCreatePermissionTypes(EntityTypeBuilder<PermissionType> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(r => r.Id);
            entityTypeBuilder.HasData(
                new { Id = 1, Name = "Administer", Description = "Is admin" },
                new { Id = 2, Name = "Own", Description = "Owns the entity" },
                new { Id = 3, Name = "Review", Description = "Reviewing the entity" },
                new { Id = 4, Name = "Participate", Description = "Participating in the entity" },
                new { Id = 5, Name = Constants.VIEW_PERMISSION, Description = "Viewing the entity" },
                new { Id = 6, Name = Constants.JOIN_PERMISSION, Description = "Joining the entity" }
                );
        }

        public DbSet<State> States { get; set; }
        private void OnCreateStates(EntityTypeBuilder<State> e)
        {
            e.HasKey(r => r.Id);
            //e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.HasData(
                new { Id = 1, Description = Constants.TRADE_USER_IOI },
                new { Id = 2, Description = Constants.TRADE_USER_JOINED },
                new { Id = 3, Description = Constants.TRADE_USER_CONFIRMED },
                new { Id = 4, Description = Constants.TRADE_USER_EXCLUDED },
                new { Id = 5, Description = Constants.TRADE_USER_EXITED },
                new { Id = 6, Description = Constants.TRADE_USER_ADDED }
                );
        }


        //public DbSet<RelationshipState> RelationshipStates { get; set; }
        //private void OnCreateRelationshipStates(EntityTypeBuilder<RelationshipState> e)
        //{

        //}

        public DbSet<RelationshipType> RelationshipType { get; set; }
        private void OnCreateRelationshipTypes(EntityTypeBuilder<RelationshipType> e)
        {
            e.HasKey(r => r.Id);
            //e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.HasData( new { Id = 1, Name = "TradeUser" });
        }
        public DbSet<Permission> Permissions { get; set; }
        private void OnCreatePermissions(EntityTypeBuilder<Permission> entity)
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Id).ValueGeneratedOnAdd();
            entity.Ignore(r => r.RelationshipType);

            //entity.HasOne(d => d.ActionDetails)
            //    .WithMany(p => p.Permissions)
            //    .HasForeignKey(d => d.ActionDetailsId);

            entity.HasOne(d => d.PermissionType)
                .WithMany(p => p.Permissions)
                .HasForeignKey(d => d.PermissionTypeId);

            entity.HasOne(d => d.RelationshipType);

            //entity.HasData(
            //    new { Id = -1, PermissionTypeId= -4, RelationshipTypeId = -1, RelationId = -1 }
            //    );
        }

        public DbSet<Item> Items { get; set; }
        protected void OnCreateItems(EntityTypeBuilder<Item> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.HasData(
                new Item { Id = -1, Name = "Fruit", Measurement = "pound", Quantity = 3 },
                new Item { Id = -2, Name = "Clothes", Measurement = "piece", Quantity = 2 },
                new Item { Id = -3, Name = "Chair", Measurement = "piece", Quantity = 1 },
                new Item { Id = -4, Name = "Desk", Measurement = "piece", Quantity = 1 },
                new Item { Id = -5, Name = "Furniture", Measurement = "piece", Quantity = 3 },
                new Item { Id = -6, Name = "Shirt", Measurement = "piece", Quantity = 1 }
                );
        }
        public DbSet<ItemList> ItemList { get; set; }
        protected void OnCreateItemList(EntityTypeBuilder<ItemList> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.Ignore(r => r.Items);
            e.HasData(
                new ItemList { Id = -1 }
                );
        }
        protected void OnCreateItemList_Item(EntityTypeBuilder<ItemList_Item> e)
        {
            e.HasKey(t => new { t.ItemListId, t.ItemId });
            e.HasOne(ili => ili.Item)
                .WithMany(i => i.ItemList_Item)
                .HasForeignKey(ili => ili.ItemId);
            e.HasOne(ili => ili.ItemList)
                .WithMany(il => il.ItemList_Item)
                .HasForeignKey(ili => ili.ItemListId);
            e.HasData(
                new {ItemListId = -1, ItemId = -4 },
                new {ItemListId = -1, ItemId = -3 },
                new {ItemListId = -1, ItemId = -6 }
                );
        }
        public DbSet<Trade> Trades { get; set; }
        protected void OnCreateTrade(EntityTypeBuilder<Trade> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();

            e.HasOne(d => d.TradeItemList)
                .WithMany(p => p.Trades)
                .HasForeignKey(d => d.TradeItemListId);
            //e.HasOne(d => d.TradeUserList)
            //    .WithMany(p => p.Trades)
            //    .HasForeignKey(d => d.TradeItemListId);
            e.HasData(
                new { Id = -1, TimeStarted = "20200707", TradeItemListId = -1},
                new { Id = -2, TimeStarted = "20200708", TradeItemListId = -2}
                ); ;
        }


        // Do not need this in the entity, only on the SQL database
        public DbSet<TradeUser> TradeUsers{ get; set; }
        public void OnCreateTradeUser(EntityTypeBuilder<TradeUser> e)
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).ValueGeneratedOnAdd();

            e.HasOne(d => d.Trade)
                .WithMany(p => p.TradeUser)
                .HasForeignKey(d => d.TradeId);
            e.HasOne(d => d.User)
                .WithMany(p => p.TradeUser)
                .HasForeignKey(d => d.UserId);
            e.HasOne(d => d.State);

            e.HasData(new { Id = -1, TradeId = -1, UserId = -1, StateId = 1 });
        }

        public DbSet<TradeItemList> TradeItemLists { get; set; }
        private void OnCreateTradeItemLists(EntityTypeBuilder<TradeItemList> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.Ignore(r => r.TradeItemDetails);
            e.HasData(
                new { Id = -1 },
                new { Id = -2 }
                );
        }

        public DbSet<TradeItemDetails> TradeDetails { get; set; }
        protected void OnCreateTradeItemDetails(EntityTypeBuilder<TradeItemDetails> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.HasOne(d => d.Item);
                //.WithMany(p => p.TradeItemDetails)
                //.HasForeignKey(d => d.ItemId);
            e.HasOne(d => d.TradeItemList)
                .WithMany(p => p.TradeItemDetails)
                .HasForeignKey(d => d.TradeItemListId);
            e.HasData(
                new { Id = -1, ItemId = -1, FulfilledQuantity= 0, NeedOffset = 1, TradeItemListId = -1 },
                new { Id = -2, ItemId = -2, FulfilledQuantity= 0, NeedOffset = 1, TradeItemListId = -1 },
                new { Id = -3, ItemId = -3, FulfilledQuantity= 0, NeedOffset = 1, TradeItemListId = -1 },
                new { Id = -4, ItemId = -4, FulfilledQuantity= 0, NeedOffset = 1, TradeItemListId = -2 },
                new { Id = -5, ItemId = -5, FulfilledQuantity= 0, NeedOffset = 1, TradeItemListId = -2 },
                new { Id = -6, ItemId = -6, FulfilledQuantity= 0, NeedOffset = 1, TradeItemListId = -2 }
                );
        }
      }
}

