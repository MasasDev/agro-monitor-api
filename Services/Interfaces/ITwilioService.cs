namespace AgroMonitor.Services.Interfaces
{
    public interface ITwilioService
    {
        void SendSMS(string message, string phoneNumber);
    }
}
