namespace EnglishTelegrammBot.Models
{
    public class FileUploadViewModel
    {
        public IFormFile UploadedFile { get; set; }
        public Dictionary<string, string> Dictionary { get; set; }
    }
}
