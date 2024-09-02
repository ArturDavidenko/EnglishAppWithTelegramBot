using EnglishTelegrammBot.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EnglishTelegrammBot.Controllers
{
    public class TelegramBotController : Controller
    {
        private readonly TelegramRepository _telegramRepository;

        public TelegramBotController(TelegramRepository telegramRepository)
        {
            _telegramRepository = telegramRepository;
        }

        [HttpGet]
        public async Task<IActionResult> testViewBot()
        {
            await _telegramRepository.InitTelegramBot();
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> testViewBot()
        //{

        //    return View();
        //}
    }
}
