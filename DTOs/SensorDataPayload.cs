﻿namespace AgroMonitor.DTOs
{
    public class SensorDataPayload
    {
        public string DeviceIdentifier { get; set; } = null!;
        public List<SensorReadingItem> Readings { get; set; } = null!;

        public class SensorReadingItem
        {
            public string SensorType { get; set; } = null!;
            public double Value { get; set; }
        }
    }
    
}
