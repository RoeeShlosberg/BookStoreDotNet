using BookStore.Data;
using BookStore.Dtos;
using BookStore.Models;
using BookStore.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BooksApi.Tests.Services
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

        private BookService CreateBookService(BooksDbContext context)
        {
            return new BookService(context);
        }

        [Fact]
        public async Task GetAllBooksAsync_ShouldReturnAllBooks()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author 1", PublishedDate = new DateTime(2020, 1, 1) },
                new Book { Id = 2, Title = "Book 2", Author = "Author 2", PublishedDate = new DateTime(2021, 1, 1) }
            };
            
            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllBooksAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Title == "Book 1");
            Assert.Contains(result, b => b.Title == "Book 2");
        }

        [Fact]
        public async Task GetBookByIdAsync_ExistingBook_ShouldReturnBook()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", PublishedDate = new DateTime(2023, 1, 1) };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetBookByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Title);
            Assert.Equal("Test Author", result.Author);
            Assert.Equal(new DateTime(2023, 1, 1), result.PublishedDate);
        }

        [Fact]
        public async Task GetBookByIdAsync_NonExistentBook_ShouldReturnNull()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);

            // Act
            var result = await service.GetBookByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateBookAsync_ValidBook_ShouldCreateAndReturnBook()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var createBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author",
                PublishedDate = new DateTime(2024, 1, 1)
            };

            // Act
            var result = await service.CreateBookAsync(createBookDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Book", result.Title);
            Assert.Equal("New Author", result.Author);
            Assert.Equal(new DateTime(2024, 1, 1), result.PublishedDate);
            Assert.True(result.Id > 0);

            // Verify it was saved to database
            var savedBook = await context.Books.FindAsync(result.Id);
            Assert.NotNull(savedBook);
            Assert.Equal("New Book", savedBook.Title);
        }

        [Fact]
        public async Task UpdateBookAsync_ExistingBook_ShouldUpdateAndReturnTrue()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var book = new Book { Id = 1, Title = "Old Title", Author = "Old Author", PublishedDate = new DateTime(2020, 1, 1) };
            context.Books.Add(book);
            await context.SaveChangesAsync();
            
            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author",
                PublishedDate = new DateTime(2025, 1, 1)
            };

            // Act
            var result = await service.UpdateBookAsync(1, updateBookDto);

            // Assert
            Assert.True(result);

            // Verify it was updated in database
            var updatedBook = await context.Books.FindAsync(1);
            Assert.NotNull(updatedBook);
            Assert.Equal("Updated Title", updatedBook.Title);
            Assert.Equal("Updated Author", updatedBook.Author);
            Assert.Equal(new DateTime(2025, 1, 1), updatedBook.PublishedDate);
        }

        [Fact]
        public async Task UpdateBookAsync_NonExistentBook_ShouldReturnFalse()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author",
                PublishedDate = new DateTime(2025, 1, 1)
            };

            // Act
            var result = await service.UpdateBookAsync(999, updateBookDto);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteBookAsync_ExistingBook_ShouldReturnTrue()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var book = new Book { Id = 1, Title = "Book to Delete", Author = "Author", PublishedDate = new DateTime(2021, 1, 1) };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act
            var result = await service.DeleteBookAsync(1);

            // Assert
            Assert.True(result);

            // Verify it was deleted from database
            var deletedBook = await context.Books.FindAsync(1);
            Assert.Null(deletedBook);
        }

        [Fact]
        public async Task DeleteBookAsync_NonExistentBook_ShouldReturnFalse()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);

            // Act
            var result = await service.DeleteBookAsync(999);

            // Assert
            Assert.False(result);
        }
    }
}
