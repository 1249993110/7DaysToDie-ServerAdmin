using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using System.Security.Cryptography.X509Certificates;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Providers
{
    /// <summary>
    /// Provides the configuration for RPC clients.
    /// </summary>
    public class RpcClientConfigProvider : IRpcClientConfigProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RpcClientConfigProvider"/> class.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="serviceProvider"></param>
        public RpcClientConfigProvider(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        async Task<IEnumerable<RpcClientConfig>> IRpcClientConfigProvider.GetAllAsync(CancellationToken cancellationToken)
        {
            var configs = new List<RpcClientConfig>();
            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                var asyncDocumentSession = scope.ServiceProvider.GetRequiredService<IAsyncDocumentSession>();

                int count = await asyncDocumentSession.Query<GameServerConfig>().CountAsync(cancellationToken);
                if(count == 0)
                {
                    var config = new GameServerConfig()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Ip = "127.0.0.1",
                        Port = 8081,
                        IsEnabled = true,
                        Name = "Test",
                    };
                    
                    await asyncDocumentSession.StoreAsync(config, cancellationToken);

                    using var fileStream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, GameServerConfig.PfxFileName));
                    asyncDocumentSession.Advanced.Attachments.Store(config.Id, GameServerConfig.PfxFileName, fileStream);

                    await asyncDocumentSession.SaveChangesAsync(cancellationToken);
                }

                var gameServerConfigs = asyncDocumentSession.Query<GameServerConfig>().Where(p => p.IsEnabled).AsAsyncEnumerable();
                await foreach (var item in gameServerConfigs)
                {
                    var attachment = await asyncDocumentSession.Advanced.Attachments.GetAsync(item.Id, GameServerConfig.PfxFileName, cancellationToken);
                    using var ms = new MemoryStream();
                    attachment.Stream.CopyTo(ms);
                    byte[] data = ms.ToArray();

                    var rpcClientConfig = new RpcClientConfig()
                    {
                        Id = item.Id,
                        Url = $"tcp://{item.Ip}:{item.Port}",
                        Certificate = X509CertificateLoader.LoadPkcs12(data, item.PfxPassword)
                    };

                    configs.Add(rpcClientConfig);
                }

                return configs;
            }
        }
    }
}
