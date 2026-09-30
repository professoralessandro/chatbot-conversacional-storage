#region REFERENCES
using ChatbotConversacionalStorage.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Domain.Interfaces.Repository.FileStorage
{
    public interface IFileStorageRepository
    {
        #region FIND BY ID
        Task<StorageFile> GetByIdAsync(Guid id);
        #endregion

        #region GET ALL
        Task<IEnumerable<T>> GetAllPaginatedAsync<T>(Guid? fileId = null, string? title = null, Guid? externalReferenceId = null, int? pagNumber = null, int? rowsPpage = null);
        #endregion

        #region RETURN LIST WITH PARAMETERS PAGINATED
        // Task<IEnumerable<T>> ReturnListWithParametersPaginated<T>(Guid userId, Guid? id = null, string descricao = null, bool? ativo = null, int? pageNumber = null, int? rowspPage = null);
        #endregion

        #region INSERT
        Task AddAsync(StorageFile obj);
        #endregion

        #region UPDATE
        Task UpdateAsync(StorageFile obj);
        #endregion

        #region DELETE
        Task RemoveAsync(StorageFile obj);
        #endregion
    }
}
