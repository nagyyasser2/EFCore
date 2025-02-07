using EFCore.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _dbContext;

        public EmployeesController(ILogger<EmployeesController> logger, ApplicationDbContext dbContext)
        {
            this._logger = logger;
            this._dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Get() { 
            
         var employess = await _dbContext.Employees.ToListAsync();

         return Ok(employess);        
        }


        [HttpPost]
        public async Task<IActionResult> Create(Employee emp)
        {
            var newEmployee = new Employee
            {
                Name = emp.Name,
            };

            await _dbContext.Employees.AddAsync(newEmployee);

            await _dbContext.SaveChangesAsync();

            return Ok(newEmployee);
        }
    }
}
