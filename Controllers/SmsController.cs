using AgroMonitor.Data;
using AgroMonitor.Models;
using AgroMonitor.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json;

namespace AgroMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ISensorReadingsProcessor _sensorReadingsProcessor;

        public SmsController(ApplicationDbContext db, ISensorReadingsProcessor sensorReadingsProcessor)
        {
            _db = db;
            _sensorReadingsProcessor = sensorReadingsProcessor;
        }
        

        [HttpPost]
        public async Task<ActionResult> ReceiveIncomingMessage([FromBody] JsonElement payload)
        {
            if (!payload.TryGetProperty("data", out JsonElement data))
            {
                return BadRequest("Missing 'data' field in the payload.");
            }

            var sms = JsonConvert.DeserializeObject<SmsData>(data.ToString());

            if (sms == null || string.IsNullOrWhiteSpace(sms.Content))
            {
                return BadRequest("Invalid or missing SMS content.");
            }

            string message = sms.Content;

            if (!message.ToLower().StartsWith("deviceid="))
            {
                return BadRequest("First data field must be deviceId");
            }

            string[] dataParts = message.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            string extractedDeviceIdentifier = dataParts[0].Substring(9);

            var device = await _db.Devices.FirstOrDefaultAsync(d => d.DeviceUniqueIdentifier == extractedDeviceIdentifier);
            if (device == null)
            {
                return BadRequest("There is no device in the system with this id: " + extractedDeviceIdentifier);
            }

            List<SensorReading> readings = new List<SensorReading>();

            for (int i = 1; i < dataParts.Length; i++)
            {
                string[] sensorData = dataParts[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (sensorData.Length != 2 || !double.TryParse(sensorData[1], out double value))
                    continue; // Skip invalid pairs

                SensorReading sensorReading = new SensorReading
                {
                    SensorType = sensorData[0],
                    SensorValue = value,
                    TimeStamp = DateTime.UtcNow,
                    DeviceId = device.Id,
                };

                readings.Add(sensorReading);
            }

            await _db.SensorReadings.AddRangeAsync(readings);
            await _db.SaveChangesAsync();
            await _sensorReadingsProcessor.ProcessAsync(readings);

            return Ok(new
            {
                Device = device.Name,
                InsertedCount = readings.Count,
                Timestamp = DateTime.UtcNow
            });
        }
        public class SmsData
        {
            [JsonProperty("contact")]
            public string Contact { get; set; } = string.Empty;

            [JsonProperty("owner")]
            public string Owner { get; set; } = string.Empty;

            [JsonProperty("content")]
            public string Content { get; set; } = string.Empty;
        }
    }
}
