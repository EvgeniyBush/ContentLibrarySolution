using DAL2.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL2
{
    public class AppDbContext : DbContext
    {
        public DbSet<Content> Contents => Set<Content>();
        public DbSet<Book> Books => Set<Book>();
        public DbSet<Audio> Audios => Set<Audio>();
        public DbSet<Video> Videos => Set<Video>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<Storage> Storages => Set<Storage>();
        public DbSet<ContentLocation> ContentLocations => Set<ContentLocation>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=contentlibrary.db"); //бд з Sqlite
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Content>()
                .HasDiscriminator<string>("ContentType")
                .HasValue<Book>("Book")
                .HasValue<Audio>("Audio")
                .HasValue<Video>("Video")
                .HasValue<Document>("Document");
        }
    }
}
