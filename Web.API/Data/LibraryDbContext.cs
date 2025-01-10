using Microsoft.EntityFrameworkCore;

namespace Web.API.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options) {}
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Author).IsRequired();
            entity.Property(e => e.ReleaseDate).IsRequired();
        });
    }
}