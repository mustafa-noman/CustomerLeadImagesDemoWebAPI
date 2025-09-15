using LeadImagesDemo.Data;
using LeadImagesDemo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeadImagesDemo.Controllers
{
    [ApiController]
    [Route("api")]
    public class ImagesController : ControllerBase
    {
        private readonly AppDb _db;
        public ImagesController(AppDb db) => _db = db;

        [HttpPost("{ownerType}/{ownerId:int}/images")]
        public async Task<IActionResult> Upload(string ownerType, int ownerId, [FromForm] IFormFileCollection? files)
        {
            if (!Enum.TryParse<OwnerType>(ownerType, true, out var type))
                return BadRequest("ownerType must be 'customer' or 'lead'.");
            var ownerExists = type == OwnerType.Customer
                ? await _db.Customers.AnyAsync(x => x.Id == ownerId)
                : await _db.Leads.AnyAsync(x => x.Id == ownerId);
            if (!ownerExists) return NotFound("Owner not found.");
            if (files is null || files.Count == 0) return BadRequest("No files provided.");

            var used = await _db.Images
                .Where(i => i.OwnerType == type && i.OwnerId == ownerId)
                .Select(i => i.SequenceNo)
                .ToListAsync();
            var freeSlots = Enumerable.Range(1, 10).Except(used).ToList();
            if (freeSlots.Count == 0) return BadRequest("Limit reached (10 images).");

            var toAdd = new List<ImageRecord>();
            foreach (var file in files)
            {
                if (freeSlots.Count == 0) break;
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var base64 = Convert.ToBase64String(ms.ToArray());
                var slot = freeSlots[0];
                freeSlots.RemoveAt(0);
                toAdd.Add(new ImageRecord { OwnerType = type, OwnerId = ownerId, SequenceNo = slot, Base64Data = base64 });
            }
            _db.Images.AddRange(toAdd);
            try { await _db.SaveChangesAsync(); }
            catch (DbUpdateException ex) { return Conflict($"Save failed: {ex.Message}"); }

            return Ok(new { added = toAdd.Count, remainingSlots = freeSlots.Count });
        }

        [HttpGet("{ownerType}/{ownerId:int}/images")]
        public async Task<IActionResult> List(string ownerType, int ownerId)
        {
            if (!Enum.TryParse<OwnerType>(ownerType, true, out var type))
                return BadRequest("ownerType must be 'customer' or 'lead'.");
            var items = await _db.Images
                .Where(i => i.OwnerType == type && i.OwnerId == ownerId)
                .OrderBy(i => i.SequenceNo)
                .Select(i => new ImageDto(i.Id, i.SequenceNo, i.CreatedUtc, i.Base64Data))
                .ToListAsync();
            return Ok(items);
        }

        [HttpDelete("images/{imageId:int}")]
        public async Task<IActionResult> Delete(int imageId)
        {
            var img = await _db.Images.FindAsync(imageId);
            if (img is null) return NotFound();
            _db.Images.Remove(img);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
