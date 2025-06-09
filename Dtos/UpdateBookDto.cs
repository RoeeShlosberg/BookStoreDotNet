namespace BookStore.Dtos
{
    /// <summary>
    /// Data transfer object for updating an existing book
    /// </summary>
    public class UpdateBookDto
    {
        /// <summary>
        /// The updated title of the book
        /// </summary>
        public string Title { get; set; } = string.Empty;
        
        /// <summary>
        /// The updated author of the book
        /// </summary>
        public string Author { get; set; } = string.Empty;
        
        /// <summary>
        /// The updated publication date of the book
        /// </summary>
        public DateTime? PublishedDate { get; set; }
    }
}
