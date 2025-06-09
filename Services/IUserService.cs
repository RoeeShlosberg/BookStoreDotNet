using BookStore.Dtos;
using BookStore.Models;

namespace BookStore.Services
{
    public interface IUserService
    {
        // Register a new user
        Task<UserDto> RegisterUserAsync(CreateUserDto createUserDto);

        // Authenticate a user
        Task<LoginResponseDto?> loginUserAsync(string username, string password);

        // Get user by ID
        Task<UserDto> GetUserByIdAsync(int id);

        // Get all users
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        // Update user details
        Task<UserDto> UpdateUserAsync(int id, CreateUserDto updateUserDto);
    }
}
