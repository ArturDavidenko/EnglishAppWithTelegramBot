using EnglishTelegrammBot.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace EnglishTelegrammBot.Repository
{
    public class WordRepository
    {
        private static readonly Random _random = new Random();
        private readonly HttpClient _httpClient;
        private readonly BotSettings _botSettings;

        public WordRepository(HttpClient httpClient, IOptions<BotSettings> botSettings)
        { 
            _httpClient = httpClient;
            _botSettings = botSettings.Value;
        }

        public async Task<FileUploadViewModel> ReadFile(FileUploadViewModel model)
        {
            if (model.UploadedFile != null && model.UploadedFile.Length > 0)
            {
                var dictionary = new Dictionary<string, string>();

                using (var stream = new StreamReader(model.UploadedFile.OpenReadStream()))
                {
                    string line;
                    while ((line = await stream.ReadLineAsync()) != null)
                    {
                        var parts = line.Split(new[] { '-' }, 2);
                        if (parts.Length == 2)
                        {
                            var key = parts[0].Trim();
                            var value = parts[1].Trim();
                            dictionary[key] = value;
                        }
                    }
                }
                model.Dictionary = dictionary;
                //API REQUEST HERE
                var jsonContent = new StringContent(JsonSerializer.Serialize(dictionary), Encoding.UTF8, "application/json");
                await _httpClient.PostAsync(_botSettings.ApiUrl, jsonContent);
            }
            return model;
        }

        public async Task<KeyValuePair<string, string>> GetRandomWord(Dictionary<string, string> dictionary)
        {
            var keys = dictionary.Keys.ToList();
            var randomKey = keys[_random.Next(keys.Count)];
            return new KeyValuePair<string, string>(randomKey, dictionary[randomKey]);
        }

        public async Task<Dictionary<string, string>> LoadFileFromDB()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(_botSettings.ApiUrl);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Dictionary<string, string> dictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse);
                return dictionary;
            }
            
        }

        public async Task DeleteList()
        {
            await _httpClient.DeleteAsync(_botSettings.ApiUrl);
        }


        public string ConvertorToString(Dictionary<string, string> dictionary)
        {
            StringBuilder result = new StringBuilder();

            foreach (var item in dictionary)
            {
                result.AppendLine($"{item.Key} {item.Value}");
            }
            return result.ToString(); 
        }
    }
}
