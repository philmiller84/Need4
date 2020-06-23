using Microsoft.EntityFrameworkCore;
using Need4Protocol;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Need4Context : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemList> ItemList { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<TradeItemDetails> TradeDetails { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //string ConnectionString = "Data Source=" + Path.Combine(Directory.GetCurrentDirectory(), "need4.db");
                string ConnectionString = "Data Source=need4.db";
                Console.WriteLine(ConnectionString);

                try
                {
                    optionsBuilder.UseSqlite(ConnectionString); 
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API
            modelBuilder.Entity<Item>().HasKey(r => r.Name);

            // Configure the primary key
            modelBuilder.Entity<ItemList>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).ValueGeneratedOnAdd();
            });
            //Ignore this field so it does not end up in DB table. We will use join instead.
            modelBuilder.Entity<ItemList>().Ignore(r => r.List);

            modelBuilder.Entity<ItemListItem>()
                .HasKey(t => new { t.Id, t.Name });

            modelBuilder.Entity<ItemListItem>()
                .HasOne(ili => ili.Item)
                .WithMany(i => i.joins)
                .HasForeignKey(ili => ili.Name);

            modelBuilder.Entity<ItemListItem>()
                .HasOne(ili => ili.ItemList)
                .WithMany(i => i.joins)
                .HasForeignKey(ili => ili.Id);
               

            //modelBuilder.Entity<ItemList>()
            //    .HasMany<Item>(t => t.items);

            //modelBuilder.Entity<Item>()
            //    .HasMany<ItemList>(t => t.lists);
            
            modelBuilder.Entity<TradeItemDetails>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<Trade>(b =>
            {
                b.HasKey(r => r.Id);
                b.Property(r => r.Id).ValueGeneratedOnAdd();
            });
        }
    }
}

