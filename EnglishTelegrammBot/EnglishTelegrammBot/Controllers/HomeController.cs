using EnglishTelegrammBot.Models;
using EnglishTelegrammBot.Repository;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace EnglishTelegrammBot.Controllers
{
    public class HomeController : Controller
    {

        private readonly WordRepository _wordRepository;
  
        public HomeController(WordRepository wordRepository)
        {
            _wordRepository = wordRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(FileUploadViewModel model)
        {
            var result = await _wordRepository.ReadFile(model);
            var loadedFiles = await _wordRepository.LoadFileFromDB();
            if (loadedFiles != null)
            {
                var modelToUpload = new FileUploadViewModel { Dictionary = loadedFiles };
                return View(modelToUpload);
            }
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> EnglishTestPage()
        {
            var dictionary =  await _wordRepository.LoadFileFromDB();
            var randomWord = await _wordRepository.GetRandomWord(dictionary);
            var model = new TestViewModel
            {
                EnglishWord = randomWord.Key,
                RussianWord = randomWord.Value 
            };
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> CheckTranslation(string userTranslation, string EnglishWord, string RussianTranslation)
        {
            var correctTranslation = RussianTranslation;
            var englishWord = EnglishWord;

            var isCorrect = string.Equals(userTranslation, correctTranslation, StringComparison.OrdinalIgnoreCase);

            return View(new CheckViewModel
            {
                EnglishWord = englishWord,
                UserTranslation = userTranslation,
                CorrectTranslation = correctTranslation,
                IsCorrect = isCorrect
            });
        }

        public async Task<IActionResult> DeleteList()
        {
            await _wordRepository.DeleteList();
            return View("Index");
        }

    }
}
