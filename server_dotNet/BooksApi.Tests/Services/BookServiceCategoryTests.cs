using BookStore.Data;
using BookStore.Dtos;
using BookStore.Models;
using BookStore.Services;
using BookStore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Tests.Services
{
    public class BookServiceCategoryTests
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
        public async Task CreateBookAsync_InvalidCategories_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var createBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 7,
                Categories = new List<string> { "InvalidCategory" }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateBookAsync(createBookDto));
        }

        [Fact]
        public async Task CreateBookAsync_EmptyCategories_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var createBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 7,
                Categories = new List<string>() // Empty categories
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateBookAsync(createBookDto));
        }

        [Fact]
        public async Task CreateBookAsync_TooManyCategories_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var createBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 7,
                Categories = new List<string> { "Drama", "Fantasy", "Mystery", "Horror" } // 4 categories (exceeds limit of 3)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateBookAsync(createBookDto));
        }

        [Fact]
        public async Task UpdateBookAsync_InvalidCategories_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var book = new Book { 
                Id = 1, 
                Title = "Old Title", 
                Author = "Old Author", 
                UploadDate = new DateTime(2020, 1, 1),
                Rank = 4,
                Categories = new List<string> { "Non-Fiction", "History" }
            };
            context.Books.Add(book);
            await context.SaveChangesAsync();
            
            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author",
                UploadDate = new DateTime(2025, 1, 1),
                Rank = 8,
                Categories = new List<string> { "InvalidCategory" }
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateBookAsync(1, updateBookDto));
        }

        [Fact]
        public async Task UpdateBookAsync_EmptyCategories_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var book = new Book { 
                Id = 1, 
                Title = "Old Title", 
                Author = "Old Author", 
                UploadDate = new DateTime(2020, 1, 1),
                Rank = 4,
                Categories = new List<string> { "Non-Fiction", "History" }
            };
            context.Books.Add(book);
            await context.SaveChangesAsync();
            
            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author",
                UploadDate = new DateTime(2025, 1, 1),
                Rank = 8,
                Categories = new List<string>() // Empty categories
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateBookAsync(1, updateBookDto));
        }

        [Fact]
        public async Task UpdateBookAsync_TooManyCategories_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var book = new Book { 
                Id = 1, 
                Title = "Old Title", 
                Author = "Old Author", 
                UploadDate = new DateTime(2020, 1, 1),
                Rank = 4,
                Categories = new List<string> { "Non-Fiction", "History" }
            };
            context.Books.Add(book);
            await context.SaveChangesAsync();
            
            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author",
                UploadDate = new DateTime(2025, 1, 1),
                Rank = 8,
                Categories = new List<string> { "Drama", "Fantasy", "Mystery", "Horror" } // 4 categories (exceeds limit of 3)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateBookAsync(1, updateBookDto));
        }

        [Fact]
        public async Task CreateBookForUserAsync_ValidBook_ShouldCreateBookAndAssociation()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            // Add a user
            var user = new User { Id = 1, Username = "testuser", Password = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            
            var createBookDto = new CreateBookDto
            {
                Title = "User's Book",
                Author = "User's Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 9,
                Categories = new List<string> { "Fantasy", "Adventure", "Horror" }
            };

            // Act
            var result = await service.CreateBookForUserAsync(createBookDto, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("User's Book", result.Title);
            Assert.Equal(3, result.Categories.Count);
            Assert.Contains("Fantasy", result.Categories);
            Assert.Contains("Adventure", result.Categories);
            Assert.Contains("Horror", result.Categories);

            // Verify book-user association was created
            var bookUser = await context.BookUsers.FirstOrDefaultAsync(bu => bu.BookId == result.Id && bu.UserId == 1);
            Assert.NotNull(bookUser);
        }

        [Fact]
        public async Task CreateBookForUserAsync_TooManyCategories_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            // Add a user
            var user = new User { Id = 1, Username = "testuser", Password = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            
            var createBookDto = new CreateBookDto
            {
                Title = "User's Book",
                Author = "User's Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 9,
                Categories = new List<string> { "Fantasy", "Adventure", "Horror", "Mystery" } // 4 categories (exceeds limit of 3)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateBookForUserAsync(createBookDto, 1));
        }

        [Fact]
        public async Task CreateBookForUserAsync_EmptyCategories_ShouldThrowArgumentException()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            // Add a user
            var user = new User { Id = 1, Username = "testuser", Password = "hash" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            
            var createBookDto = new CreateBookDto
            {
                Title = "User's Book",
                Author = "User's Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 9,
                Categories = new List<string>() // Empty categories list
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateBookForUserAsync(createBookDto, 1));
        }

        [Fact]
        public async Task SearchBooksForCategoryAsync_ShouldReturnBooksWithCategory()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Fantasy Book", Author = "Author 1", UploadDate = new DateTime(2020, 1, 1), Rank = 5, Categories = new List<string> { "Fantasy", "Adventure" } },
                new Book { Id = 2, Title = "Adventure Tale", Author = "Author 2", UploadDate = new DateTime(2021, 1, 1), Rank = 6, Categories = new List<string> { "Adventure" } },
                new Book { Id = 3, Title = "Mystery Novel", Author = "Author 3", UploadDate = new DateTime(2022, 1, 1), Rank = 7, Categories = new List<string> { "Mystery" } }
            };
            
            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            // Act - This would call a method you may want to add to your service
            var result = await context.Books
                .Where(b => b.Categories.Contains("Adventure"))
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    UploadDate = b.UploadDate,
                    Rank = b.Rank,
                    Categories = b.Categories
                })
                .ToListAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Title == "Fantasy Book");
            Assert.Contains(result, b => b.Title == "Adventure Tale");
            Assert.DoesNotContain(result, b => b.Title == "Mystery Novel");
        }

        [Fact]
        public async Task GetAllBooksForUserAndCategoryAsync_ShouldReturnMatchingBooks()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            // Add users
            var user1 = new User { Id = 1, Username = "user1", Password = "hash" };
            context.Users.Add(user1);
            
            // Add books
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Fantasy Book", Author = "Author 1", UploadDate = new DateTime(2020, 1, 1), Rank = 5, Categories = new List<string> { "Fantasy", "Adventure" } },
                new Book { Id = 2, Title = "Adventure Tale", Author = "Author 2", UploadDate = new DateTime(2021, 1, 1), Rank = 6, Categories = new List<string> { "Adventure" } },
                new Book { Id = 3, Title = "Mystery Novel", Author = "Author 3", UploadDate = new DateTime(2022, 1, 1), Rank = 7, Categories = new List<string> { "Mystery" } }
            };
            context.Books.AddRange(books);
            
            // Associate books with users
            var bookUsers = new List<BookUser>
            {
                new BookUser { UserId = 1, BookId = 1 },
                new BookUser { UserId = 1, BookId = 2 },
                new BookUser { UserId = 1, BookId = 3 }
            };
            context.BookUsers.AddRange(bookUsers);
            
            await context.SaveChangesAsync();

            // Act - This would call a method you may want to add to your service
            var result = await context.BookUsers
                .Where(bu => bu.UserId == 1 && bu.Book.Categories.Contains("Adventure"))
                .Select(bu => new BookDto
                {
                    Id = bu.Book.Id,
                    Title = bu.Book.Title,
                    Author = bu.Book.Author,
                    UploadDate = bu.Book.UploadDate,
                    Rank = bu.Book.Rank,
                    Categories = bu.Book.Categories
                })
                .ToListAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Title == "Fantasy Book");
            Assert.Contains(result, b => b.Title == "Adventure Tale");
            Assert.DoesNotContain(result, b => b.Title == "Mystery Novel");
        }
    }
}
