using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AgroMonitor.Services.Interfaces;

namespace AgroMonitor.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;

        public GeminiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> AnalyzePromptAsync(string prompt)
        {
            var requestPayload = new GeminiRequest
            {
                contents = new List<Content>
                {
                    new Content
                    {
                        parts = new List<Part>
                        {
                            new Part { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestPayload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_httpClient.BaseAddress, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API error: {error}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();

            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson);

            return geminiResponse?.candidates?.FirstOrDefault()?
                       .content?.parts?.FirstOrDefault()?.text
                   ?? "No response from Gemini.";
        }

        // Request DTOs
        public class GeminiRequest
        {
            public List<Content> contents { get; set; } = new();
        }

        public class Content
        {
            public List<Part> parts { get; set; } = new();
        }

        public class Part
        {
            public string text { get; set; } = string.Empty;
        }

        // Response DTOs
        public class GeminiResponse
        {
            public List<Candidate>? candidates { get; set; }
        }

        public class Candidate
        {
            public Content? content { get; set; }
            public string? finishReason { get; set; }
        }
    }
}
