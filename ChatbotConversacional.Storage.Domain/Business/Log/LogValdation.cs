#region REFERENCES
using ChatbotConversacionalStorage.Application.Dtos.Log;
using ChatbotConversacionalStorage.Domain.Interfaces.Services.Log;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Domain.Business.Log
{
    public static class LogValdation
    {
        #region INSERT
        public static async Task<string> ValidInsert(this LogDto model, ILogService _service)
        {
            return string.Empty;
        }
        #endregion

        #region UPDATE
        public static string ValidUpdate(this LogDto model)
        {
            return string.Empty;
        }
        #endregion

        #region DELETE
        public static string ValidDelete(this ChatbotConversacionalStorage.Domain.Entities.Log model)
        {
            return string.Empty;
        }
        #endregion
    }
}
