using AgroMonitor.Data;
using AgroMonitor.DTOs;
using AgroMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeviceController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public DeviceController(ApplicationDbContext db) 
        {
            _db = db;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDTO>> GetDevice(int id)
        {
            var device = await _db.Devices.Include(d => d.Readings).FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
            {
                return NotFound();
            }

            DeviceDTO deviceDTO = new DeviceDTO
            {
                DeviceUniqueIdentifier = device.DeviceUniqueIdentifier,
                Name = device.Name,
                RegistrationDate = device.RegistrationDate,
                Readings = device.Readings.Select(r => new SensorReadingDTO
                {
                    SensorType = r.SensorType,
                    SensorValue = r.SensorValue,
                    Timestamp = r.TimeStamp,
                    DeviceName = device.Name

                }).ToList()
            };

            return Ok(deviceDTO);
        }

        [HttpGet]
        public async Task<ActionResult<List<DeviceDTO>>> GetAllDevices()
        {
            var devices = await _db.Devices.Include(_ => _.Readings).Select(d => new DeviceDTO
            {
                DeviceUniqueIdentifier = d.DeviceUniqueIdentifier,
                Name = d.Name,
                RegistrationDate = d.RegistrationDate,
                Readings = d.Readings.Select(r => new SensorReadingDTO
                {
                    SensorType = r.SensorType,
                    SensorValue = r.SensorValue,
                    Timestamp = r.TimeStamp,
                    DeviceName = d.Name,

                }).ToList()

            }).ToListAsync();

            return Ok(devices);
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddDevice([FromBody] AddDeviceDTO addDeviceDTO)
        {
            if (addDeviceDTO == null)
            {
                return BadRequest("The payload is missing");
            }
            if (string.IsNullOrWhiteSpace(addDeviceDTO.DeviceUniqueIdentifier))
            {
                return BadRequest("The device must have a unique identifier.");
            }
            if (await _db.Devices.AnyAsync(d => d.DeviceUniqueIdentifier == addDeviceDTO.DeviceUniqueIdentifier))
            {
                return Conflict("A device with this identifier already exists.");
            }

            Device newDevice = new Device
            {
                DeviceUniqueIdentifier = addDeviceDTO.DeviceUniqueIdentifier,
                Name = addDeviceDTO.Name,
                RegistrationDate = DateTime.UtcNow,
            };

            await _db.Devices.AddAsync(newDevice);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDevice), new { id = newDevice.Id }, newDevice.DeviceUniqueIdentifier);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveDevice(int id)
        {
            var device = await _db.Devices
                .Include(d => d.Readings)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
            {
                return NotFound("This device does not exist in the system");
            }

            _db.SensorReadings.RemoveRange(device.Readings);

            _db.Devices.Remove(device);

            await _db.SaveChangesAsync();

            return Ok(new { message = "Device removed successfully", id = device.Id });
        }


    }
}
