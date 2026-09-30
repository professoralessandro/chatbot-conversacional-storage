namespace ChatbotConversacionalStorage.Application.Dtos.File.Response
{
    public class PostFileStorageResponse
    {
        public Guid StorageFileId { get; set; }
        public string Description { get; set; }
        public string Imagepath { get; set; }
    }
}