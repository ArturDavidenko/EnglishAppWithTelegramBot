using EnglishTelegrammBot.Models;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EnglishTelegrammBot.Repository
{
    public class TelegramRepository
    {
        private readonly TelegramBotClient _client;
        private readonly BotSettings _botSettings;
        private readonly WordRepository _wordRepository;
        private readonly ILogger<TelegramRepository> _logger;
        private Dictionary<long, UserState> _userStates = new Dictionary<long, UserState>();


        public TelegramRepository(WordRepository wordRepository, ILogger<TelegramRepository> logger, IOptions<BotSettings> botSettings)
        {
            _client = new TelegramBotClient(botSettings.Value.Token);
            _wordRepository = wordRepository;
            _logger = logger;
            _botSettings = botSettings.Value;
        }

        public async Task InitTelegramBot()
        {
            _client.StartReceiving(Update, Error);
        }

        private Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
        {
            _logger.LogError(arg2, "Произошла ошибка в Telegram Bot");
            return Task.CompletedTask;
        }

        private async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;

            if (message.Text.ToLower().Contains("help"))
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Comands list: " + "\n" 
                    + "1. List: Show list of words" + "\n" 
                    + "2. Start: create test with words list"
                    );
                return;
            }

            if (message.Text.ToLower().Contains("list"))
            {
                var dictiponaryOfWords = await _wordRepository.LoadFileFromDB();

                if (dictiponaryOfWords.Count == 0)
                        await botClient.SendTextMessageAsync(message.Chat.Id, "List is empty !");

                if (dictiponaryOfWords.Count != 0)
                {
                    var stringList = _wordRepository.ConvertorToString(dictiponaryOfWords);
                    var tempFilePath = Path.GetTempFileName();
                    System.IO.File.WriteAllText(tempFilePath, stringList);

                    using (var stream = new FileStream(tempFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        await botClient.SendDocumentAsync(
                        chatId: message.Chat.Id,
                        document: new InputFileStream(stream, "list.txt"),
                        caption: "Your list!"
                        );
                    };

                    System.IO.File.Delete(tempFilePath);
                } 
                return;
            }

            if (message.Text.ToLower().Contains("start"))
            {

                var dictiponaryOfWords = await _wordRepository.LoadFileFromDB();

                await botClient.SendTextMessageAsync(message.Chat.Id, "Test start! Translate these words to russian. Write stop to exit from the test");

                var randomWord = await _wordRepository.GetRandomWord(dictiponaryOfWords);
                

                _userStates[message.Chat.Id] = new UserState
                {
                    CurrentWord = randomWord.Key,
                    ExpectedTranslation = randomWord.Value
                };
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Word: {randomWord.Key}" + "\n" + $"Your translations: ");
                return;
            }

            if (_userStates.ContainsKey(message.Chat.Id))
            {
                if (message.Text.ToLower().Contains("stop"))
                {
                    _userStates.Remove(message.Chat.Id);
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Your test was STOPPED !");
                }

                if (message.Text.ToLower().Contains("stop") != true)
                {
                    var userState = _userStates[message.Chat.Id];

                    if (message.Text.ToLower() == userState.ExpectedTranslation.ToLower())
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Your translation is CORRECT!");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Your translation is NOT CORRECT! DEBIL" + "\n" + $"Correct one is: {userState.ExpectedTranslation.ToLower()}");

                    }
                }

                if (message.Text.ToLower().Contains("stop") != true)
                {
                    var dictiponaryOfWords = await _wordRepository.LoadFileFromDB();
                    var randomWord = await _wordRepository.GetRandomWord(dictiponaryOfWords);

                    _userStates[message.Chat.Id] = new UserState
                    {
                        CurrentWord = randomWord.Key,
                        ExpectedTranslation = randomWord.Value
                    };
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Word: {randomWord.Key}" + "\n" + $"Your translations: ");
                }
                return;
            }
        }
    }
}
