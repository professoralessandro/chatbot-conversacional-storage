#region REFERENCES
using ChatbotConversacionalStorage.Infrastructure.Arquiteture.RepositoryInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using ChatbotConversacionalStorage.Domain.Connector;
using ChatbotConversacionalStorage.Domain.Interfaces.Data;
using ChatbotConversacionalStorage.Domain.Servies.User;
using ChatbotConversacionalStorage.Domain.Interfaces.Common;
using ChatbotConversacionalStorage.Domain.Interfaces.Services.Log;
using ChatbotConversacionalStorage.Domain.Interfaces.Services.FileStorage;
using ChatbotConversacionalStorage.Domain.Servies.FileStorage;
using ChatbotConversacionalStorage.Domain.Interfaces.Services.BlackListToken;
using ChatbotConversacionalStorage.Domain.Servies.BlackListToken;
#endregion

namespace ChatbotConversacionalStorage.Infrastructure.Arquiteture.ServicesInjection
{
    public static class APServicesInjection
    {
        #region ADD SERVICES
        public static IServiceCollection AddServices(this IServiceCollection services)
        {

            return services
                .RemoveAll<IHttpMessageHandlerBuilderFilter>()
                .RegisterServices()
                .RegisterRepositories()
                .AddServiceNoDependency();
        }
        #endregion

        #region REGFISTER SERVICES
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            // DAPPER
            services.AddScoped<APConnector>();
            services.AddTransient<IAPDWork, APWork>();

            // SERVICES
            services.AddScoped<ILogService, LogService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IBlackListTokenService, BlackListTokenService>();

            return services;
        }
        #endregion

        #region ADD SERVICE NO DEPENDENCY
        private static IServiceCollection AddServiceNoDependency(this IServiceCollection services)
        {
            var implementationsType = typeof(APServicesInjection).Assembly.GetTypes()
                .Where(t => typeof(IService).IsAssignableFrom(t) && t.BaseType != null);

            foreach (var item in implementationsType)
            {
                services.AddScoped(item);
            }

            return services;

        }
        #endregion
    }
}
