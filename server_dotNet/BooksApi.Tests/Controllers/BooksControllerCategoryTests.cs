using BookStore.Controllers;
using BookStore.Dtos;
using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace BooksApi.Tests.Controllers
{
    public class BooksControllerCategoryTests
    {
        private BooksController SetupController(Mock<IBookService> mockBookService)
        {
            var controller = new BooksController(mockBookService.Object);
            
            // Setup user claims for authorization
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "testuser")
            }, "mock"));
            
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            
            return controller;
        }
        
        [Fact]
        public async Task CreateBook_WithValidCategories_ShouldReturnCreatedResult()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            var createBookDto = new CreateBookDto
            {
                Title = "Test Book",
                Author = "Test Author",
                UploadDate = DateTime.Now,
                Rank = 5,
                Categories = new List<string> { "Fantasy", "Adventure" }
            };
            
            var bookDto = new BookDto
            {
                Id = 1,
                Title = createBookDto.Title,
                Author = createBookDto.Author,
                UploadDate = createBookDto.UploadDate,
                Rank = createBookDto.Rank,
                Categories = createBookDto.Categories
            };
            
            mockBookService
                .Setup(s => s.CreateBookForUserAsync(It.IsAny<CreateBookDto>(), 1))
                .ReturnsAsync(bookDto);
                
            var controller = SetupController(mockBookService);
            
            // Act
            var result = await controller.CreateBook(createBookDto);
            
            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(StatusCodes.Status201Created, createdResult.StatusCode);
            var returnedBook = Assert.IsType<BookDto>(createdResult.Value);
            Assert.Equal(2, returnedBook.Categories.Count);
            Assert.Contains("Fantasy", returnedBook.Categories);
            Assert.Contains("Adventure", returnedBook.Categories);
        }
        
        [Fact]
        public async Task CreateBook_WithInvalidCategories_ShouldReturnBadRequest()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            var createBookDto = new CreateBookDto
            {
                Title = "Test Book",
                Author = "Test Author",
                UploadDate = DateTime.Now,
                Rank = 5,
                Categories = new List<string> { "InvalidCategory" }
            };
            
            mockBookService
                .Setup(s => s.CreateBookForUserAsync(It.IsAny<CreateBookDto>(), 1))
                .ThrowsAsync(new ArgumentException("One or more categories are invalid."));
                
            var controller = SetupController(mockBookService);
            
            // Act
            var result = await controller.CreateBook(createBookDto);
            
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var errorResponse = Assert.IsAssignableFrom<object>(badRequestResult.Value);
            Assert.Contains("One or more categories are invalid.", errorResponse.ToString());
        }
        
        [Fact]
        public async Task CreateBook_WithTooManyCategories_ShouldReturnBadRequest()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            var createBookDto = new CreateBookDto
            {
                Title = "Test Book",
                Author = "Test Author",
                UploadDate = DateTime.Now,
                Rank = 5,
                Categories = new List<string> { "Fantasy", "Adventure", "Mystery", "Horror" } // 4 categories (exceeds limit of 3)
            };
            
            mockBookService
                .Setup(s => s.CreateBookForUserAsync(It.IsAny<CreateBookDto>(), 1))
                .ThrowsAsync(new ArgumentException("Books can have at most 3 categories."));
                
            var controller = SetupController(mockBookService);
            
            // Act
            var result = await controller.CreateBook(createBookDto);
            
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateBook_WithValidCategories_ShouldReturnOk()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            var updateBookDto = new UpdateBookDto
            {
                Id = 1,
                Title = "Updated Book",
                Author = "Updated Author",
                UploadDate = DateTime.Now,
                Rank = 6,
                Categories = new List<string> { "Mystery", "Horror", "Drama" }
            };
            
            mockBookService
                .Setup(s => s.UpdateBookAsync(1, updateBookDto))
                .ReturnsAsync(true);
                
            var controller = SetupController(mockBookService);
            
            // Act
            var result = await controller.UpdateBook(1, updateBookDto);
              // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
        }
        
        [Fact]
        public async Task UpdateBook_WithInvalidCategories_ShouldReturnBadRequest()
        {
            // Arrange
            var mockBookService = new Mock<IBookService>();
            var updateBookDto = new UpdateBookDto
            {
                Id = 1,
                Title = "Updated Book",
                Author = "Updated Author",
                UploadDate = DateTime.Now,
                Rank = 6,
                Categories = new List<string> { "InvalidCategory" }
            };
            
            mockBookService
                .Setup(s => s.UpdateBookAsync(1, updateBookDto))
                .ThrowsAsync(new ArgumentException("One or more categories are invalid."));
                
            var controller = SetupController(mockBookService);
            
            // Act
            var result = await controller.UpdateBook(1, updateBookDto);
            
            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestResult.StatusCode);
            var errorResponse = Assert.IsAssignableFrom<object>(badRequestResult.Value);
            Assert.Contains("One or more categories are invalid.", errorResponse.ToString());
        }
    }
}
