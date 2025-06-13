namespace BookStore.Dtos
{
    /// <summary>
    /// Data transfer object for user information
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// The unique identifier for the user
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The username of the user
        /// </summary>
        public string Username { get; set; } = "";
        
        /// <summary>
        /// The password of the user (Note: In a real application, passwords should be hashed and not returned in DTOs)
        /// </summary>
        public string Password { get; set; } = ""; // Note: In a real application, passwords should be hashed and not stored in plain text.
    }

    /// <summary>
    /// Data transfer object for creating or authenticating a user
    /// </summary>
    public class CreateUserDto
    {
        /// <summary>
        /// The username for the new user or login
        /// </summary>
        public string Username { get; set; } = "";
        
        /// <summary>
        /// The password for the new user or login
        /// </summary>
        public string Password { get; set; } = ""; // Note: In a real application, passwords should be hashed and not stored in plain text.
    }
}