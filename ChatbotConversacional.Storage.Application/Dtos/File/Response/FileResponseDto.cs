#region REFERENCES
using Microsoft.AspNetCore.Http;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Application.Dtos.File.Request
{
    public class FileResponseDto
    {
        public Guid FileId { get; set; }

        public byte[] File { get; set; }

        public string Title { get; set; }

        public string FilePath { get; set; }

        public string Description { get; set; }

        public bool MainFile { get; set; }

        public bool Public { get; set; }
    }
}
