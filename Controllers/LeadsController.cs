using LeadImagesDemo.Data;
using LeadImagesDemo.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeadImagesDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        private readonly AppDb _db;
        public LeadsController(AppDb db) => _db = db;

        [HttpPost]
        public async Task<ActionResult<Lead>> Create(Lead l)
        {
            _db.Leads.Add(l);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = l.Id }, l);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Lead>> Get(int id)
        {
            var entity = await _db.Leads.FindAsync(id);
            return entity is null ? NotFound() : entity;
        }
    }
}
