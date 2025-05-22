using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMonitor.Data;
using AgroMonitor.Models;
using AgroMonitor.DTOs;

namespace AgroMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageForDisplaysController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public PackageForDisplaysController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<PackageForDisplayDTO>>> GetPackagesForDisplay()
        {
            var packagesForDisplay = await _db.PackagesForDisplay.Include(p => p.DeviceForDisplay).Select(p => ToDTO(p)).ToListAsync();

            return Ok(packagesForDisplay);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PackageForDisplayDTO>> GetPackageForDisplay(int id)
        {
            var packageForDisplay = await _db.PackagesForDisplay.Include(p => p.DeviceForDisplay).FirstOrDefaultAsync(p => p.Id == id);

            if (packageForDisplay == null)
            {
                return NotFound("Package for display not found");
            }

            return Ok(ToDTO(packageForDisplay));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackageForDisplay(int id,[FromBody] UpdatePackageForDisplayDTO updatePackageForDisplay)
        {
            if(updatePackageForDisplay == null)
            {
                return BadRequest("Payload is missing");
            }

            if (id != updatePackageForDisplay.Id)
            {
                return BadRequest("The ID in the route does not match the ID in the payload.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await _db.PackagesForDisplay.AnyAsync(p => p.Id != id && p.Name.ToUpper() == updatePackageForDisplay.Name.ToUpper()))
            {
                return BadRequest("A package for display with this exact name already exists");
            }

            var packageForDisplay = await _db.PackagesForDisplay.FirstOrDefaultAsync(p => p.Id == id);

            if (packageForDisplay == null)
            {
                return NotFound("Package for display not found");
            }

            var newDeviceForDisplay = await _db.DevicesForDisplay.FindAsync(updatePackageForDisplay.DeviceForDisplayId);

            if (newDeviceForDisplay == null)
            {
                return NotFound("Device for display not found");
            }

            packageForDisplay.Name = updatePackageForDisplay.Name;
            packageForDisplay.Description = updatePackageForDisplay.Description;
            packageForDisplay.Price = updatePackageForDisplay.Price;
            packageForDisplay.DeviceForDisplayId = newDeviceForDisplay.Id;
            packageForDisplay.LastUpdatedAt = DateTime.UtcNow;

            _db.PackagesForDisplay.Update(packageForDisplay);

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<PackageForDisplayDTO>> CreatePackageForDisplay([FromBody]CreatePackageForDisplayDTO createPackageForDisplay)
        {
            if(createPackageForDisplay == null)
            {
                return BadRequest("Payload is missing");
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (await _db.PackagesForDisplay.AnyAsync(p => p.Name.ToUpper() == createPackageForDisplay.Name.ToUpper()))
            {
                return BadRequest("A package for display with this exact name already exists");
            }

            var deviceForDisplay = await _db.DevicesForDisplay.FindAsync(createPackageForDisplay.DeviceForDisplayId);

            if(deviceForDisplay == null)
            {
                return NotFound("Device for display not found");
            }

            PackageForDisplay packageForDisplay = new PackageForDisplay
            {
                Name = createPackageForDisplay.Name,
                Description = createPackageForDisplay.Description,
                Price = createPackageForDisplay.Price,
                DeviceForDisplayId = createPackageForDisplay.DeviceForDisplayId,
                CreatedAt = DateTime.UtcNow,
            };

            await _db.PackagesForDisplay.AddAsync(packageForDisplay);
            await _db.SaveChangesAsync();

            return CreatedAtAction("GetPackageForDisplay", new { id = packageForDisplay.Id }, ToDTO(packageForDisplay));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackageForDisplay(int id)
        {
            var packageForDisplay = await _db.PackagesForDisplay.FindAsync(id);
            if (packageForDisplay == null)
            {
                return NotFound("Package for display not found");
            }

            _db.PackagesForDisplay.Remove(packageForDisplay);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        private static PackageForDisplayDTO ToDTO(PackageForDisplay packageForDisplay)
        {
            return new PackageForDisplayDTO
            {
                Id = packageForDisplay.Id,
                Name = packageForDisplay.Name,
                Description = packageForDisplay.Description,
                Price = packageForDisplay.Price,
                DeviceForDisplay = new DeviceForDisplayDTO
                {
                    Id = packageForDisplay.DeviceForDisplay.Id,
                    BrandCode = packageForDisplay.DeviceForDisplay.BrandCode,
                    Name = packageForDisplay.DeviceForDisplay.Name,
                },
                CreatedAt = packageForDisplay.CreatedAt,
                LastUpdatedAt = packageForDisplay.LastUpdatedAt,
            };
        }
    }
}
