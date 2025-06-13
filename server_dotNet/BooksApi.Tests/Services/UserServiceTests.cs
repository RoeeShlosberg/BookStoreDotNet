using BookStore.Data;
using BookStore.Dtos;
using BookStore.Models;
using BookStore.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BooksApi.Tests.Services
{
    public class UserServiceTests
    {
        private BooksDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<BooksDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new BooksDbContext(options);
        }

        private UserService CreateUserService(BooksDbContext context)
        {
            return new UserService(context);
        }

        [Fact]
        public async Task RegisterUserAsync_ValidUser_ShouldCreateAndReturnUser()
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

            // Verify it was saved to database
            var savedUser = await context.Users.FindAsync(result.Id);
            Assert.NotNull(savedUser);
            Assert.Equal("testuser", savedUser.Username);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUser_ShouldReturnUser()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);
            
            var user = new User { Id = 1, Username = "testuser", Password = "hashedpassword" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetUserByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.Username);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);
            
            var users = new List<User>
            {
                new User { Id = 1, Username = "user1", Password = "password1" },
                new User { Id = 2, Username = "user2", Password = "password2" }
            };
            
            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllUsersAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Username == "user1");
            Assert.Contains(result, u => u.Username == "user2");
        }

        [Fact]
        public async Task UpdateUserAsync_ExistingUser_ShouldUpdateAndReturnUser()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var service = CreateUserService(context);
            
            var user = new User { Id = 1, Username = "oldusername", Password = "oldpassword" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            
            var updateUserDto = new CreateUserDto
            {
                Username = "newusername",
                Password = "newpassword"
            };

            // Act
            var result = await service.UpdateUserAsync(1, updateUserDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newusername", result.Username);

            // Verify it was updated in database
            var updatedUser = await context.Users.FindAsync(1);
            Assert.NotNull(updatedUser);
            Assert.Equal("newusername", updatedUser.Username);
        }
    }
}
