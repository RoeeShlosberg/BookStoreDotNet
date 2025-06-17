// Controller for managing books in the library system
// This will handle CRUD operations for books
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BookStore.Dtos;
using BookStore.Data;
using BookStore.Services;
using System.Security.Claims;

namespace BookStore.Controllers
{
    /// <summary>
    /// Controller for managing books in the library system
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;


        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }

        /// <summary>
        /// Get all books
        /// </summary>
        /// <returns>A list of all books in the library</returns>
        /// <response code="200">Returns the list of books</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBooks()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();
            var books = await _bookService.GetAllBooksForUserAsync(userId);
            return Ok(books);
        }

        /// <summary>
        /// Get a specific book by ID
        /// </summary>
        /// <param name="id">The ID of the book to retrieve</param>
        /// <returns>The requested book</returns>
        /// <response code="200">Returns the requested book</response>
        /// <response code="404">If the book is not found</response>
        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
                return NotFound();
            return Ok(book);
        }

        /// <summary>
        /// Create a new book (requires authentication)
        /// </summary>
        /// <param name="book">The book details</param>
        /// <returns>The newly created book</returns>
        /// <response code="201">Returns the newly created book</response>
        /// <response code="400">If the book data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateBook([FromBody] CreateBookDto book)
        {
            if (book == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            // Extract user ID from token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            try
            {
                var createdBook = await _bookService.CreateBookForUserAsync(book, userId);
                return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing book (requires authentication)
        /// </summary>
        /// <param name="id">The ID of the book to update</param>
        /// <param name="book">The updated book details</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the book was successfully updated</response>
        /// <response code="400">If the book data is invalid</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the book is not found</response>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookDto book)
        {
            if (book == null || !ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updated = await _bookService.UpdateBookAsync(id, book);
                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a book (requires authentication)
        /// </summary>
        /// <param name="id">The ID of the book to delete</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">If the book was successfully deleted</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the book is not found</response>
        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var deleted = await _bookService.DeleteBookAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Search for books by title or author
        /// </summary>
        /// <param name="searchTerm">The term to search for in book titles and author names</param>
        /// <returns>A list of books matching the search criteria</returns>
        /// <response code="200">Returns the matching books</response>
        /// <response code="400">If the search term is empty</response>
        [Authorize]
        [HttpGet("search")]
        public async Task<IActionResult> SearchBooks([FromQuery] string searchTerm)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();
            var books = await _bookService.SearchBooksForUserAsync(searchTerm, userId);
            return Ok(books);
        }
    }
}