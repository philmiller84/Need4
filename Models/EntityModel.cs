using Microsoft.EntityFrameworkCore;
using Need4Protocol;
using System;

namespace Models
{
    public class Need4Context : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemList> ItemList { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradeItemDetails> TradeDetails { get; set; }
        public DbSet<TradeAction> TradeActions { get; set; }
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
            // ITEM
            modelBuilder.Entity<Item>().HasKey(r => r.Name);

            //ITEMLIST
            modelBuilder.Entity<ItemList>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).ValueGeneratedOnAdd();
            });

            //Ignore this field so it does not end up in DB table. We will use join instead.
            modelBuilder.Entity<ItemList>().Ignore(r => r.Items);

            //ITEMLIST+ITEM JOIN TABLE
            modelBuilder.Entity<ItemList_Item>()
                .HasKey(t => new { t.Id, t.Name });

            modelBuilder.Entity<ItemList_Item>()
                .HasOne(ili => ili.Item)
                .WithMany(i => i.joins)
                .HasForeignKey(ili => ili.Name);

            modelBuilder.Entity<ItemList_Item>()
                .HasOne(ili => ili.ItemList)
                .WithMany(i => i.joins)
                .HasForeignKey(ili => ili.Id);



            //TRADEACTION
            modelBuilder.Entity<TradeAction>(a =>
            {
                a.HasKey(r => r.Id);
                a.Property(r => r.Id).ValueGeneratedOnAdd();
            });

            //TRADEITEMDETAILS
            modelBuilder.Entity<TradeItemDetails>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).ValueGeneratedOnAdd();
            });

            //TRADE
            modelBuilder.Entity<Trade>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).ValueGeneratedOnAdd();
                b.Property(x => x.Started).HasColumnType("datetime");
            });

            //T_Timestamp
            modelBuilder.Entity<T_Timestamp>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Ignore<T_Timestamp>();
        }
    }
}

