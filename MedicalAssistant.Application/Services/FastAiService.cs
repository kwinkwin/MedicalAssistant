using MedicalAssistant.Application.DTOs;
using MedicalAssistant.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly string _apiKey; 
        private readonly ILogger<FastAiService> _logger; 
        public FastAiService(HttpClient httpClient, IConfiguration configuration, ILogger<FastAiService> logger)
        {
            _httpClient = httpClient;
            // Đọc URL từ appsettings.json
            _pythonApiUrl = configuration["PythonService:BaseUrl"];
            _apiKey = configuration["PythonService:ApiKey"]; // Đọc Key từ appsettings.json
            _logger = logger;
        }

        public async Task<string> GetAnswerFromAiAsync(string question, List<MessageDto>? history = null)
        {
            try
            {
                var safeHistory = history ?? new List<MessageDto>();

                _logger.LogInformation($"[FastAiService] Sending request. Question: {question}");
                _logger.LogInformation($"[FastAiService] History count: {safeHistory.Count}");

                var historyPayload = safeHistory.Select(x => new
                {
                    role = (x.IsAiResponse == 1) ? "ai" : "user",
                    content = x.Content,
                    createdDate = x.CreatedDate
                }).ToList();

                var payload = new
                {
                    text = question,
                    history = historyPayload
                };

                var requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{_pythonApiUrl}/api/chat");

                // Thêm Header bảo mật
                requestMessage.Headers.Add("X-API-Key", _apiKey);
                requestMessage.Content = JsonContent.Create(payload);

                // Gửi request
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<AiResponse>();
                return result?.Answer ?? "Sorry. AI has not responsed.";
            }
            catch (Exception ex)
            {
                _logger.LogError($"AI Connection error: {ex.Message}");
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
