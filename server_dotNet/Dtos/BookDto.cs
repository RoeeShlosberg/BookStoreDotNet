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
        /// The date when the book was uploaded
        /// </summary>
        public DateTime? UploadDate { get; set; }
        
        /// <summary>
        /// The rank of the book, ranging from 1 to 10
        /// summary>
        public int Rank { get; set; }
    }
}

