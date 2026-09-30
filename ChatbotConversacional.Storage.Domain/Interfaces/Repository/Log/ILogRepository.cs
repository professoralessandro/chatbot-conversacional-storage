namespace ChatbotConversacionalStorage.Domain.Interfaces.Repository.Log
{
    public interface ILogRepository
    {
        #region FIND BY ID
        Task<ChatbotConversacionalStorage.Domain.Entities.Log> GetByIdAsync(Guid logId);
        #endregion

        #region GET ALL ASYNC
        Task<IEnumerable<ChatbotConversacionalStorage.Domain.Entities.Log>> GetAllAsync();
        #endregion

        #region GET ALL BY PARAMETER ASYNC
        Task<IEnumerable<T>> GetAllPaginatedAsync<T>(DateTime? dateAdded = null, string? paramm = null, int? pagNumber = null, int? rowsPpage = null);
        #endregion GET ALL BY PARAMETER ASYNC

        #region INSERT
        Task AddAsync(ChatbotConversacionalStorage.Domain.Entities.Log model);
        #endregion

        #region UPDATE
        Task UpdateAsync(ChatbotConversacionalStorage.Domain.Entities.Log model);
        #endregion

        #region DELETE
        Task RemoveAsync(ChatbotConversacionalStorage.Domain.Entities.Log model);
        #endregion
    }
}
