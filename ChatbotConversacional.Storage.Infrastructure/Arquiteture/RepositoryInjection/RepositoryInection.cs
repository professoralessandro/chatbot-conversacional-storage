#region REFERENCES
using ChatbotConversacionalStorage.Domain.Interfaces.Repository.BlackListToken;
using ChatbotConversacionalStorage.Domain.Interfaces.Repository.FileStorage;
using ChatbotConversacionalStorage.Domain.Interfaces.Repository.Log;
using ChatbotConversacionalStorage.Infrastructure.Repositorys.BlackList;
using ChatbotConversacionalStorage.Infrastructure.Repositorys.FileStorage;
using ChatbotConversacionalStorage.Infrastructure.Repositorys.Log;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace ChatbotConversacionalStorage.Infrastructure.Arquiteture.RepositoryInjection
{
    public static class RepositoryInection
    {
        #region REGISTER REPOSITORIES
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            return services.
                AddScoped<ILogRepository, LogRepository>().
                AddScoped<IBlackListTokenRepository, BlackListTokenRepository>().
                AddScoped<IFileStorageRepository, FileStorageRepository>();
        }
        #endregion
    }
}
