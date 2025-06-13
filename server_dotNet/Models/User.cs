// USER Model
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookStore.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        // Unique index for Username should be configured in DbContext using Fluent API
        public string Username { get; set; }
        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)] // Ensures the password is treated as sensitive data
        public string Password { get; set; }
    }
}