using AgroMonitor.DTOs;
using AgroMonitor.Models;

namespace AgroMonitor.Services.Interfaces
{
    public interface ISensorReadingsProcessor
    {
        Task<string> ProcessAsync(List<SensorReading> sensorReadings);
    }
}
