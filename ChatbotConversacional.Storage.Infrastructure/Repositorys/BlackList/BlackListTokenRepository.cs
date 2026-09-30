#region REFERENCES
using ChatbotConversacionalStorage.Application.Dtos.Base.Response.Common;
using ChatbotConversacionalStorage.Application.Helper.Settings;
using ChatbotConversacionalStorage.Domain.Interfaces.Repository.BlackListToken;
using ChatbotConversacionalStorage.Infrastructure.Repositorys.Base;
using Microsoft.Extensions.Options;
#endregion REFERENCES

namespace ChatbotConversacionalStorage.Infrastructure.Repositorys.BlackList
{
    public class BlackListTokenRepository : HttpClientBase<object>, IBlackListTokenRepository
    {
        #region ATRIBUTTES
        private readonly HttpRumtimeSettings _setings;
        #endregion

        #region CONSTRUCTORES
        public BlackListTokenRepository(IOptions<HttpRumtimeSettings> setings)
        {
            _setings = setings.Value;
        }
        #endregion

        #region VALIDATE IF BLACK LIST TOKEN
        public async Task<bool> CheckTokenInBlackList(string token)
        {
            string url = string.Concat(_setings.SecurityApiUrl, _setings.SecurityBlackList);

            var result = await this.PostAsync<BaseResponseDto>(url, data: token);

            return bool.TryParse(result.JsonObject.ToString(), out bool isInBlackList) ? isInBlackList : true;
        }
        #endregion VALIDATE IF BLACK LIST TOKEN
    }
}
