using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class SharedBookList
    {
        [Key]
        public string Id { get; set; } // Unique, unguessable string (GUID or similar)

        public List<int> BookIds { get; set; } = new List<int>(); // List of Book IDs
    }
}
