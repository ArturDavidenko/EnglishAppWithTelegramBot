namespace EnglishTelegrammBot.Models
{
    public class CheckViewModel
    {
        public string EnglishWord { get; set; }
        public string UserTranslation { get; set; }
        public string CorrectTranslation { get; set; }
        public bool IsCorrect { get; set; }
    }
}
