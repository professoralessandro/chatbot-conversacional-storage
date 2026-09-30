namespace ChatbotConversacionalStorage.Domain.Interfaces.Repository.BlackListToken
{
    public interface IBlackListTokenRepository
    {
        #region CHECK BLACK LIST TOKEN
        Task<bool> CheckTokenInBlackList(string token);
        #endregion CHECK BLACK LIST TOKEN
    }
}
