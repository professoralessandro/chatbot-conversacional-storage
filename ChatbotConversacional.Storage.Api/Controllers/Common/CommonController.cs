#region REFERENCES
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Api.Controllers.Common
{
    [ApiController, Route("api/[controller]"), Authorize, ApiVersion("1.0")]
    public class CommonController : ControllerBase
    {
    }
}
