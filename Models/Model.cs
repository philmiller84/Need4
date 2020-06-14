using Need4Protocol;
using Microsoft.EntityFrameworkCore;

namespace Models
{
    public class ItemContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options) => options.UseSqlite("Data Source=blogging.db");
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API
            modelBuilder.Entity<Item>().HasKey(r => r.Name);
        }
    }
}
