using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Need4Protocol;
using System;

namespace Models
{
    public class Need4Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //string ConnectionString = "Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), "need4.db");
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
            OnCreatePermissionTypes(modelBuilder.Entity<PermissionType>());
            OnCreateActionTypes(modelBuilder.Entity<ActionType>());
            OnCreateUsers(modelBuilder.Entity<User>());
            OnCreatePermissions(modelBuilder.Entity<Permission>());
            OnCreateItems(modelBuilder.Entity<Item>());
            OnCreateItemList(modelBuilder.Entity<ItemList>());
            OnCreateItemList_Item(modelBuilder.Entity<ItemList_Item>());
            OnCreateTrade(modelBuilder.Entity<Trade>());
            OnCreateTradeItemLists(modelBuilder.Entity<TradeItemList>());
            OnCreateTradeItemDetails(modelBuilder.Entity<TradeItemDetails>());
            OnCreateTradeItemList_TradeItemDetails(modelBuilder.Entity<TradeItemList_TradeItemDetails>());
            OnCreateTradeActions(modelBuilder.Entity<TradeAction>());
        }



        public DbSet<PermissionType> PermissionTypes { get; set; }
        private void OnCreatePermissionTypes(EntityTypeBuilder<PermissionType> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(r => r.Id);
            entityTypeBuilder.Property(r => r.Id).ValueGeneratedOnAdd();
            entityTypeBuilder.HasData(
                new { Id = -1, Name = "Owner", Description = "The entity is owned" },
                new { Id = -2, Name = "Reviewer", Description = "The entity is being reviewed" },
                new { Id = -3, Name = "Participant", Description = "The entity is being participated in" },
                new { Id = -4, Name = "Any", Description = "Any status has permission to entity" }
                );
        }

        public DbSet<ActionType> ActionTypes { get; set; }
        private void OnCreateActionTypes(EntityTypeBuilder<ActionType> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(r => r.Id);
            entityTypeBuilder.Property(r => r.Id).ValueGeneratedOnAdd();
            entityTypeBuilder.HasData(new { Id = -1, Name = "GetTradeData", Description = "/get/trade/{0}" });
        }

        public DbSet<User> Users { get; set; }
        private void OnCreateUsers(EntityTypeBuilder<User> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(r => r.Id);
            entityTypeBuilder.Property(r => r.Id).ValueGeneratedOnAdd();
            entityTypeBuilder.HasData( new { Id = -1, Name = "Phil" } );
        }
        public DbSet<Permission> Permissions { get; set; }
        private void OnCreatePermissions(EntityTypeBuilder<Permission> entityTypeBuilder)
        {
            entityTypeBuilder.HasKey(r => r.Id);
            entityTypeBuilder.Property(r => r.Id).ValueGeneratedOnAdd();
            entityTypeBuilder.HasData( new { Id = -1, PermissionTypeId = -4, ActionTypeId = -1 } /* LEGIT: this seed data refers to child entities */ );
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
                .WithMany(i => i.Joins)
                .HasForeignKey(ili => ili.ItemId);
            e.HasOne(ili => ili.ItemList)
                .WithMany(il => il.Joins)
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
            e.HasData(
                new { Id = -1, TimeStarted = "20200707", TradeItemsListId = -1 }
                ); ;
        }

        public DbSet<TradeItemList> TradeItemLists { get; set; }
        private void OnCreateTradeItemLists(EntityTypeBuilder<TradeItemList> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.Ignore(r => r.TradeItemDetails);
            e.HasData(
                new { Id = -1, TradeItemDetailsId = -1 }
                );
        }


        public DbSet<TradeItemDetails> TradeDetails { get; set; }
        protected void OnCreateTradeItemDetails(EntityTypeBuilder<TradeItemDetails> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.HasData(
                new { Id = -1, ItemId = -1, FulfilledQuantity= 0, NeedOffset = 1 }
                );
        }

        private void OnCreateTradeItemList_TradeItemDetails(EntityTypeBuilder<TradeItemList_TradeItemDetails> e)
        {
            e.HasKey(r => new { r.TradeItemListId, r.TradeItemDetailsId});
            e.HasOne(r => r.TradeItemList)
                .WithMany(r => r.Joins)
                .HasForeignKey(r => r.TradeItemListId);
            e.HasOne(r => r.TradeItemDetails)
                .WithMany(r => r.Joins)
                .HasForeignKey(r => r.TradeItemDetailsId);
            e.HasData(
                new {TradeItemListId = -1, TradeItemDetailsId = -1}
                );
        }
        public DbSet<TradeAction> TradeActions { get; set; }
        protected void OnCreateTradeActions(EntityTypeBuilder<TradeAction> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
        }

      }
}

