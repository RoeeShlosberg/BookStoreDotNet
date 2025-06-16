using BookStore.Controllers;
using BookStore.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BooksApi.Tests.Controllers
{
    public class CategoriesControllerTests
    {
        [Fact]
        public void GetCategories_ShouldReturnAllowedCategories()
        {
            // Arrange
            var controller = new CategoriesController();

            // Act
            var result = controller.GetCategories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var categories = Assert.IsType<List<string>>(okResult.Value);
            
            // Verify all allowed categories are returned
            Assert.Equal(CategoryStore.AllowedCategories.Count, categories.Count);
            foreach (var category in CategoryStore.AllowedCategories)
            {
                Assert.Contains(category, categories);
            }
        }
    }
}
