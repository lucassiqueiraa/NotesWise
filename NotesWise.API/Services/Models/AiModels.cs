using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace NotesWise.API.Services.Models
{
    // Request/Response models for AI services
    public class GenerateSummaryRequest
    {
        public required string Content { get; set; }
    }

    public class GenerateSummaryResponse
    {
        public required string Summary { get; set; }
    }


    public class GenerateAudioRequest
    {
        public required string Text { get; set; }
        public string Voice { get; set; } = "alloy";
    }

    public class GenerateAudioResponse
    {
        public required string AudioContent { get; set; } // Base64 encoded audio
    }

    //OpenAI API Models
    public class OpenAiRequest
    {
        public string model { get; set; }
        public string input { get; set; }
    }

    public class OpenAiResponse
    {
        public List<OpenAiOutput> output { get; set; }
    }

    public class OpenAiOutput
    {
        public List<OpenAiContent> content { get; set; }
    }

    public class OpenAiContent
    {
        public string text { get; set; }
    }

    public class GeminiTextResponse
    {
        [JsonPropertyName("candidates")]
        public Candidate[] Candidates { get; set; }
    }

    public class Candidate
    {
        [JsonPropertyName("content")]
        public Content Content { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("parts")]
        public Part[] Parts { get; set; }
    }

    public class Part
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }


    // ElevenLabs API models
    public class ElevenLabsRequest
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; }
        [JsonPropertyName("model_id")]
        public string Model_Id { get; set; } = "eleven_monolingual_v1";
        [JsonPropertyName("voice_settings")]
        public ElevenLabsVoiceSettings Voice_Settings { get; set; } = new();
    }

    public class ElevenLabsVoiceSettings
    {
        [JsonPropertyName("stability")]
        public double Stability { get; set; } = 0.5;
        [JsonPropertyName("similarity_boost")]
        public double Similarity_Boost { get; set; } = 0.5;
    }

}