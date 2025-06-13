using BookStore.Controllers;
using BookStore.Dtos;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace BooksApi.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UsersController(_mockUserService.Object);
        }

        [Fact]
        public async Task RegisterUser_ValidUser_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Username = "testuser",
                Password = "testpassword"
            };
            
            var createdUser = new UserDto
            {
                Id = 1,
                Username = "testuser",
                Password = "testpassword"
            };
            
            _mockUserService.Setup(x => x.RegisterUserAsync(createUserDto)).ReturnsAsync(createdUser);

            // Act
            var result = await _controller.RegisterUser(createUserDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedUser = Assert.IsType<UserDto>(createdAtActionResult.Value);
            Assert.Equal("testuser", returnedUser.Username);
        }

        [Fact]
        public async Task LoginUser_ValidCredentials_ShouldReturnOkWithToken()
        {
            // Arrange
            var loginDto = new CreateUserDto
            {
                Username = "testuser",
                Password = "testpassword"
            };
            
            var loginResponse = new LoginResponseDto
            {
                Id = 1,
                Username = "testuser",
                Token = "fake-jwt-token"
            };
            
            _mockUserService.Setup(x => x.loginUserAsync("testuser", "testpassword")).ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.LoginUser(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedResponse = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.Equal("testuser", returnedResponse.Username);
            Assert.Equal("fake-jwt-token", returnedResponse.Token);
        }

        [Fact]
        public async Task LoginUser_InvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            var loginDto = new CreateUserDto
            {
                Username = "testuser",
                Password = "wrongpassword"
            };
            
            _mockUserService.Setup(x => x.loginUserAsync("testuser", "wrongpassword")).ReturnsAsync((LoginResponseDto?)null);

            // Act
            var result = await _controller.LoginUser(loginDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task GetUserById_ExistingUser_ShouldReturnOkWithUser()
        {
            // Arrange
            var user = new UserDto { Id = 1, Username = "testuser", Password = "testpassword" };
            _mockUserService.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("testuser", returnedUser.Username);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOkWithUsers()
        {
            // Arrange
            var users = new List<UserDto>
            {
                new UserDto { Id = 1, Username = "user1", Password = "password1" },
                new UserDto { Id = 2, Username = "user2", Password = "password2" }
            };
            
            _mockUserService.Setup(x => x.GetAllUsersAsync()).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
            Assert.Equal(2, returnedUsers.Count());
        }
    }
}
