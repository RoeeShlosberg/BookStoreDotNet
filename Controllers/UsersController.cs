// Controller for managing user-related operations
using BookStore.Dtos;
using BookStore.Models;
using BookStore.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    /// <summary>
    /// Controller for managing user-related operations including registration, login, and user management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="createUserDto">User registration information</param>
        /// <returns>The newly created user information</returns>
        /// <response code="201">Returns the newly created user</response>
        /// <response code="400">If the user data is invalid</response>
        /// <response code="409">If the username already exists</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null || !ModelState.IsValid)
                return BadRequest("Invalid user data.");

            try
            {
                var user = await _userService.RegisterUserAsync(createUserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // Username already exists
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); // Invalid user data
            }
        }

        /// <summary>
        /// Authenticate a user and get a JWT token
        /// </summary>
        /// <param name="loginDto">Login credentials</param>
        /// <returns>User information and authentication token</returns>
        /// <response code="200">Returns the user info and JWT token</response>
        /// <response code="400">If the credentials are invalid</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> LoginUser([FromBody] CreateUserDto loginDto)
        {
            if (loginDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.loginUserAsync(loginDto.Username, loginDto.Password);
            if (user == null)
                return BadRequest("Invalid username or password.");

            return Ok(user);
        }

        /// <summary>
        /// Get a user by their ID
        /// </summary>
        /// <param name="id">The ID of the user to retrieve</param>
        /// <returns>The user information</returns>
        /// <response code="200">Returns the user</response>
        /// <response code="404">If the user is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Get all users
        /// </summary>
        /// <returns>A list of all users</returns>
        /// <response code="200">Returns the list of users</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Update an existing user
        /// </summary>
        /// <param name="id">The ID of the user to update</param>
        /// <param name="updateUserDto">The updated user details</param>
        /// <returns>The updated user information</returns>
        /// <response code="200">Returns the updated user</response>
        /// <response code="400">If the user data is invalid</response>
        /// <response code="404">If the user is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] CreateUserDto updateUserDto)
        {
            if (updateUserDto == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);
            if (updatedUser == null)
                return NotFound();

            return Ok(updatedUser);
        }       
    }
}