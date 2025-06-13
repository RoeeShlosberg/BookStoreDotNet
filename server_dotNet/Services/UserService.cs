// User Service
using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.Dtos;
using BookStore.Models;
using Microsoft.Extensions.Configuration; // Added for IConfiguration
using System.IdentityModel.Tokens.Jwt; // Added for JwtSecurityTokenHandler
using System.Security.Claims; // Added for Claims
using System.Text; // Added for Encoding
using Microsoft.IdentityModel.Tokens; // Added for SymmetricSecurityKey


namespace BookStore.Services
{
    public class UserService : IUserService
    {
        private readonly BooksDbContext _context;
        private readonly IConfiguration _configuration; // Added IConfiguration

        public UserService(BooksDbContext context, IConfiguration configuration) // Updated constructor
        {
            _context = context;
            _configuration = configuration; // Store IConfiguration
        }
        
        public async Task<UserDto> RegisterUserAsync(CreateUserDto createUserDto)
        {
            // Validate the input
            if (createUserDto == null || string.IsNullOrEmpty(createUserDto.Username) || string.IsNullOrEmpty(createUserDto.Password))
            {
                throw new ArgumentException("Invalid user data.");
            }
            // Check if the username already exists
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == createUserDto.Username);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Username already exists.");
            }
            var user = new User
            {
                Username = createUserDto.Username,
                Password = createUserDto.Password, // In a real application, ensure to hash the password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
            };
        }

        public async Task<LoginResponseDto?> loginUserAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user == null) return null;

            await _context.SaveChangesAsync();

            // Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                    // Can add more claims (e.g "Admin?") if needed
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiration
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new LoginResponseDto
            {
                Token = tokenString,
                Id = user.Id,
                Username = user.Username,
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Password = u.Password,
                })
                .ToListAsync();
        }

        public async Task<UserDto> UpdateUserAsync(int id, CreateUserDto updateUserDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            user.Username = updateUserDto.Username;
            user.Password = updateUserDto.Password; // In a real application, ensure to hash the password

            await _context.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
            };
        }
    }
}

