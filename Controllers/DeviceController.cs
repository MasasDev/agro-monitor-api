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
                DeviceIdentifier = device.DeviceUniqueIdentifier,
                Name = device.Name,
                Location = device.Location,
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
                DeviceIdentifier = d.DeviceUniqueIdentifier,
                Name = d.Name,
                Location = d.Location,
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
        public async Task<ActionResult<string>> RegisterDevice([FromBody] RegisterDeviceDTO registerDeviceDTO)
        {
            if (registerDeviceDTO == null)
            {
                return BadRequest();
            }
            if (string.IsNullOrWhiteSpace(registerDeviceDTO.DeviceIdentifier))
            {
                return BadRequest("The device must have a unique identifier.");
            }
            if (await _db.Devices.AnyAsync(d => d.DeviceUniqueIdentifier == registerDeviceDTO.DeviceIdentifier))
            {
                return Conflict("A device with this identifier already exists.");
            }

            Device newDevice = new Device
            {
                DeviceUniqueIdentifier = registerDeviceDTO.DeviceIdentifier,
                Name = registerDeviceDTO.Name,
                Location = registerDeviceDTO.Location,
                RegistrationDate = DateTime.UtcNow,
            };

            await _db.Devices.AddAsync(newDevice);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDevice), new { id = newDevice.Id }, newDevice.DeviceUniqueIdentifier);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> UnregisterDevice(int id)
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

            return Ok(new { message = "Device unregistered successfully", id = device.Id });
        }


    }
}
