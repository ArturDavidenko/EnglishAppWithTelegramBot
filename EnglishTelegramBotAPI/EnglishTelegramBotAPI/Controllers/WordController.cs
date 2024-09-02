using EnglishTelegramBotAPI.Repository;
using EnglishTelegramBotAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EnglishTelegramBotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WordController : Controller
    {
        private readonly IWordInterface _wordRepository;

        public WordController(IWordInterface wordRepository)
        {
            _wordRepository = wordRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetWordsList()
        {
            var WordList = await _wordRepository.GetWordsList();
            return Ok(WordList);
        }

        [HttpPost]
        public async Task<ActionResult> CreateWordsList([FromBody] Dictionary<string, string> dictionary)
        {
            await _wordRepository.CreateWordsList(dictionary);
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteWordsList()
        {
            await _wordRepository.DeleteWordsList();
            return Ok();
        }
        
    }
}
