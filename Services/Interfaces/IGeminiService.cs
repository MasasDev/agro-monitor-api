using AgroMonitor.DTOs;

namespace AgroMonitor.Services.Interfaces
{
    public interface IGeminiService
    {
        Task<string> AnalyzePromptAsync(string prompt);
    }
}
