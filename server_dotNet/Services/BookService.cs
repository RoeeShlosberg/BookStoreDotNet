using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using BookStore.Dtos;
using BookStore.Models;
using BookStore.Controllers; // For CategoriesController.AllowedCategories
using BookStore.Infrastructure;

namespace BookStore.Services
{
    public class BookService : IBookService
    {
        private readonly BooksDbContext _context;

        public BookService(BooksDbContext context)
        {
            _context = context;
        }

        public async Task<List<BookDto>> GetAllBooksAsync()
        {
            return await _context.Books
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    UploadDate = b.UploadDate,
                    Rank = b.Rank,
                    Categories = b.Categories
                })
                .ToListAsync();
        }

        public async Task<BookDto?> GetBookByIdAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return null;

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                UploadDate = book.UploadDate,
                Rank = book.Rank,
                Categories = book.Categories
            };
        }        public async Task<BookDto> CreateBookAsync(CreateBookDto dto)
        {
            // Validate categories
            var allowedCategories = CategoryStore.AllowedCategories;
            if (dto.Categories == null || !dto.Categories.Any())
            {
                throw new ArgumentException("Books must have at least one category.");
            }
            if (dto.Categories.Count > 3)
            {
                throw new ArgumentException("Books can have at most 3 categories.");
            }
            if (dto.Categories.Any(c => !allowedCategories.Contains(c)))
            {
                throw new ArgumentException("One or more categories are invalid.");
            }
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                UploadDate = dto.UploadDate ?? DateTime.UtcNow,
                Rank = dto.Rank,
                Categories = dto.Categories ?? new List<string>()
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                UploadDate = book.UploadDate,
                Rank = book.Rank,
                Categories = book.Categories
            };
        }        public async Task<bool> UpdateBookAsync(int id, UpdateBookDto dto)
        {
            // Validate categories
            var allowedCategories = CategoryStore.AllowedCategories;
            if (dto.Categories == null || !dto.Categories.Any())
            {
                throw new ArgumentException("Books must have at least one category.");
            }
            if (dto.Categories.Count > 3)
            {
                throw new ArgumentException("Books can have at most 3 categories.");
            }
            if (dto.Categories.Any(c => !allowedCategories.Contains(c)))
            {
                throw new ArgumentException("One or more categories are invalid.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.UploadDate = dto.UploadDate ?? book.UploadDate;
            book.Rank = dto.Rank;
            book.Categories = dto.Categories ?? new List<string>();

            _context.Books.Update(book);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            // Remove all BookUser associations for this book
            var bookUsers = _context.BookUsers.Where(bu => bu.BookId == id);
            _context.BookUsers.RemoveRange(bookUsers);

            _context.Books.Remove(book);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<BookDto>> SearchBooksAsync(string searchTerm)
        {
            return await _context.Books
                .Where(b => b.Title.Contains(searchTerm) || b.Author.Contains(searchTerm))
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    UploadDate = b.UploadDate,
                    Rank = b.Rank,
                    Categories = b.Categories
                })
                .ToListAsync();
        }        public async Task<BookDto> CreateBookForUserAsync(CreateBookDto dto, int userId)
        {
            // Validate categories
            var allowedCategories = CategoryStore.AllowedCategories;
            if (dto.Categories == null || !dto.Categories.Any())
            {
                throw new ArgumentException("Books must have at least one category.");
            }
            if (dto.Categories.Count > 3)
            {
                throw new ArgumentException("Books can have at most 3 categories.");
            }
            if (dto.Categories.Any(c => !allowedCategories.Contains(c)))
            {
                throw new ArgumentException("One or more categories are invalid.");
            }
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                UploadDate = dto.UploadDate ?? DateTime.UtcNow,
                Rank = dto.Rank,
                Categories = dto.Categories ?? new List<string>()
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var bookUser = new BookUser
            {
                UserId = userId,
                BookId = book.Id
            };
            _context.BookUsers.Add(bookUser);
            await _context.SaveChangesAsync();

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                UploadDate = book.UploadDate,
                Rank = book.Rank,
                Categories = book.Categories
            };
        }

        public async Task<List<BookDto>> GetAllBooksForUserAsync(int userId)
        {
            return await _context.BookUsers
                .Where(bu => bu.UserId == userId)
                .Select(bu => new BookDto
                {
                    Id = bu.Book.Id,
                    Title = bu.Book.Title,
                    Author = bu.Book.Author,
                    UploadDate = bu.Book.UploadDate,
                    Rank = bu.Book.Rank,
                    Categories = bu.Book.Categories
                })
                .ToListAsync();
        }

        public async Task<List<BookDto>> SearchBooksForUserAsync(string searchTerm, int userId)
        {
            return await _context.BookUsers
                .Where(bu => bu.UserId == userId && (bu.Book.Title.Contains(searchTerm) || bu.Book.Author.Contains(searchTerm)))
                .Select(bu => new BookDto
                {
                    Id = bu.Book.Id,
                    Title = bu.Book.Title,
                    Author = bu.Book.Author,
                    UploadDate = bu.Book.UploadDate,
                    Rank = bu.Book.Rank,
                    Categories = bu.Book.Categories
                })
                .ToListAsync();
        }
    }
}
