// Create model for Book
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        //  PublishedDate - optional, if not provided, defaults to current date
        [DataType(DataType.Date)]
        public DateTime? PublishedDate { get; set; } = DateTime.Now;
    }
}