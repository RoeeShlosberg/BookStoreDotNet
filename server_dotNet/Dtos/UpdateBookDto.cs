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
        /// The updated upload date of the book
        /// </summary>
        public DateTime? UploadDate { get; set; }
        
        /// <summary>
        /// The updated rank of the book, ranging from 1 to 10
        /// </summary>
        public int Rank { get; set; }
    }
}
