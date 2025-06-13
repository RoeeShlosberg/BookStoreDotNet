using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using BookStore.Controllers;
using BookStore.Services;
using BookStore.Dtos;

namespace BooksApi.Tests
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
        public async Task RegisterUser_ShouldReturnCreatedResult_WhenValidData()
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
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedUser = Assert.IsType<UserDto>(createdResult.Value);
            Assert.Equal("testuser", returnedUser.Username);
        }

        [Fact]
        public async Task LoginUser_ShouldReturnOkResult_WhenValidCredentials()
        {
            // Arrange
            var loginDto = new CreateUserDto
            {
                Username = "testuser",
                Password = "testpassword"
            };

            var loginResponse = new LoginResponseDto
            {
                Username = "testuser",
                Token = "fake-jwt-token"
            };

            _mockUserService.Setup(x => x.loginUserAsync(loginDto.Username, loginDto.Password))
                          .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.LoginUser(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginResponseDto>(okResult.Value);
            Assert.Equal("testuser", response.Username);
            Assert.Equal("fake-jwt-token", response.Token);
        }        [Fact]
        public async Task LoginUser_ShouldReturnUnauthorized_WhenInvalidCredentials()
        {
            // Arrange
            var loginDto = new CreateUserDto
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            _mockUserService.Setup(x => x.loginUserAsync(loginDto.Username, loginDto.Password))
                          .ReturnsAsync((LoginResponseDto?)null);

            // Act
            var result = await _controller.LoginUser(loginDto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOkResult_WhenUserExists()
        {
            // Arrange
            var user = new UserDto
            {
                Id = 1,
                Username = "testuser",
                Password = "testpassword"
            };

            _mockUserService.Setup(x => x.GetUserByIdAsync(1)).ReturnsAsync(user);

            // Act
            var result = await _controller.GetUserById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("testuser", returnedUser.Username);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOkResult_WithUsersList()
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

        [Fact]
        public async Task UpdateUser_ShouldReturnOkResult_WhenValidData()
        {
            // Arrange
            var updateUserDto = new CreateUserDto
            {
                Username = "updateduser",
                Password = "updatedpassword"
            };

            var updatedUser = new UserDto
            {
                Id = 1,
                Username = "updateduser",
                Password = "updatedpassword"
            };

            _mockUserService.Setup(x => x.UpdateUserAsync(1, updateUserDto)).ReturnsAsync(updatedUser);

            // Act
            var result = await _controller.UpdateUser(1, updateUserDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal("updateduser", returnedUser.Username);
        }

        [Fact]
        public async Task RegisterUser_ShouldReturnBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Username = "",
                Password = "testpassword"
            };

            _controller.ModelState.AddModelError("Username", "Username is required");

            // Act
            var result = await _controller.RegisterUser(createUserDto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}