using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NotesWise.API.Services.Models;

namespace NotesWise.API.Services
{
    public class AiService : IAiService
    {
        readonly HttpClient _httpClient;
        readonly string _openAiKey;
        readonly string _geminiApiKey;
        // readonly JsonSerializerOptions _jsonOptions;
        public AiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _openAiKey = configuration["OpenAi:ApiKey"] ?? "";
            _geminiApiKey = configuration["Gemini:ApiKey"] ?? "";
        }

        public async Task<string> GenerateSummaryAsync(string content)
        {
            var request = new
            {
                contents = new[] {
                    new {
                        parts = new[]{
                            new {
                                text = $"Resuma este texto de forma concisa: {content}"
                            }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(request);

            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent")
            {
                Content = httpContent
            };

            httpRequest.Headers.Add("x-goog-api-key", $"{_geminiApiKey}");

            var response = await _httpClient.SendAsync(httpRequest);

            var responseText = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseText);

            var geminiResponse = JsonSerializer.Deserialize<GeminiTextResponse>(responseText);

            var summary = geminiResponse.Candidates.First().Content.Parts.First().Text;

            return summary;
        }

        // public async Task<string> GenerateSummaryAsync(string content)
        // {
        //     var request = new OpenAiRequest
        //     {
        //         model = "gpt-4o-mini",
        //         input = $"Resuma este texto de forma concisa:{content}"
        //     };

        //     var json = JsonSerializer.Serialize(request);

        //     var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

        //     var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/responses")
        //     {
        //         Content = httpContent
        //     };

        //     httpRequest.Headers.Add("Authorization", $"Bearer {_openAiKey}");

        //     var response = await _httpClient.SendAsync(httpRequest);


        //     if (!response.IsSuccessStatusCode)
        //     {
        //         var errorText = await response.Content.ReadAsStringAsync();
        //         throw new Exception($"Erro ao chamar OpenAI: {response.StatusCode} - {errorText}");
        //     }

        //     var responseText = await response.Content.ReadAsStringAsync();

        //     Console.WriteLine(response.Content);

        //     var openAiResponse = JsonSerializer.Deserialize<OpenAiResponse>(responseText);

        //     var summary = openAiResponse.output.First().content.First().text;

        //     return summary;
        // }
    }
}