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
                new Book { Id = 1, Title = "Book 1", Author = "Author 1", UploadDate = new DateTime(2020, 1, 1), Rank = 5, Categories = new List<string> { "Drama" } },
                new Book { Id = 2, Title = "Book 2", Author = "Author 2", UploadDate = new DateTime(2021, 1, 1), Rank = 6, Categories = new List<string> { "Sci-Fi", "Adventure" } }
            };
            
            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllBooksAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Title == "Book 1");
            Assert.Contains(result, b => b.Title == "Book 2");
            Assert.Contains(result, b => b.Categories.Contains("Drama"));
            Assert.Contains(result, b => b.Categories.Contains("Sci-Fi") && b.Categories.Contains("Adventure"));
        }

        [Fact]
        public async Task GetBookByIdAsync_ExistingBook_ShouldReturnBook()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var book = new Book { Id = 1, Title = "Test Book", Author = "Test Author", UploadDate = new DateTime(2023, 1, 1), Rank = 5, Categories = new List<string> { "Drama", "Fantasy" } };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetBookByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Title);
            Assert.Equal("Test Author", result.Author);
            Assert.Equal(new DateTime(2023, 1, 1), result.UploadDate);
            Assert.Equal(5, result.Rank);
            Assert.Equal(2, result.Categories.Count);
            Assert.Contains("Drama", result.Categories);
            Assert.Contains("Fantasy", result.Categories);
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
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 7,
                Categories = new List<string> { "Mystery", "Horror" }
            };

            // Act
            var result = await service.CreateBookAsync(createBookDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Book", result.Title);
            Assert.Equal("New Author", result.Author);
            Assert.Equal(new DateTime(2024, 1, 1), result.UploadDate);
            Assert.Equal(7, result.Rank);
            Assert.Equal(2, result.Categories.Count);
            Assert.Contains("Mystery", result.Categories);
            Assert.Contains("Horror", result.Categories);
            Assert.True(result.Id > 0);

            // Verify it was saved to database
            var savedBook = await context.Books.FindAsync(result.Id);
            Assert.NotNull(savedBook);
            Assert.Equal("New Book", savedBook.Title);
            Assert.Equal(2, savedBook.Categories.Count);
            Assert.Contains("Mystery", savedBook.Categories);
            Assert.Contains("Horror", savedBook.Categories);
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
        public async Task UpdateBookAsync_ExistingBook_ShouldUpdateAndReturnTrue()
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
                Categories = new List<string> { "Fantasy", "Adventure" }
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
            Assert.Equal(new DateTime(2025, 1, 1), updatedBook.UploadDate);
            Assert.Equal(8, updatedBook.Rank);
            Assert.Equal(2, updatedBook.Categories.Count);
            Assert.Contains("Fantasy", updatedBook.Categories);
            Assert.Contains("Adventure", updatedBook.Categories);
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
        public async Task UpdateBookAsync_NonExistentBook_ShouldReturnFalse()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Title",
                Author = "Updated Author",
                UploadDate = new DateTime(2025, 1, 1),
                Rank = 8,
                Categories = new List<string> { "Fantasy", "Adventure" }
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
            
            var book = new Book { 
                Id = 1, 
                Title = "Book to Delete", 
                Author = "Author", 
                UploadDate = new DateTime(2021, 1, 1),
                Rank = 3,
                Categories = new List<string> { "Drama" }
            };
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

        [Fact]
        public async Task SearchBooksAsync_ShouldReturnMatchingBooks()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Mystery Novel", Author = "Author 1", UploadDate = new DateTime(2020, 1, 1), Rank = 5, Categories = new List<string> { "Mystery", "Drama" } },
                new Book { Id = 2, Title = "Science Fiction", Author = "Author 2", UploadDate = new DateTime(2021, 1, 1), Rank = 6, Categories = new List<string> { "Sci-Fi" } },
                new Book { Id = 3, Title = "Historical Facts", Author = "Mystery Writer", UploadDate = new DateTime(2022, 1, 1), Rank = 7, Categories = new List<string> { "Non-Fiction", "History" } }
            };
            
            context.Books.AddRange(books);
            await context.SaveChangesAsync();

            // Act
            var result = await service.SearchBooksAsync("Mystery");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Title == "Mystery Novel");
            Assert.Contains(result, b => b.Author == "Mystery Writer");
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
        public async Task GetAllBooksForUserAsync_ShouldReturnUserBooks()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateBookService(context);
            
            // Add users
            var user1 = new User { Id = 1, Username = "user1", Password = "hash" };
            var user2 = new User { Id = 2, Username = "user2", Password = "hash" };
            context.Users.AddRange(user1, user2);
            
            // Add books
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author 1", UploadDate = new DateTime(2020, 1, 1), Rank = 5, Categories = new List<string> { "Drama" } },
                new Book { Id = 2, Title = "Book 2", Author = "Author 2", UploadDate = new DateTime(2021, 1, 1), Rank = 6, Categories = new List<string> { "Sci-Fi" } },
                new Book { Id = 3, Title = "Book 3", Author = "Author 3", UploadDate = new DateTime(2022, 1, 1), Rank = 7, Categories = new List<string> { "Fantasy" } }
            };
            context.Books.AddRange(books);
            
            // Associate books with users
            var bookUsers = new List<BookUser>
            {
                new BookUser { UserId = 1, BookId = 1 },
                new BookUser { UserId = 1, BookId = 2 },
                new BookUser { UserId = 2, BookId = 3 }
            };
            context.BookUsers.AddRange(bookUsers);
            
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllBooksForUserAsync(1);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Title == "Book 1");
            Assert.Contains(result, b => b.Title == "Book 2");
            Assert.DoesNotContain(result, b => b.Title == "Book 3");
        }

        [Fact]
        public async Task SearchBooksForUserAsync_ShouldReturnMatchingUserBooks()
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
                new Book { Id = 2, Title = "Adventure Tale", Author = "Fantasy Writer", UploadDate = new DateTime(2021, 1, 1), Rank = 6, Categories = new List<string> { "Adventure" } },
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

            // Act
            var result = await service.SearchBooksForUserAsync("Fantasy", 1);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Title == "Fantasy Book");
            Assert.Contains(result, b => b.Author == "Fantasy Writer");
            Assert.DoesNotContain(result, b => b.Title == "Mystery Novel");
        }

        [Fact]
        public async Task GetBooksForUserByCategoryAsync_ShouldReturnMatchingBooks()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            
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

            // Act - Manually query by category since there's no specific method for this yet
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
