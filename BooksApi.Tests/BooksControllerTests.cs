using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BookStore.Controllers;
using BookStore.Services;
using BookStore.Dtos;

namespace BooksApi.Tests
{
    public class BooksControllerTests
    {
        private readonly Mock<IBookService> _mockBookService;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            _mockBookService = new Mock<IBookService>();
            _controller = new BooksController(_mockBookService.Object);
        }

        [Fact]
        public async Task GetBooks_ShouldReturnOkResult()
        {
            // Arrange
            var books = new List<BookDto>
            {
                new BookDto { Id = 1, Title = "Book 1", Author = "Author 1" },
                new BookDto { Id = 2, Title = "Book 2", Author = "Author 2" }
            };
            _mockBookService.Setup(s => s.GetAllBooksAsync()).ReturnsAsync(books);

            // Act
            var result = await _controller.GetBooks();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBooks = Assert.IsAssignableFrom<List<BookDto>>(okResult.Value);
            Assert.Equal(2, returnedBooks.Count);
        }

        [Fact]
        public async Task GetBook_ShouldReturnOkResult_WhenBookExists()
        {
            // Arrange
            var book = new BookDto { Id = 1, Title = "Test Book", Author = "Test Author" };
            _mockBookService.Setup(s => s.GetBookByIdAsync(1)).ReturnsAsync(book);

            // Act
            var result = await _controller.GetBook(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBook = Assert.IsType<BookDto>(okResult.Value);
            Assert.Equal("Test Book", returnedBook.Title);
        }
    }
}