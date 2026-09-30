#region REFERENCES
using ChatbotConversacionalStorage.Api.Middleware;
using ChatbotConversacionalStorage.Application.Enums.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Api.Controllers.Common
{
    [ApiController, Route("api/[controller]"), Authorize, APClainsAuthorize(ClainEnum.CommonControllerClainType, ClainEnum.CommonControllerClainValue), ApiVersion("1.0")]
    public class CommonController : ControllerBase
    {
    }
}
