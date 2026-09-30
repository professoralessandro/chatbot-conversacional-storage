namespace ChatbotConversacionalStorage.Domain.Interfaces.Services.BlackListToken
{
    public interface IBlackListTokenService
    {
        #region CHECK BLACK LIST TOKEN
        Task<bool> CheckTokenInBlackList(string token);
        #endregion CHECK BLACK LIST TOKEN
    }
}
