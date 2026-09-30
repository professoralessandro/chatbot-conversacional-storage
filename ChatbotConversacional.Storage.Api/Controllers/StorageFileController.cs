#region REFERENCES
using ChatbotConversacionalStorage.Api.Controllers.Common;
using ChatbotConversacionalStorage.Application.Dtos.Base.Response.Common;
using ChatbotConversacionalStorage.Application.Dtos.File.Request;
using ChatbotConversacionalStorage.Application.Dtos.Log;
using ChatbotConversacionalStorage.Application.Helper.Static.Generic;
using ChatbotConversacionalStorage.Application.Helper.Static.User;
using ChatbotConversacionalStorage.Domain.Interfaces.Services.FileStorage;
using ChatbotConversacionalStorage.Domain.Interfaces.Services.Log;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Serilog;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Api.Controllers
{
    public class StorageFileController : CommonController
    {
        #region ATTRIBUTTES
        private readonly IFileStorageService _service;
        private readonly ILogService _log;
        #endregion

        #region CONSTRUCTOR
        public StorageFileController(IFileStorageService service, ILogService log)
        {
            _service = service;
            _log = log;
        }
        #endregion

        #region GET BY ID
        [HttpGet, Route("GetById"), OutputCache]
        public async Task<ActionResult<FileResponseDto>> GetById(
            [FromQuery] Guid fileId
            )
        {
            try
            {
                var result = await _service.GetByIdAsync(fileId);

                if (result == null) { return NotFound($"register: {fileId} not found"); }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return UnprocessableEntity(ex.Message);
            }
        }
        #endregion

        #region GET ALL
        [HttpGet, Route("GetAll"), AllowAnonymous, OutputCache]
        public async Task<ActionResult<List<FileResponseDto>>> GetAllAsync(
            [FromQuery] Guid? fileId,
            [FromQuery] string? title,
            [FromQuery] Guid? externalReferenceId,
            [FromQuery] bool? isDownloaFile,
            [FromQuery] int? pagNumber,
            [FromQuery] int? rowsPpage
            )
        {
            try
            {
                var result = await _service.GetAllAsync(fileId, title, externalReferenceId, pagNumber, rowsPpage, isDownloaFile.HasValue ? isDownloaFile.Value : false);

                if (!result.Any()) throw new ValidationException("No records found");

                return Ok(new BaseResponseDto
                {
                    Message = "User authenticated!!",
                    StatusCode = Response.StatusCode,
                    IsSuccess = true,
                    JsonObject = result.OrderByDescending(c => c.Title)
                });
            }
            catch (ValidationException ex)
            {
                string errorMessage = "ERROR => GetAllAsync => There was an unexpected error: " + ex.Message;
                Serilog.Log.Error(ex, UtilHelper.FormatLogInformationMessage(message: errorMessage, isHeaderOrFooter: true, request: Request, response: Response, userId: UserHelper.UserId));
                await this._log.Create(this.Request, this.Response, errorMessage, userId: UserHelper.UserId);
                return NotFound(new BaseResponseDto
                {
                    Message = errorMessage,
                    StatusCode = Response.StatusCode,
                    IsSuccess = false
                });
            }
            catch (Exception ex)
            {
                string errorMessage = "ERROR => GetAllAsync => There was an unexpected error: " + ex.Message;
                Serilog.Log.Error(ex, UtilHelper.FormatLogInformationMessage(message: errorMessage, isHeaderOrFooter: true, request: Request, response: Response, userId: UserHelper.UserId));
                await this._log.Create(this.Request, this.Response, errorMessage, userId: UserHelper.UserId);
                return BadRequest(new BaseResponseDto
                {
                    Message = errorMessage,
                    StatusCode = Response.StatusCode,
                    IsSuccess = false
                });
            }
        }
        #endregion

        #region POST IMAGE
        [HttpPost, Route("post-image"), RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> SubmitPost([FromBody] FileRequestPostDto postRequest)
        {
            try
            {
                if (!string.IsNullOrEmpty(postRequest.ProfileImage))
                {
                    var resp = await _service.SaveImageAsync(postRequest, UserHelper.UserId);
                    // return Ok(resp);

                    return Ok(new BaseResponseDto {
                        IsSuccess = true,
                        StatusCode = 204,
                        JsonObject = JsonSerializer.Serialize(new {
                            FileId = resp.FileId,
                            FilePath = resp.FilePath
                        })
                    });
                }
                else
                {
                    return NotFound(new BaseResponseDto {IsSuccess = false, StatusCode = 404, Message = "The file should be bonded" });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, UtilHelper.FormatLogInformationMessage("Method => SubmitPost" + ex.Message, Guid.Parse("d2a833de-5bb4-4931-a3c2-133c8994072a"), request: Request, response: Response));
                this.Response.StatusCode = 422;
                await this._log.Create(this.Request, this.Response, ex.Message, null);
                return UnprocessableEntity(new BaseResponseDto { IsSuccess = false, StatusCode = this.Response.StatusCode, Message = ex.Message });
            }
        }
        #endregion POST IMAGE

        #region POST PROFILE IMAGE
        [HttpPost, Route("post-profile-image"), AllowAnonymous, RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> SubmitProfilePost([FromBody] ProfileFileRequestPostDto postRequest)
        {
            try
            {
                if (!string.IsNullOrEmpty(postRequest.ProfileImage))
                {
                    var resp = await _service.SaveImageAsync(postRequest, postRequest.ExternalReferenceId);
                    
                    return Ok(new BaseResponseDto
                    {
                        IsSuccess = true,
                        StatusCode = 204,
                        JsonObject = JsonSerializer.Serialize(new
                        {
                            FileId = resp.FileId,
                            FilePath = resp.FilePath
                        })
                    });
                }
                else
                {
                    return NotFound(new BaseResponseDto { IsSuccess = false, StatusCode = 404, Message = "The file should be bonded" });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                Log.Error(ex, UtilHelper.FormatLogInformationMessage("Method => SubmitPost" + ex.Message, Guid.Parse("d2a833de-5bb4-4931-a3c2-133c8994072a"), request: Request, response: Response));
                this.Response.StatusCode = 422;
                await this._log.Create(this.Request, this.Response, ex.Message, null);
                return UnprocessableEntity(new BaseResponseDto { IsSuccess = false, StatusCode = this.Response.StatusCode, Message = ex.Message });
            }
        }
        #endregion POST PROFILE IMAGE

        #region UPDATE
        [HttpPut]
        public async Task<ActionResult> Update(FileRequestPutDto postRequest)
        {
            try
            {
                if (!Guid.TryParse(this.Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.PrimarySid)?.Value, out Guid userUpdatedId)) return NotFound("Error to validate user");

                var response = await _service.UpdateAsync(postRequest, userUpdatedId);
                await this._log.Create(this.Request, this.Response, this.Response.StatusCode.ToString(), null);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, UtilHelper.FormatLogInformationMessage("Method => Update" + ex.Message, Guid.Parse("d2a833de-5bb4-4931-a3c2-133c8994072a"), request: Request, response: Response));
                this.Response.StatusCode = 422;
                await this._log.Create(this.Request, this.Response, ex.Message, null);
                return UnprocessableEntity(new BaseResponseDto { IsSuccess = false, StatusCode = this.Response.StatusCode, Message = ex.Message });
            }
        }
        #endregion

        #region DELETE
        [HttpDelete]
        public async Task<ActionResult> Delete([FromQuery] Guid id)
        {
            try
            {
                if (!Guid.TryParse(this.Request.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.PrimarySid)?.Value, out Guid userDeletedId)) return NotFound("Error to validate user");

                await _service.RemoveAsync(id, userDeletedId);
                await this._log.Create(this.Request, this.Response, this.Response.StatusCode.ToString(), null);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, UtilHelper.FormatLogInformationMessage("Method => Update" + ex.Message, Guid.Parse("d2a833de-5bb4-4931-a3c2-133c8994072a"), request: Request, response: Response));
                this.Response.StatusCode = 422;
                await this._log.Create(this.Request, this.Response, ex.Message, null);
                return UnprocessableEntity(new BaseResponseDto { IsSuccess = false, StatusCode = this.Response.StatusCode, Message = ex.Message });
            }
        }
        #endregion

        #region DOWNLOADFILE BY DATE
        // Faz Download de um arquivo
        [HttpGet, Route("DownloadFile")]
        public async Task<IActionResult> Download([FromQuery] string filePath)
        {
            try
            {
                var result = await _service.DownloadFile(filePath);

                // Define o tipo MIME do arquivo (por exemplo, "application/pdf" para PDF)
                string tipoMime = "application/octet-stream";

                string fileName = Path.GetFileName(filePath);

                return File(result, tipoMime, fileName);
            }
            catch (Exception ex)
            {
                if (ex.Message == "filepart not present") return NotFound(ex.Message);

                string errorMessage = "Method => Update => filepart not present" + ex.Message;
                Log.Error(ex, UtilHelper.FormatLogInformationMessage(errorMessage, Guid.Parse("d2a833de-5bb4-4931-a3c2-133c8994072a"), request: Request, response: Response));
                this.Response.StatusCode = 422;
                await this._log.Create(this.Request, this.Response, errorMessage, null);
                return UnprocessableEntity(new BaseResponseDto { IsSuccess = false, StatusCode = this.Response.StatusCode, Message = ex.Message });
            }
        }
        #endregion
    }
}