using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgroMonitor.Data;
using AgroMonitor.Models;
using AgroMonitor.DTOs;

namespace AgroMonitor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceForDisplaysController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public DeviceForDisplaysController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<DeviceForDisplayDTO>>> GetDevicesForDisplay()
        {
            var devicesForDisplay = await _db.DevicesForDisplay.Select(d => ToDTO(d)).ToListAsync();

            return Ok(devicesForDisplay);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceForDisplayDTO>> GetDeviceForDisplay(int id)
        {
            var deviceForDisplay = await _db.DevicesForDisplay.FindAsync(id);

            if (deviceForDisplay == null)
            {
                return NotFound("Device for display not found");
            }

            return Ok(ToDTO(deviceForDisplay));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeviceForDisplay(int id,[FromBody] UpdateDeviceForDisplayDTO updateDeviceForDisplay)
        {
            if(updateDeviceForDisplay == null)
            {
                return BadRequest("Payload is missing");
            }

            if (id != updateDeviceForDisplay.Id)
            {
                return BadRequest("The ID in the route does not match the ID in the payload.");
            }
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var deviceForDisplay = await _db.DevicesForDisplay.FindAsync(id);

            if(deviceForDisplay == null)
            {
                return NotFound("Device for display not found");
            }

            if (await _db.DevicesForDisplay.AnyAsync(d => d.Id != id && d.BrandCode.ToUpper() == updateDeviceForDisplay.BrandCode.ToUpper()))
            {
                return BadRequest("Another device already uses this BrandCode.");
            }

            deviceForDisplay.Name = updateDeviceForDisplay.Name;
            deviceForDisplay.BrandCode = updateDeviceForDisplay.BrandCode;
            deviceForDisplay.LastUpdatedAt = DateTime.UtcNow;

            _db.DevicesForDisplay.Update(deviceForDisplay);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<DeviceForDisplayDTO>> PostDeviceForDisplay([FromBody]AddDeviceForDisplayDTO addDeviceForDisplay)
        {
            if(addDeviceForDisplay == null)
            {
                return BadRequest("Payload is missing");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(await _db.DevicesForDisplay.AnyAsync(d => d.BrandCode.ToUpper() == addDeviceForDisplay.BrandCode.ToUpper()))
            {
                return BadRequest("Another device already uses this BrandCode.");
            }

            DeviceForDisplay deviceForDisplay = new DeviceForDisplay
            {
                Name = addDeviceForDisplay.Name,
                BrandCode = addDeviceForDisplay.BrandCode.ToUpper(),
                CreatedAt = DateTime.UtcNow,
            };

            _db.DevicesForDisplay.Add(deviceForDisplay);

            await _db.SaveChangesAsync();

            return CreatedAtAction("GetDeviceForDisplay", new { id = deviceForDisplay.Id }, ToDTO(deviceForDisplay));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviceForDisplay(int id)
        {
            var deviceForDisplay = await _db.DevicesForDisplay.FindAsync(id);
            if (deviceForDisplay == null)
            {
                return NotFound("Device for display not found");
            }

            _db.DevicesForDisplay.Remove(deviceForDisplay);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        private static DeviceForDisplayDTO ToDTO(DeviceForDisplay deviceForDisplay)
        {
            return new DeviceForDisplayDTO
            {
                Id = deviceForDisplay.Id,
                Name = deviceForDisplay.Name,
                BrandCode = deviceForDisplay.BrandCode,
                CreatedAt = deviceForDisplay.CreatedAt,
                LastUpdatedAt = deviceForDisplay.LastUpdatedAt,
            };
        }
    }
}
