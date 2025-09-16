using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotesWise.API.Models;
using NotesWise.API.Services.Models;

namespace NotesWise.API.Services
{
    public interface IAiService
    {
        Task<string> GenerateSummaryAsync(string content);
        Task<List<FlashcardData>> GenerateFlashcardsAsync(string content);
        Task<string> GenerateAudioAsync(string text, string voice = "burt");
        Task<GenerateFlashcardAudioResponse> GenerateFlashcardAudioAsync(Flashcard flashcard, string voice = "burt", string type = "both");
    }
}