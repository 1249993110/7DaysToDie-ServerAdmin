using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Core;
using LSTY.Sdtd.ServerAdmin.RpcClient.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRpcClientManager<TRpcClientConfigProvider>(this IServiceCollection services) where TRpcClientConfigProvider : class, IRpcClientConfigProvider
        {
            services.AddSingleton<IRpcClientConfigProvider, TRpcClientConfigProvider>();
            services.AddSingleton<RpcClientManager>();
            services.AddHostedService<RpcClientHostedService>();
            return services;
        }
    }
}
