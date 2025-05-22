using AgroMonitor.Data;
using AgroMonitor.DTOs;
using AgroMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackageController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public PackageController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PackageDTO>> GetPackage(long id)
        {
            var package = await _db.Packages.Include(p => p.CustomerPackages).ThenInclude(cp => cp.Customer).FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                return NotFound("Package not found");
            }

            PackageDTO packageDTO = ToPackageDTO(package);

            return Ok(packageDTO);
        }

        [HttpGet]
        public async Task<ActionResult<List<PackageDTO>>> GetAllPackages()
        {
            var packages = await _db.Packages.Include(p => p.CustomerPackages).ThenInclude(cp => cp.Customer).Select(p => ToPackageDTO(p)).ToListAsync();

            return Ok(packages);
        }

        [HttpPost("single")]
        public async Task<ActionResult<PackageDTO>> CreatePackage([FromBody]CreatePackageDTO createPackage)
        {
            if(createPackage == null)
            {
                return BadRequest("The payload is missing");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var device = await _db.Devices.FindAsync(createPackage.DeviceId);

            if (device == null)
            {
                return NotFound($"Device with ID {createPackage.DeviceId} was not found");
            }

            Package package = new Package
            {
                Name = createPackage.Name,
                Description = createPackage.Description,
                Price = createPackage.Price,
                DeviceId = createPackage.DeviceId,
                CreatedAt = DateTime.UtcNow,
            };

            await _db.Packages.AddAsync(package);

            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPackage), new { id = package.Id }, ToPackageDTO(package));

        }
        [HttpPost("bulk")]
        public async Task<ActionResult<List<PackageDTO>>> CreatePackages([FromBody] List<CreatePackageDTO> createPackageList)
        {
            if (createPackageList == null || createPackageList.Count == 0)
            {
                return BadRequest("Payload is missing or empty.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deviceIds = createPackageList.Select(p => p.DeviceId).Distinct().ToList();

            var devices = await _db.Devices
                .Where(d => deviceIds.Contains(d.Id))
                .ToDictionaryAsync(d => d.Id);

            var invalidDeviceIds = deviceIds.Except(devices.Keys).ToList();
            if (invalidDeviceIds.Any())
            {
                return NotFound($"Devices not found for IDs: {string.Join(", ", invalidDeviceIds)}");
            }

            var packages = createPackageList.Select(p => new Package
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                DeviceId = p.DeviceId,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await _db.Packages.AddRangeAsync(packages);
            await _db.SaveChangesAsync();

            var result = packages.Select(ToPackageDTO).ToList();

            return Ok(result);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePackage(long id, [FromBody]UpdatePackageDTO updatePackage)
        {
            if(updatePackage == null)
            {
                return BadRequest("The payload is missing");
            }
            if(id != updatePackage.Id)
            {
                return BadRequest("The ID is not matching the ID of the payload");
            }

            var package = await _db.Packages.FindAsync(id);

            if(package == null)
            {
                return NotFound($"Package with ID {updatePackage.Id} was not found");
            }

            var device = await _db.Devices.FindAsync(updatePackage.DeviceId);

            if(device  == null)
            {
                return NotFound($"Device with ID {updatePackage.DeviceId} was not found");
            }

            package.Name = updatePackage.Name;
            package.Description = updatePackage.Description;
            package.Price = updatePackage.Price;
            package.DeviceId = updatePackage.DeviceId;
            package.LastUpdatedAt = DateTime.UtcNow;

            _db.Packages.Update(package);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePackage(long id)
        {
            var package = await _db.Packages.FirstOrDefaultAsync(p => p.Id == id);

            if (package == null)
            {
                return NotFound("Package not found");
            }

            _db.Packages.Remove(package);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private static PackageDTO ToPackageDTO(Package package)
        {
            return new PackageDTO
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Price = package.Price,
                CreatedAt = package.CreatedAt,
                DeviceId = package.DeviceId,
                LastUpdatedAt = package.LastUpdatedAt,
                Customers = package.CustomerPackages.Select(cp => new CustomerDTO
                {
                    Id = cp.CustomerId,
                    CustomerUniqueIdentifier = cp.Customer.CustomerUniqueIdentifier,
                    Name = cp.Customer.Name,
                    Email = cp.Customer.Email,
                    PhoneNumber = cp.Customer.PhoneNumber,
                    FarmLocation = cp.Customer.FarmLocation,
                    AreRentedDevicesReturned = cp.Customer.AreRentedDevicesReturned,
                    RegistrationDate = cp.Customer.RegistrationDate,
                    RentedDevicesReturnDate = cp.Customer.RentedDevicesReturnDate,
                    
                }).ToList(),
            };
        }
    }
}
