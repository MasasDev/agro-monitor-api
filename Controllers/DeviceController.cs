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
                return NotFound("Device not found");
            }

            DeviceDTO deviceDTO = ToDeviceDTO(device);

            return Ok(deviceDTO);
        }

        [HttpGet]
        public async Task<ActionResult<List<DeviceDTO>>> GetAllDevices()
        {
            var devices = await _db.Devices.Include(_ => _.Readings).Select(d => ToDeviceDTO(d)).ToListAsync();

            return Ok(devices);
        }

        [HttpPost("single")]
        public async Task<ActionResult<string>> AddDevice([FromBody] AddDeviceDTO addDeviceDTO)
        {
            if (addDeviceDTO == null)
            {
                return BadRequest("The payload is missing");
            }
            if (string.IsNullOrWhiteSpace(addDeviceDTO.BrandCode))
            {
                return BadRequest("The brand code is missing");
            }
            if (await _db.Devices.AnyAsync(d => d.BrandCode == addDeviceDTO.BrandCode))
            {
                return Conflict("A device with this brand code already exists");
            }

            Random random = new Random();

            while(await _db.Devices.AnyAsync(d => d.DeviceUniqueIdentifier == addDeviceDTO.DeviceIdentifier))
            {
                addDeviceDTO.DeviceIdentifier = "CU" + random.Next(10000, 99999).ToString();
            }

            Device newDevice = new Device
            {
                BrandCode = addDeviceDTO.BrandCode.ToUpper(),
                Name = addDeviceDTO.Name,
                DeviceUniqueIdentifier = addDeviceDTO.DeviceIdentifier,
                RegistrationDate = DateTime.UtcNow,
            };

            await _db.Devices.AddAsync(newDevice);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDevice), new { id = newDevice.Id }, newDevice.BrandCode);
        }
        [HttpPost("bulk")]
        public async Task<ActionResult<List<DeviceDTO>>> AddDevices([FromBody] List<AddDeviceDTO> addDeviceDTOList)
        {
            if (addDeviceDTOList == null || addDeviceDTOList.Count == 0)
            {
                return BadRequest("Payload is missing or empty.");
            }

            var invalidItems = addDeviceDTOList
                .Where(d => string.IsNullOrWhiteSpace(d.BrandCode))
                .ToList();

            if (invalidItems.Any())
            {
                return BadRequest("One or more devices have missing brand codes.");
            }

            var random = new Random();

            var existingIdentifiers = new HashSet<string>(
                await _db.Devices.Select(d => d.DeviceUniqueIdentifier).ToListAsync()
            );

            foreach (var dto in addDeviceDTOList)
            {
                while (string.IsNullOrWhiteSpace(dto.DeviceIdentifier) || existingIdentifiers.Contains(dto.DeviceIdentifier))
                {
                    dto.DeviceIdentifier = "CU" + random.Next(10000, 99999).ToString();
                }

                existingIdentifiers.Add(dto.DeviceIdentifier);
            }

            var newDevices = addDeviceDTOList.Select(dto => new Device
            {
                BrandCode = dto.BrandCode,
                Name = dto.Name,
                DeviceUniqueIdentifier = dto.DeviceIdentifier,
                RegistrationDate = DateTime.UtcNow
            }).ToList();

            await _db.Devices.AddRangeAsync(newDevices);
            await _db.SaveChangesAsync();

            var result = newDevices.Select(d => ToDeviceDTO(d)).ToList();

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDevice(long id, UpdateDeviceDTO updateDevice)
        {
            if (updateDevice == null)
            {
                return BadRequest("Payload is missing");
            }

            if(id != updateDevice.Id)
            {
                return BadRequest("The ID in the route does not match the ID in the payload.");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var device = await _db.Devices.FirstOrDefaultAsync(d => d.Id == id);

            if(device == null)
            {
                return NotFound("Device not found");
            }

            device.BrandCode = updateDevice.BrandCode.ToUpper();
            device.Name = updateDevice.Name;
            device.DeviceUniqueIdentifier = updateDevice.DeviceIdentifier;

             _db.Devices.Update(device);

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> RemoveDevice(int id)
        {
            var device = await _db.Devices
                .Include(d => d.Readings)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
            {
                return NotFound("Device not found");
            }

            _db.SensorReadings.RemoveRange(device.Readings);

            _db.Devices.Remove(device);

            await _db.SaveChangesAsync();

            return Ok(new { message = "Device removed successfully", id = device.Id });
        }

        private static DeviceDTO ToDeviceDTO(Device device)
        {
            return new DeviceDTO
            {
                Id = device.Id,
                BrandCode = device.BrandCode,
                DeviceUniqueIdentifier = device.DeviceUniqueIdentifier,
                Name = device.Name,
                RegistrationDate = device.RegistrationDate,
                Readings = device.Readings.Select(r => new SensorReadingDTO
                {
                    SensorType = r.SensorType,
                    SensorValue = r.SensorValue,
                    Timestamp = r.TimeStamp,
                    DeviceName = device.Name,

                }).ToList()
            };
        }


    }
}
