using BookStore.Controllers;
using BookStore.Dtos;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace BooksApi.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly BooksController _controller;        public BooksControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _controller = new BooksController(_mockBookService.Object);
            
            // Setup mock user claims for authorization
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            
            // Setup ControllerContext with the mocked User
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }[Fact]
        public async Task GetBooks_ShouldReturnOkWithBooks()
        {
            // Arrange
            var books = new List<BookDto>
            {
                new BookDto { Id = 1, Title = "Book 1", Author = "Author 1", UploadDate = new DateTime(2020, 1, 1), Rank = 4, Categories = new List<string> { "Drama" } },
                new BookDto { Id = 2, Title = "Book 2", Author = "Author 2", UploadDate = new DateTime(2021, 1, 1), Rank = 5, Categories = new List<string> { "Mystery" } }
            };
            
            _mockBookService.Setup(x => x.GetAllBooksForUserAsync(1)).ReturnsAsync(books);

            // Act
            var result = await _controller.GetBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBooks = Assert.IsAssignableFrom<IEnumerable<BookDto>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count());
        }

        [Fact]
        public async Task GetBook_ExistingBook_ShouldReturnOkWithBook()
        {
            // Arrange
            var book = new BookDto { Id = 1, Title = "Test Book", Author = "Test Author", UploadDate = new DateTime(2023, 1, 1), Rank = 4, Categories = new List<string> { "Drama" } };
            _mockBookService.Setup(x => x.GetBookByIdAsync(1)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetBook(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBook = Assert.IsType<BookDto>(okResult.Value);
            Assert.Equal("Test Book", returnedBook.Title);
        }

        [Fact]
        public async Task GetBook_NonExistentBook_ShouldReturnNotFound()
        {
            // Arrange
            _mockBookService.Setup(x => x.GetBookByIdAsync(999)).ReturnsAsync((BookDto?)null);

            // Act
            var result = await _controller.GetBook(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }        [Fact]
        public async Task CreateBook_ValidBook_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 4,
                Categories = new List<string> { "Drama", "Fantasy" }
            };
              var createdBook = new BookDto
            {
                Id = 1,
                Title = "New Book",
                Author = "New Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 4,
                Categories = new List<string> { "Drama", "Fantasy" }
            };
            
            _mockBookService.Setup(x => x.CreateBookForUserAsync(createBookDto, 1)).ReturnsAsync(createdBook);

            // Act
            var result = await _controller.CreateBook(createBookDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(BooksController.GetBook), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues!["id"]);
            
            var returnedBook = Assert.IsType<BookDto>(createdAtActionResult.Value);
            Assert.Equal("New Book", returnedBook.Title);
        }

        [Fact]
        public async Task CreateBook_InvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Title is required");            var createBookDto = new CreateBookDto
            {
                Title = "", // Invalid
                Author = "Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 3,
                Categories = new List<string> { "Fantasy" }
            };

            // Act
            var result = await _controller.CreateBook(createBookDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateBook_ExistingBook_ShouldReturnNoContent()
        {
            // Arrange
            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Book",
                Author = "Updated Author",
                UploadDate = new DateTime(2025, 1, 1),
                Rank = 5,
                Categories = new List<string> { "Mystery", "Sci-Fi" }
            };
            
            _mockBookService.Setup(x => x.UpdateBookAsync(1, updateBookDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateBook(1, updateBookDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateBook_NonExistentBook_ShouldReturnNotFound()
        {
            // Arrange
            var updateBookDto = new UpdateBookDto
            {
                Title = "Updated Book",
                Author = "Updated Author",
                UploadDate = new DateTime(2025, 1, 1),
                Rank = 5,
                Categories = new List<string> { "Mystery", "Sci-Fi" }
            };
            
            _mockBookService.Setup(x => x.UpdateBookAsync(999, updateBookDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBook(999, updateBookDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateBook_InvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Title", "Title is required");
            
            var updateBookDto = new UpdateBookDto
            {
                Title = "", // Invalid
                Author = "Author",
                UploadDate = new DateTime(2024, 1, 1),
                Rank = 3,
                Categories = new List<string> { "Fantasy" }
            };

            // Act
            var result = await _controller.UpdateBook(1, updateBookDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteBook_ExistingBook_ShouldReturnNoContent()
        {
            // Arrange
            _mockBookService.Setup(x => x.DeleteBookAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteBook(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteBook_NonExistentBook_ShouldReturnNotFound()
        {
            // Arrange
            _mockBookService.Setup(x => x.DeleteBookAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteBook(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
