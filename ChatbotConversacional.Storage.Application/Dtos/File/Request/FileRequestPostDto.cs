#region REFERENCES
using Microsoft.AspNetCore.Http;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Application.Dtos.File.Request
{
    public class FileRequestPostDto
    {
        public string? ProfileImage { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public bool? MainFile { get; set; }

        public bool? Public { get; set; }
    }
}
