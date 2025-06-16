using BookStore.Data;
using BookStore.Dtos;


public interface IBookService
{
    Task<List<BookDto>> GetAllBooksAsync();
    Task<BookDto?> GetBookByIdAsync(int id);
    Task<BookDto> CreateBookAsync(CreateBookDto bookDto);
    Task<BookDto> CreateBookForUserAsync(CreateBookDto bookDto, int userId);
    Task<bool> UpdateBookAsync(int id, UpdateBookDto bookDto);
    Task<bool> DeleteBookAsync(int id);
    Task<List<BookDto>> SearchBooksAsync(string searchTerm);
    Task<List<BookDto>> GetAllBooksForUserAsync(int userId);
    Task<List<BookDto>> SearchBooksForUserAsync(string searchTerm, int userId);
}
