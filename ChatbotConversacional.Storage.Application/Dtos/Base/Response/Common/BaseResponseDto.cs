#region ATTRIBUTTES
using System.Text.Json.Serialization;
#endregion ATTRIBUTTES

namespace ChatbotConversacionalStorage.Application.Dtos.Base.Response.Common
{
    public class BaseResponseDto
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public object? JsonObject { get; set; }
    }
}
