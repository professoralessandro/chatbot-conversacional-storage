using ChatbotConversacionalDomain.Interfaces.Services.Log;
using ChatbotConversacionalStorage.Application.Dtos.Log;

namespace ChatbotConversacionalStorage.Business.Business.Log
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
        public static string ValidDelete(this LogDto model)
        {
            return string.Empty;
        }
        #endregion
    }
}
