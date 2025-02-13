using EFCore.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;

        public BlogsController(ILogger<BlogsController> logger, ApplicationDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get() { 
            
         var result = await _dbContext.Blogs.ToListAsync();

         return Ok(result);        
        }


        [HttpPost]
        public async Task<IActionResult> Create(Blog emp)
        {
            var newBlog = new Blog
            {
                Url = emp.Url,
            };

            await _dbContext.Blogs.AddAsync(newBlog);

            await _dbContext.SaveChangesAsync();

            return Ok(newBlog);
        }
    }
}
