#region REFERENCES
using ChatbotConversacionalStorage.Api.Controllers.Common;
using ChatbotConversacionalStorage.Domain.Interfaces.Services.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Api.Controllers
{
    public class LogController : CommonController
    {
        #region ATTRIBUTTES
        private readonly ILogService _service;
        #endregion

        #region CONSTRUCTORS
        public LogController([FromServices] ILogService service)
        {
            _service = service;
        }
        #endregion

        #region DOWNLOADFILE BY DATE
        // Faz Download de um arquivo
        [HttpGet, Route("DownloadLogFile")]
        public async Task<IActionResult> Download([FromQuery] DateTime? date)
        {
            try
            {
                DateTime dateValue = date ?? DateTime.Now.AddDays(-1);

                var result = await _service.DownloadFile(dateValue);

                return File(result, "application/txt", $@"log{dateValue.ToString("yyyyMMdd")}.txt");
            }
            catch (Exception ex)
            {
                await this._service.Create(this.Request, this.Response, ex.Message, null);

                if (ex.Message == "filepart not present") return NotFound(ex.Message);

                return UnprocessableEntity(ex.Message);
            }
        }
        #endregion
    }
}
