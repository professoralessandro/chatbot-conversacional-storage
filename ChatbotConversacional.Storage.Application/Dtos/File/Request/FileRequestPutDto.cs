namespace ChatbotConversacionalStorage.Application.Dtos.File.Request
{
    public class FileRequestPutDto
    {
        public Guid FileId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool MainFile { get; set; }

        public bool Public { get; set; }
    }
}
