using BookStore.Data;
using BookStore.Dtos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace BooksApi.Tests.Integration
{
    public class BooksApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public BooksApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<BooksDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database for testing
                    services.AddDbContext<BooksDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });
                });
            });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetBooks_ShouldReturnEmptyList_WhenNoBooksExist()
        {
            // Act
            var response = await _client.GetAsync("/api/books");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<BookDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(books);
            Assert.Empty(books);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnNotFound_WhenBookDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync("/api/books/999");
            
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
