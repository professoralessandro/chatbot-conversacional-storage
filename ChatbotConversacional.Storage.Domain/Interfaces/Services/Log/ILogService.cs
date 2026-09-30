#region REFERENCES
using ChatbotConversacionalStorage.Application.Dtos.Log;
using Microsoft.AspNetCore.Http;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Domain.Interfaces.Services.Log
{
    public interface ILogService
    {
        #region FIND BY ID
        Task<LogDto> GetByIdAsync(Guid logId);
        #endregion

        #region GET ALL
        Task<IEnumerable<LogDto>> GetAllAsync();
        #endregion

        #region GET ALL BY PARAMETER ASYNC
        Task<IEnumerable<LogDto>> GetAllPaginatedAsync(DateTime? dateAdded = null, string? paramm = null, int? pagNumber = null, int? rowsPpage = null);
        #endregion GET ALL BY PARAMETER ASYNC

        #region INSERT
        Task<LogDto> InsertAsync(LogDto obj);
        #endregion

        #region CREATE
        Task<LogDto> Create(HttpRequest request, HttpResponse response, string message, Guid? userId, string payload = null, string previousPayload = null);
        #endregion

        #region INSERT
        Task<LogDto> UpdateAsync(LogDto obj);
        #endregion

        #region DELETE
        Task<LogDto> RemoveAsync(Guid logId);
        #endregion

        #region DOWNLOADFILE BY DATE
        Task<byte[]> DownloadFile(DateTime date);
        #endregion
    }
}
