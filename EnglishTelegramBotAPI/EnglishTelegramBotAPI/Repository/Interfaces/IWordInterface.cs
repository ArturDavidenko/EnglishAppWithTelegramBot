using EnglishTelegramBotAPI.Models;

namespace EnglishTelegramBotAPI.Repository.Interfaces
{
    public interface IWordInterface
    {
        Task<Dictionary<string, string>> GetWordsList();

        Task CreateWordsList(Dictionary<string, string> dictionary);

        Task DeleteWordsList();
    }
}
