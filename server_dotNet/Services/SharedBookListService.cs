using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Models;

namespace BookStore.Services
{
    public class SharedBookListService
    {
        private readonly BooksDbContext _context;
        public SharedBookListService(BooksDbContext context)
        {
            _context = context;
        }

        public async Task<SharedBookList> CreateSharedListAsync(List<int> bookIds)
        {
            for (int i = 0; i < bookIds.Count; i++)
            {
                // Validate each book ID
                var book = await _context.Books.FindAsync(bookIds[i]);
                if (book == null)
                {
                    throw new ArgumentException($"Book with ID {bookIds[i]} does not exist.");
                }
            }
            var sharedList = new SharedBookList
            {
                Id = Guid.NewGuid().ToString(),
                BookIds = bookIds
            };
            _context.SharedBookLists.Add(sharedList);
            await _context.SaveChangesAsync();
            return sharedList;
        }

        public async Task<List<Book>> GetBooksForSharedListAsync(string sharedListId)
        {
            var sharedList = await _context.SharedBookLists.FindAsync(sharedListId);
            if (sharedList == null) throw new ArgumentException($"Shared list with ID {sharedListId} does not exist.");
            var books = await _context.Books
                .AsNoTracking()
                .Where(b => sharedList.BookIds.Contains(b.Id))
                .ToListAsync();
            return books;
        }
    }
}
