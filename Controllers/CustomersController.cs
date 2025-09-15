using LeadImagesDemo.Data;
using LeadImagesDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeadImagesDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly AppDb _db;
        public CustomersController(AppDb db) => _db = db;

        [HttpPost]
        public async Task<ActionResult<Customer>> Create(Customer c)
        {
            _db.Customers.Add(c);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = c.Id }, c);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Customer>> Get(int id)
        {
            var entity = await _db.Customers.FindAsync(id);
            return entity is null ? NotFound() : entity;
        }
    }
}
