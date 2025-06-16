using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using BookStore.Infrastructure;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private static readonly List<string> AllowedCategories = CategoryStore.AllowedCategories;

        [HttpGet]
        public ActionResult<List<string>> GetCategories()
        {
            return Ok(AllowedCategories);
        }
    }
}
