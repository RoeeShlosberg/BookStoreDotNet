using Xunit;
using BookStore.Services;
using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BooksApi.Tests
{
    public class BookServiceTests
    {
        private BooksDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BooksDbContext(options);
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnAllBooks()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new BookService(context);
              var book1 = new Book { Id = 1, Title = "Book 1", Author = "Author 1" };
            var book2 = new Book { Id = 2, Title = "Book 2", Author = "Author 2" };
            
            await context.Books.AddRangeAsync(book1, book2);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllBooksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetBookByIdAsync_ShouldReturnBook_WhenBookExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = new BookService(context);
            
            var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author" };
            await context.Books.AddAsync(book);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetBookByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Title);
        }
    }
}