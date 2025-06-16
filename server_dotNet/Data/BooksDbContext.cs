using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using BookStore.Models;

namespace BookStore.Data
{
    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }  // Represents the Books table in the database
        public DbSet<User> Users { get; set; } // Represents the Users table in the database
        public DbSet<BookUser> BookUsers { get; set; } // Join table for books and users
        public DbSet<SharedBookList> SharedBookLists { get; set; } // Table for shared book lists

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Book>()
                .Property(b => b.Categories)
                .HasConversion(new ValueConverter<List<string>, string>(
                    v => string.Join(";", v ?? new List<string>()),
                    v => v.Split(';', System.StringSplitOptions.RemoveEmptyEntries).ToList()
                ));

            modelBuilder.Entity<BookUser>()
                .HasIndex(bu => new { bu.UserId, bu.BookId })
                .IsUnique();

            modelBuilder.Entity<BookUser>()
                .HasOne(bu => bu.User)
                .WithMany()
                .HasForeignKey(bu => bu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookUser>()
                .HasOne(bu => bu.Book)
                .WithMany()
                .HasForeignKey(bu => bu.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // SharedBookList: store BookIds as a semicolon-separated string
            modelBuilder.Entity<SharedBookList>()
                .Property(s => s.BookIds)
                .HasConversion(
                    v => string.Join(";", v),
                    v => v.Split(';', System.StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                );
        }
    }
}
