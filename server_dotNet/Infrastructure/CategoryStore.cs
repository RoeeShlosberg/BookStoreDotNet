using System.Collections.Generic;

namespace BookStore.Infrastructure
{
    public static class CategoryStore
    {
        public static readonly List<string> AllowedCategories = new List<string>
        {
            "Drama",
            "Sci-Fi",
            "Mystery",
            "Romance",
            "Fantasy",
            "Non-Fiction",
            "Biography",
            "History",
            "Horror",
            "Adventure"
        };
    }
}
