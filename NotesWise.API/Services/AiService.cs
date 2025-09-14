using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NotesWise.API.Services.Models;
using NotesWise.API.Models;


namespace NotesWise.API.Services
{
    public class AiService : IAiService
    {
        readonly HttpClient _httpClient;
        private readonly ILogger<AiService> _logger;
        readonly string _openAiKey;
        readonly string _geminiApiKey;
        readonly string _elevenLabsApiKey;
        readonly JsonSerializerOptions _jsonOptions;
        public AiService(HttpClient httpClient, IConfiguration configuration, ILogger<AiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _openAiKey = configuration["OpenAi:ApiKey"] ?? "";
            _geminiApiKey = configuration["Gemini:ApiKey"] ?? "";
            _elevenLabsApiKey = configuration["ElevenLabs:ApiKey"] ?? "";

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                WriteIndented = false
            };
        }

        public async Task<string> GenerateAudioAsync(string text, string voice = "alloy")
        {
            try
            {
                var voiceDictionary = new Dictionary<string, string>
                {
                    { "burt", "4YYIPFl9wE5c4L2eu2Gb" },
                };

                var voiceId = voiceDictionary[voice];

                var request = new ElevenLabsRequest
                {
                    Text = text,
                    Model_Id = "eleven_multilingual_v2",
                    Voice_Settings = new ElevenLabsVoiceSettings
                    {
                        Stability = 0.5,
                        Similarity_Boost = 0.75
                    }
                };

                var json = JsonSerializer.Serialize(request, _jsonOptions);

                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.elevenlabs.io/v1/text-to-speech/{voiceId}")
                {
                    Content = httpContent
                };

                httpRequest.Headers.Add("xi-api-key", $"{_elevenLabsApiKey}");

                var response = await _httpClient.SendAsync(httpRequest);

                var audioBytes = await response.Content.ReadAsByteArrayAsync();

                var base64Audio = Convert.ToBase64String(audioBytes);

                return base64Audio;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating audio");
                throw;
            }
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