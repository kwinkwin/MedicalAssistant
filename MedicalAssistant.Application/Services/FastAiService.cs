using MedicalAssistant.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAssistant.Application.Services
{
    public class FastAiService : IAiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pythonApiUrl;

        public FastAiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            // Đọc URL từ appsettings.json
            _pythonApiUrl = configuration["PythonService:BaseUrl"];
        }

        public async Task<string> GetAnswerFromAiAsync(string question)
        {
            try
            {
                var payload = new { text = question };

                var response = await _httpClient.PostAsJsonAsync($"{_pythonApiUrl}/api/chat", payload);
                response.EnsureSuccessStatusCode();

                // AI service trả về JSON: { "answer": "Nội dung..." }
                var result = await response.Content.ReadFromJsonAsync<AiResponse>();
                return result?.Answer ?? "Sorry. AI has not responsed.";
            }
            catch (Exception ex)
            {
                // Error
                return $"AI Connection error: {ex.Message}";
            }
        }

        private class AiResponse
        {
            public string Answer { get; set; }
            public object DebugInfo { get; set; }
        }
    }
}
