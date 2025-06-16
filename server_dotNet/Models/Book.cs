// Create model for Book
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BookStore.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string Author { get; set; }

        // Uploaded Date - default to current date
        [DataType(DataType.Date)]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // Rank - 1-10
        [Range(1, 10)]
        [Required]
        public int Rank { get; set; }

        [Required]
        public List<string> Categories { get; set; } = new List<string>();
    }

    public class BookUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}