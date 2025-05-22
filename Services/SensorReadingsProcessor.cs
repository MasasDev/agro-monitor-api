using System.Text;
using AgroMonitor.Data;
using AgroMonitor.DTOs;
using AgroMonitor.Models;
using AgroMonitor.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Twilio.Types;

namespace AgroMonitor.Shared
{
    public class SensorReadingsProcessor : ISensorReadingsProcessor
    {
        private readonly IGeminiService _geminiService;
        private readonly ApplicationDbContext _db;
        private readonly ITwilioService _twilioService;
        public SensorReadingsProcessor(IGeminiService geminiService, ITwilioService twilioService, ApplicationDbContext db)
        {
            _geminiService = geminiService;
            _twilioService = twilioService; 
            _db = db;
        }
        public async Task<string> ProcessAsync(List<SensorReading> sensorReadings)
        {
            string formattedText = ToNaturalLanguage(sensorReadings);

            string fullPrompt =
                $"You are an expert agronomist. Given this sensor data:\n{formattedText}\n" +
                "Reply with *1 short, direct farming recommendation* ONLY. Response must be actionable and fit within 160 characters. " +
                "NO greetings, NO explanations, NO filler. Just the advice. End response with a period.";

            string response =  await _geminiService.AnalyzePromptAsync(fullPrompt);

            await SaveResponseToDb(sensorReadings, response);

            return response;
        }
        public static string ToNaturalLanguage(List<SensorReading> sensorReadings)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Sensor Readings:");

            foreach (var reading in sensorReadings)
            {
                sb.AppendLine($"- {reading.SensorType}: {reading.SensorValue}");
            }

            return sb.ToString();
        }

        private async Task SaveResponseToDb(List<SensorReading> sensorReadings, string aiResponse)
        {
            if (aiResponse == null)
                return;

            SensorReadingBatch sensorReadingBatch = new SensorReadingBatch
            {
                AISuggestion = aiResponse,
                SensorReadings = sensorReadings,
                DeviceId = sensorReadings.First().DeviceId,
                CreatedAt = DateTime.UtcNow,
            };

            await _db.ReadingBatches.AddAsync(sensorReadingBatch);

            await _db.SaveChangesAsync();

            var package = _db.Packages
                    .Include(p => p.CustomerPackages)
                    .ThenInclude(cp => cp.Customer)
                    .FirstOrDefault(p => p.DeviceId == sensorReadingBatch.DeviceId);

            var customer = package.CustomerPackages.Select(cp => cp.Customer).First();

            _twilioService.SendSMS(ToNaturalLanguage(sensorReadings), customer.PhoneNumber);
            _twilioService.SendSMS(aiResponse, customer.PhoneNumber);
        }
    }
}
