#region REFERENCES
using ChatbotConversacionalStorage.Application.Dtos.File.Request;
using ChatbotConversacionalStorage.Application.Dtos.Log;
using ChatbotConversacionalStorage.Application.Helper.Static.File;
using ChatbotConversacionalStorage.Application.Helper.Static.Generic;
using ChatbotConversacionalStorage.Domain.Entities;
using ChatbotConversacionalStorage.Domain.Interfaces.Repository.FileStorage;
using ChatbotConversacionalStorage.Domain.Interfaces.Services.FileStorage;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.ComponentModel.DataAnnotations;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Domain.Servies.FileStorage
{
    public class FileStorageService : IFileStorageService
    {
        #region ATRIBUTTES
        private readonly IFileStorageRepository _repository;
        private readonly IMapper _mapper;
        #endregion

        #region CONTRUCTORS
        public FileStorageService(IFileStorageRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        #endregion

        #region PUBLIC METHOD
        public async Task<StorageFile> SaveImageAsync(FileRequestPostDto postRequest, Guid userId)
        {
            try
            {
                #region VALIDATION
                var msgLog = "\nSaveImageAsync => Validating Image\n";
                Serilog.Log.Information(msgLog);
                if (postRequest == null) throw new  ValidationException("Invalid post request");
                #endregion VALIDATION

                #region SAVING IMAGE
                msgLog = "\nSaveImageAsync => Creating file into base\n";
                Serilog.Log.Information(msgLog);
                var model = await CreatePostStorageFileObjectAsync(postRequest, userId);
                #endregion SAVING IMAGE

                Serilog.Log.Information("\nSaveImageAsync => Creating FileStorage into data base\n");
                await _repository.AddAsync(model);

                return model;
            }
            catch (ValidationException ex)
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nVALIDATION ERROR: Houve um erro ao alterar o arquivo: \n" + ex.Message;
                msgLog += "\n======================================================================================";
                Serilog.Log.Error(msgLog);

                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nERROR: Houve um erro ao alterar o arquivo: \n" + ex.Message;
                msgLog += "\n======================================================================================";
                Serilog.Log.Error(msgLog);

                throw new Exception(ex.Message);
            }
        }

        #region FIND BY ID
        public async Task<FileResponseDto> GetByIdAsync(Guid fileId, bool isDownlodFile = false)
        {
            try
            {
                var result = await _repository.GetByIdAsync(fileId);

                var resultDto = _mapper.Map<StorageFile, FileResponseDto>(result);

                if (isDownlodFile) resultDto.File = await this.PutImageOnFileStoreObject(resultDto.FilePath);

                return resultDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao buscar o registro desejado!" + ex.Message);
            }
        }
        #endregion

        #region GET ALL ASYNC
        public async Task<List<FileResponseDto>> GetAllAsync(Guid? fileId = null, string? title = null, Guid? externalReferenceId = null, int? pagNumber = null, int? rowsPpage = null, bool isDowlloadFile = false)
        {
            try
            {
                var result2 = await _repository.GetAllPaginatedAsync<StorageFile>(fileId, title, externalReferenceId, pagNumber, rowsPpage);

                var resultDto = _mapper.Map<IEnumerable<StorageFile>, List<FileResponseDto>>(result2);

                if (isDowlloadFile) {
                    if (resultDto.Any())
                    {
                        for (int i = 0; i < resultDto.Count(); i++)
                        {
                            resultDto[i].File = await this.PutImageOnFileStoreObject(resultDto[i].FilePath);
                        }
                    }
                }                

                return resultDto;
            }
            catch (Exception ex)
            {
                throw new Exception("Não foi possível realizar a busca por registros: " + ex.Message);
            }
        }
        #endregion

        #region DOWNLOADFILE BY DATE
        public async Task<byte[]> DownloadFile(string filePath)
        {
            try
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nDownloadFile => Buscando por registros na base de dados";
                msgLog += "\n======================================================================================";
                Serilog.Log.Information(msgLog);

                #region VALIDATION
                Serilog.Log.Information("\nDownloadFile Async => Validating user\n");
                if (!File.Exists(filePath)) throw new ValidationException($"file no found.");
                #endregion ENDREGION

                Serilog.Log.Information("\nDownloadFile Async => Downloading File\n");
                return File.ReadAllBytes(filePath);
            }
            catch (ValidationException ex)
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nVALIDATION ERROR: Houve um erro ao gerar o arquivo: \n" + ex.Message;
                msgLog += "\n======================================================================================";
                Serilog.Log.Error(msgLog);

                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nERROR: Houve um erro ao gerar o arquivo: \n" + ex.Message;
                msgLog += "\n======================================================================================";
                Serilog.Log.Error(msgLog);

                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region UPDATE
        public async Task<FileResponseDto> UpdateAsync(FileRequestPutDto postRequest, Guid userUpdatedId)
        {
            try
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nUpdate FileStorage Async\n";
                msgLog += "\n======================================================================================";
                Serilog.Log.Information(msgLog);

                var model = await _repository.GetByIdAsync(postRequest.FileId);

                #region VALIDATION
                msgLog = "\nUpdate FileStorage Async => Validating user\n";
                Serilog.Log.Information(msgLog);
                if (model.UserAddedId != userUpdatedId) throw new ValidationException("Erro ao valiar o usuario.");
                #endregion VALIDATION

                model = UpdateAtributtesStorageFile(model, postRequest, userUpdatedId);

                Serilog.Log.Information("\nUpdate FileStorage Async => Updating FileStorage \n");
                await _repository.UpdateAsync(model.TrasnformObjectPropValueToUpper());

                var resultDto = _mapper.Map<StorageFile, FileResponseDto>(model);

                return resultDto;
            }
            catch (ValidationException ex)
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nVALIDATION ERROR: Houve um erro ao alterar o arquivo: \n" + ex.Message;
                msgLog += "\n======================================================================================";
                Serilog.Log.Error(msgLog);

                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nERROR: Houve um erro ao alterar o arquivo: \n" + ex.Message;
                msgLog += "\n======================================================================================";
                Serilog.Log.Error(msgLog);

                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region DELETE
        public async Task<FileResponseDto> RemoveAsync(Guid fileId, Guid userDeletedId)
        {
            try
            {
                var model = await _repository.GetByIdAsync(fileId);

                #region VALIDATION
                Serilog.Log.Information("\nRemove FileStorage Async => Validating user\n");
                if (model.UserAddedId != userDeletedId) throw new ValidationException("Erro ao valiar o usuario.");
                #endregion VALIDATION

                Serilog.Log.Information("\nRemove FileStorage Async => Deleting real file\n");
                DeleteFile(model.FilePath);

                Serilog.Log.Information("\nRemove FileStorage Async => Deleting real file\n");
                await _repository.RemoveAsync(model);

                var resultDto = _mapper.Map<StorageFile, FileResponseDto>(model);

                return resultDto;
            }
            catch (ValidationException ex)
            {
                throw new ValidationException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Houve um erro ao deletar registro: " + ex.Message);
            }
        }
        #endregion

        #endregion PUBLIC METHOD

        #region PRIVATE METHOD
        private async Task<dynamic> SavePostImageAsync(FileRequestPostDto postRequest, Guid userId)
        {
            // CREATING DIRECTORY
            var uniqueFileName = FileHelper.GetUniqueFileName(Guid.NewGuid().ToString());

            var uploads = Path.Combine(FileHelper.GetBaseDirectoryPath(), "users", "posts", userId.ToString());

            var filePath = Path.Combine(uploads, uniqueFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            // CREATING FILE
            var file = FileHelper.ConvertBase64ToImage(postRequest.ProfileImage, postRequest.Title);

            filePath = string.Concat(filePath, FileHelper.GetFileTypeFromBase64(postRequest.ProfileImage)).Replace("image/", ".");

            var fileStream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(fileStream);

            fileStream.Close();
            fileStream.Dispose();

            return filePath;
        }

        private async Task<StorageFile> CreatePostStorageFileObjectAsync(FileRequestPostDto postRequest, Guid userId)
        {
            return new StorageFile {
                Title = postRequest.Title,
                FilePath = await SavePostImageAsync(postRequest, userId),
                Description = postRequest.Description,
                MainFile = postRequest.MainFile.Value,
                Public = postRequest.Public.Value,
                ExternalReferenceId = userId,
                DateAdded = DateTime.Now,
                UserAddedId = userId
            };
        }

        private async Task<byte[]> PutImageOnFileStoreObject(string filePath)
        {
            try
            {
                return await DownloadFile(filePath);
            }
            catch
            {
                return null;
            }
        }

        private StorageFile UpdateAtributtesStorageFile(StorageFile olderObj, FileRequestPutDto newObjt, Guid userId)
        {
            olderObj.MainFile = newObjt.MainFile;
            olderObj.Public = newObjt.Public;
            olderObj.Description = newObjt.Description;
            olderObj.Title = newObjt.Title;
            olderObj.DateUpdated = DateTime.Now;
            olderObj.UserUpdatedId = userId;

            return olderObj;
        }

        public void DeleteFile(string filePath)
        {
            try
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nProcessando a remocao do arquivo :" + filePath;
                msgLog += "\n======================================================================================";
                Serilog.Log.Information(msgLog);

                #region VALIDATION
                if (!File.Exists(filePath)) throw new ValidationException($"file no found.");
                #endregion ENDREGION

                File.Delete(filePath);

                msgLog += "\nArquivo deletado.";
                Serilog.Log.Information(msgLog);
            }
            catch (ValidationException ex)
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nVALIDATION ERROR: Houve um erro ao gerar o arquivo: \n" + ex.Message;
                msgLog += "\n======================================================================================";
                Serilog.Log.Error(msgLog);

                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                var msgLog = "\n======================================================================================";
                msgLog += "\nERROR: Houve um erro ao gerar o arquivo: \n" + ex.Message;
                msgLog += "\n======================================================================================";
                Serilog.Log.Error(msgLog);

                throw new Exception(ex.Message);
            }
        }
        #endregion PRIVATE METHOD
    }
}
