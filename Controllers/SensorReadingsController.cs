using AgroMonitor.Data;
using AgroMonitor.DTOs;
using AgroMonitor.Migrations;
using AgroMonitor.Models;
using AgroMonitor.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorReadingsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ISensorReadingsProcessor _sensorReadingsProcessor;
        public SensorReadingsController(ApplicationDbContext db, ISensorReadingsProcessor sensorReadingsProcessor) 
        {
            _db = db;
            _sensorReadingsProcessor = sensorReadingsProcessor;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SensorReading>> GetReading(int id)
        {
           var reading = await _db.SensorReadings.Include(reading => reading.Device).FirstOrDefaultAsync(reading => reading.Id == id);

            if(reading == null)
            {
                return NotFound();
            }

            SensorReadingDTO sensorReadingDTO = new SensorReadingDTO
            {
                Id = id,
                SensorType = reading.SensorType,
                SensorValue = reading.SensorValue,
                Timestamp = reading.TimeStamp,
                DeviceName = reading.Device.Name,
            };

            return Ok(sensorReadingDTO);
        }
   
        [HttpGet]
        public async Task<ActionResult<List<SensorReadingDTO>>> GetAllReadings()
        {
            if(!_db.SensorReadings.Any())
            {
                return NotFound("There are no readings yet. Try later");
            }

            var readings = await _db.SensorReadings.Include(reading => reading.Device).Select(reading =>
                new SensorReadingDTO
                {
                    Id = reading.Id,
                    SensorType = reading.SensorType,
                    SensorValue = reading.SensorValue,
                    Timestamp = reading.TimeStamp,
                    DeviceName = reading.Device.Name,
                }
            ).ToListAsync();

            return Ok(readings);
        }

        [HttpPost]
        public async Task<ActionResult> PostReadings([FromBody]SensorDataPayload payload)
        {
            if(payload == null || string.IsNullOrEmpty(payload.DeviceIdentifier) || payload.Readings == null || !payload.Readings.Any())
            {
                return BadRequest();
            }

            var device = await _db.Devices.FirstOrDefaultAsync(d => d.DeviceUniqueIdentifier == payload.DeviceIdentifier);

            if (device == null)
            {
                return NotFound("Device not found");
            }

            var readings = payload.Readings.Select(reading => 
                new SensorReading
                {
                    SensorType = reading.SensorType,
                    SensorValue = reading.Value,
                    TimeStamp = DateTime.UtcNow,
                    DeviceId = device.Id
                }
            ).ToList();

            _db.SensorReadings.AddRange(readings);
            await _db.SaveChangesAsync();

            await _sensorReadingsProcessor.ProcessAsync(readings);

            return Ok(new
            {
                Device = device.Name,
                InsertedCount = readings.Count,
                Timestamp = DateTime.UtcNow
            });
        }       
        
    }
}
