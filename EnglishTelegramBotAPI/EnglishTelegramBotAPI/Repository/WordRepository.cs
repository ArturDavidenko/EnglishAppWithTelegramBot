using EnglishTelegramBotAPI.Data;
using EnglishTelegramBotAPI.Models;
using EnglishTelegramBotAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnglishTelegramBotAPI.Repository
{
    public class WordRepository : IWordInterface
    {
        private readonly DataContext _context;
        public WordRepository(DataContext context)
        {
            _context = context;
        }

        public async Task CreateWordsList(Dictionary<string, string> dictionary)
        {
            var existingWords = await _context.words.ToListAsync();
            foreach (var item in dictionary)
            {
                if (!existingWords.Any(x => x.EngWord == item.Key && x.RusWord == item.Value))
                {
                    var word = new Word
                    {
                        EngWord = item.Key,
                        RusWord = item.Value
                    };

                    _context.words.Add(word);
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteWordsList()
        {
            var list = await _context.words.ToListAsync();
            //remove range use
            foreach (var word in list)
            {
                _context.Remove(word);
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task<Dictionary<string, string>> GetWordsList()
        {
            var wordList =  await _context.words.ToListAsync();
            var dictionary = wordList.ToDictionary(word => word.EngWord, word => word.RusWord);
            return dictionary;
        }
    }
}
