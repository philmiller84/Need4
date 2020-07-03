using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Need4Protocol;
using System;

namespace Models
{
    public class Need4Context : DbContext
    {
        public DbSet<Item> Items { get; set; }
        protected void OnCreateItems(EntityTypeBuilder<Item> e)
        {
            e.HasKey(r => r.Name);
        }
        public DbSet<ItemList> ItemList { get; set; }
        protected void OnCreateItemList(EntityTypeBuilder<ItemList> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.Ignore(r => r.Items);
        }
        protected void OnCreateItemList_Item(EntityTypeBuilder<ItemList_Item> e)
        {
            e.HasKey(t => new { t.Id, t.Name });
            e.HasOne(ili => ili.Item)
                .WithMany(i => i.joins)
                .HasForeignKey(ili => ili.Name);
            e.HasOne(ili => ili.ItemList)
                .WithMany(il => il.joins)
                .HasForeignKey(ili => ili.Id);
        }
        public DbSet<Trade> Trades { get; set; }
        protected void OnCreateTrade(EntityTypeBuilder<Trade> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
            e.Property(x => x.Started).HasColumnType("datetime");
        }
        public DbSet<TradeItemDetails> TradeDetails { get; set; }
        protected void OnCreateTradeItemDetails(EntityTypeBuilder<TradeItemDetails> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
        }
        public DbSet<TradeAction> TradeActions { get; set; }
        protected void OnCreateTradeActions(EntityTypeBuilder<TradeAction> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Permission> Permissions { get; set; }

        protected void OnCreateT_Timestamp(EntityTypeBuilder<T_Timestamp> e)
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.Id).ValueGeneratedOnAdd();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //string ConnectionString = "Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), "need4.db");
                string ConnectionString = "Data Source=C:\\Users\\Phil\\Repo\\Need4\\Models\\need4.db";
                //Console.WriteLine(ConnectionString);

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
            OnCreateItems(modelBuilder.Entity<Item>());
            OnCreateItemList(modelBuilder.Entity<ItemList>());
            OnCreateItemList_Item(modelBuilder.Entity<ItemList_Item>());
            OnCreateTrade(modelBuilder.Entity<Trade>());
            OnCreateTradeItemDetails(modelBuilder.Entity<TradeItemDetails>());
            OnCreateTradeActions(modelBuilder.Entity<TradeAction>());
            OnCreateT_Timestamp(modelBuilder.Entity<T_Timestamp>());
            modelBuilder.Ignore<T_Timestamp>();
        }
    }
}

