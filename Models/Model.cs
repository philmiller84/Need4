using Need4Protocol;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;

namespace Models
{
    public class ItemContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
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
        }
    }
}

