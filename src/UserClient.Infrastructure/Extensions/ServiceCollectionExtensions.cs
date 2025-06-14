using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserClient.Core.Interfaces;
using UserClient.Infrastructure;
using UserClient.Infrastructure.Policies;
using UserClient.Infrastructure.Services;

namespace UserClient.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUserClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
            services.AddMemoryCache();
            services.AddHttpClient<IExternalUserService,ExternalUserService>()
                    .AddPolicyHandler(UserClientPolicy.GetRetryPolicy());

            return services;
        }
    }
}
