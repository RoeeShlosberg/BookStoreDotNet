using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;
using BookStore.Dtos;
using System.Text;
using System.Text.Json;
using System.Net;

namespace BooksApi.Tests
{
    public class BooksApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;        public BooksApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove all Entity Framework related services
                    var descriptorsToRemove = services.Where(d => 
                        d.ServiceType == typeof(DbContextOptions<BooksDbContext>) ||
                        d.ServiceType == typeof(DbContextOptions) ||
                        d.ServiceType == typeof(BooksDbContext) ||
                        d.ServiceType.FullName?.Contains("EntityFramework") == true ||
                        d.ServiceType.FullName?.Contains("Microsoft.Data.Sqlite") == true ||
                        d.ServiceType.FullName?.Contains("Microsoft.EntityFrameworkCore.Sqlite") == true
                    ).ToList();

                    foreach (var descriptor in descriptorsToRemove)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database for testing
                    services.AddDbContext<BooksDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("InMemoryDbForTesting");
                    });

                    // Build the service provider and seed the database
                    var serviceProvider = services.BuildServiceProvider();
                    using var scope = serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
                    SeedDatabase(context);
                });
            });

            _client = _factory.CreateClient();
        }

        private static void SeedDatabase(BooksDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Seed test data
            var books = new List<Book>
            {
                new Book { Id = 1, Title = "Test Book 1", Author = "Author 1" },
                new Book { Id = 2, Title = "Test Book 2", Author = "Author 2" }
            };

            var users = new List<User>
            {
                new User { Id = 1, Username = "testuser1", Password = "password1" },
                new User { Id = 2, Username = "testuser2", Password = "password2" }
            };

            context.Books.AddRange(books);
            context.Users.AddRange(users);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetBooks_ShouldReturnAllBooks()
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
            Assert.Equal(2, books.Count);
        }

        [Fact]
        public async Task GetBookById_ShouldReturnSpecificBook()
        {
            // Act
            var response = await _client.GetAsync("/api/books/1");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<BookDto>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(book);
            Assert.Equal(1, book.Id);
            Assert.Equal("Test Book 1", book.Title);
        }

        [Fact]
        public async Task GetBookById_ShouldReturn404_WhenBookNotExists()
        {
            // Act
            var response = await _client.GetAsync("/api/books/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task RegisterUser_ShouldCreateNewUser()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Username = "newuser",
                Password = "newpassword"
            };

            var json = JsonSerializer.Serialize(createUserDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseContent = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(user);
            Assert.Equal("newuser", user.Username);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturn409_WhenUsernameExists()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Username = "testuser1", // This user already exists in seed data
                Password = "newpassword"
            };

            var json = JsonSerializer.Serialize(createUserDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users/register", content);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnToken_WhenValidCredentials()
        {
            // Arrange
            var loginDto = new CreateUserDto
            {
                Username = "testuser1",
                Password = "password1"
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users/login", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(loginResponse);
            Assert.Equal("testuser1", loginResponse.Username);
            Assert.NotNull(loginResponse.Token);
        }

        [Fact]
        public async Task LoginUser_ShouldReturn401_WhenInvalidCredentials()
        {
            // Arrange
            var loginDto = new CreateUserDto
            {
                Username = "testuser1",
                Password = "wrongpassword"
            };

            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users/login", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnAllUsers()
        {
            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<UserDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
        }

        [Fact]
        public async Task CreateBook_ShouldRequireAuthentication()
        {
            // Arrange
            var createBookDto = new CreateBookDto
            {
                Title = "New Book",
                Author = "New Author"
            };

            var json = JsonSerializer.Serialize(createBookDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/books", content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SearchBooks_ShouldReturnMatchingBooks()
        {
            // Act
            var response = await _client.GetAsync("/api/books/search?searchTerm=Test");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<BookDto>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(books);
            Assert.True(books.Count > 0);
            Assert.All(books, book => 
                Assert.True(book.Title.Contains("Test", StringComparison.OrdinalIgnoreCase) ||
                           book.Author.Contains("Test", StringComparison.OrdinalIgnoreCase)));
        }

        [Fact]
        public async Task ApiHealth_ShouldReturnSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/books");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}