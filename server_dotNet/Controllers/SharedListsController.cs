using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Services;
using BookStore.Models;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SharedListsController : ControllerBase
    {
        private readonly SharedBookListService _service;
        public SharedListsController(SharedBookListService service)
        {
            _service = service;
        }

        // POST: api/SharedLists
        [HttpPost]
        public async Task<ActionResult<string>> CreateSharedList([FromBody] List<int> bookIds)
        {
            var sharedList = await _service.CreateSharedListAsync(bookIds);
            return Ok(sharedList.Id);
        }

        // GET: api/SharedLists/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<List<Book>>> GetSharedList(string id)
        {
            var books = await _service.GetBooksForSharedListAsync(id);
            if (books == null) return NotFound();
            return Ok(books);
        }
    }
}
