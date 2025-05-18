using System.Text.Json.Serialization;
using AgroMonitor.Data;
using AgroMonitor.DTOs;
using AgroMonitor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroMonitor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SmsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public SmsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult> ReceiveIncomingMessage([FromBody]IncomingMessage incomingMessage)
        {
            if (incomingMessage == null)
            {
                return BadRequest("No incoming message");
            }

            String message = incomingMessage.Message;

            if (!message.ToLower().StartsWith("deviceid="))
            {
                return BadRequest("First data field must be deviceId");
            }

            string[] data = message.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);

            string extractedDeviceIdentifier = data[0].Substring(9, data[0].Length - 9);

            var device = await _db.Devices.FirstOrDefaultAsync(d => d.DeviceUniqueIdentifier == extractedDeviceIdentifier);

            if(device == null)
            {
                return BadRequest("There is no device in the system with this id : " + extractedDeviceIdentifier);
            }

            List<SensorReading> readings = new List<SensorReading>();

            for (int i = 1; i < data.Length; i++)
            {
                string[] sensorData = data[i].Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                SensorReading sensorReading = new SensorReading
                {
                    SensorType = sensorData[0],
                    SensorValue = double.Parse(sensorData[1]),
                    TimeStamp = DateTime.UtcNow,
                    DeviceId = device.Id,
                };

                readings.Add(sensorReading);
            }

            await _db.SensorReadings.AddRangeAsync(readings);
            await _db.SaveChangesAsync();

            return Ok(new
            {
                Device = device.Name,
                InsertedCount = readings.Count,
                Timestamp = DateTime.UtcNow
            });
        }

        public class IncomingMessage
        {
            [JsonPropertyName("smsId")]
            public string SmsId { get; set; }

            [JsonPropertyName("sender")]
            public string Sender { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("receivedAt")]
            public DateTime ReceivedAt { get; set; }

            [JsonPropertyName("deviceId")]
            public string DeviceId { get; set; }

            [JsonPropertyName("webhookSubscriptionId")]
            public string WebhookSubscriptionId { get; set; }

            [JsonPropertyName("webhookEvent")]
            public string WebhookEvent { get; set; }
        }
    }
}
