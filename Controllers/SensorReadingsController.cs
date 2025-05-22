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
           var reading = await _db.SensorReadings
                .Include(reading => reading.Device)
                .Include(reading => reading.Batch)
                .FirstOrDefaultAsync(reading => reading.Id == id);

            if(reading == null)
            {
                return NotFound();
            }

            return Ok(ToDTO(reading));
        }
   
        [HttpGet]
        public async Task<ActionResult<List<SensorReadingDTO>>> GetAllReadings()
        {
            if(!_db.SensorReadings.Any())
            {
                return NotFound("There are no readings yet. Try later");
            }

            var readings = await _db.SensorReadings
                .Include(reading => reading.Device)
                .Include(reading => reading.Batch)
                .Select(reading => ToDTO(reading)).ToListAsync();

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
        private static SensorReadingDTO ToDTO(SensorReading sensorReading)
        {
            return new SensorReadingDTO
            {
                SensorType = sensorReading.SensorType,
                SensorValue = sensorReading.SensorValue,
                Timestamp = sensorReading.TimeStamp,
                DeviceName = sensorReading.Device.Name,
                Batch = sensorReading.Batch != null ? new SensorReadingBatchDTO
                {
                    Id = sensorReading.Batch.Id,
                    SensorReadings = sensorReading.Batch.SensorReadings,
                    AISuggestion = sensorReading.Batch.AISuggestion,
                    Device = sensorReading.Device,
                    CreatedAt = sensorReading.Batch.CreatedAt,

                } : null

            };
        }
        
    }
}
