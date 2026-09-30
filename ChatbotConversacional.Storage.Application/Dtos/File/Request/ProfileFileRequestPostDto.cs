namespace ChatbotConversacionalStorage.Application.Dtos.File.Request
{
    public class ProfileFileRequestPostDto : FileRequestPostDto
    {
        public Guid ExternalReferenceId { get; set; }
    }
}
