using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using BookStore.Data;
using BookStore.Services;
using BookStore.Models;
using BookStore.Dtos;

namespace BooksApi.Tests
{
    public class UserServiceTests
    {
        private BooksDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BooksDbContext(options);
        }        private UserService CreateUserService(BooksDbContext context)
        {
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("your-very-long-secret-key-that-is-at-least-32-characters-long");
            mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");
            
            return new UserService(context, mockConfiguration.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldCreateNewUser_WhenValidData()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);
            var createUserDto = new CreateUserDto
            {
                Username = "testuser",
                Password = "testpassword"
            };

            // Act
            var result = await service.RegisterUserAsync(createUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            Assert.True(result.Id > 0);
            
            // Verify user was saved to database
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser");
            Assert.NotNull(savedUser);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldThrowException_WhenUsernameAlreadyExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);
            
            // Create existing user
            var existingUser = new User { Username = "existinguser", Password = "password" };
            await context.Users.AddAsync(existingUser);
            await context.SaveChangesAsync();

            var createUserDto = new CreateUserDto
            {
                Username = "existinguser",
                Password = "newpassword"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                service.RegisterUserAsync(createUserDto));
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnLoginResponse_WhenValidCredentials()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);
            
            var user = new User { Username = "testuser", Password = "testpassword" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await service.loginUserAsync("testuser", "testpassword");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            Assert.NotNull(result.Token);
        }

        [Fact]
        public async Task LoginUserAsync_ShouldReturnNull_WhenInvalidCredentials()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);

            // Act
            var result = await service.loginUserAsync("nonexistent", "wrongpassword");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);
            
            var user = new User { Username = "testuser", Password = "testpassword" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetUserByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);
            
            var users = new List<User>
            {
                new User { Username = "user1", Password = "password1" },
                new User { Username = "user2", Password = "password2" }
            };
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllUsersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUser_WhenValidData()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);
            
            var user = new User { Username = "oldusername", Password = "oldpassword" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var updateUserDto = new CreateUserDto
            {
                Username = "newusername",
                Password = "newpassword"
            };

            // Act
            var result = await service.UpdateUserAsync(user.Id, updateUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newusername", result.Username);
            
            // Verify user was updated in database
            var updatedUser = await context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("newusername", updatedUser.Username);
        }
    }
}