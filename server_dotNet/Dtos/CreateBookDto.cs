namespace BookStore.Dtos
{
    /// <summary>
    /// Data transfer object for creating a new book
    /// </summary>
    public class CreateBookDto
    {
        /// <summary>
        /// The title of the book
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The author of the book
        /// </summary>
        public string Author { get; set; } = string.Empty;

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
