using Helloworld;
using Microsoft.EntityFrameworkCore;
using System;

namespace Models
{
    public class BloggingContext : DbContext
    {
        public DbSet<HelloRequest> HelloRequests { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=blogging.db");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HelloRequest>().HasKey(r => r.Name);
        }
    }
}
