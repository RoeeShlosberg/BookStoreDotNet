namespace BookStore.Dtos
{
    /// <summary>
    /// Response data transfer object for a successful login attempt
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// JWT Bearer token to be used for authentication in subsequent API calls
        /// </summary>
        public string Token { get; set; } = string.Empty;
        
        /// <summary>
        /// The unique identifier of the authenticated user
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The username of the authenticated user
        /// </summary>
        public string Username { get; set; } = string.Empty;
    }
}
