namespace BookStore.Dtos
{
    /// <summary>
    /// Data transfer object for book information including ID
    /// </summary>
    public class BookDto
    {
        /// <summary>
        /// The unique identifier for the book
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The title of the book
        /// </summary>
        public string Title { get; set; } = "";
        
        /// <summary>
        /// The author of the book
        /// </summary>
        public string Author { get; set; } = "";
        
        /// <summary>
        /// The date when the book was published
        /// </summary>
        public DateTime? PublishedDate { get; set; }
    }
}

