#region REFERENCES
using ChatbotConversacionalStorage.Application.Dtos.File.Request;
using ChatbotConversacionalStorage.Application.Dtos.File.Response;
using ChatbotConversacionalStorage.Application.Dtos.Log;
using ChatbotConversacionalStorage.Domain.Entities;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Domain.Interfaces.Services.FileStorage
{
    public interface IFileStorageService
    {
        #region FIND BY ID
        Task<FileResponseDto> GetByIdAsync(Guid fileId, bool isDownlodFile = false);
        #endregion

        #region GET ALL
        Task<List<FileResponseDto>> GetAllAsync(Guid? fileId = null, string? title = null, Guid? externalReferenceId = null, int? pagNumber = null, int? rowsPpage = null, bool isDowlloadFile = false);
        #endregion

        #region POST IMAGE
        Task<StorageFile> SaveImageAsync(FileRequestPostDto postRequest, Guid userAddedId);
        #endregion POST IMAGE

        #region DOWNLOADFILE BY DATE
        Task<byte[]> DownloadFile(string filePath);
        #endregion

        #region UPDATE
        Task<FileResponseDto> UpdateAsync(FileRequestPutDto postRequest, Guid userUpdatedId);
        #endregion

        #region DELETE
        Task<FileResponseDto> RemoveAsync(Guid fileId, Guid userDeletedId);
        #endregion
    }
}
