using IceCoffee.Mediator;
using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Notifications;
using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.Constants;
using LSTY.Sdtd.ServerAdmin.Shared.Helpers;
using StreamJsonRpc;
using System.Diagnostics.CodeAnalysis;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Clients
{
    internal class JsonRpcClient : IRpcClient
    {
        public bool IsConnected => _currentRpc != null && _currentRpc.IsDisposed == false;

        private readonly CancellationTokenSource _cts;
        private readonly Task _mainLoopTask;
        private Dictionary<Type, IProxy>? _proxies;
        private JsonRpc? _currentRpc = null;

        private readonly RpcClientConfig _rpcClientConfig;
        private readonly IMediator _mediator;
        private readonly ICustomLogger _logger;

        public JsonRpcClient(
            RpcClientConfig rpcClientConfig,
            IMediator mediator,
            ICustomLoggerFactory customLoggerFactory)
        {
            _rpcClientConfig = rpcClientConfig;
            _mediator = mediator;
            _logger = customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.JsonRpcClient, _rpcClientConfig.Id);

            _cts = new CancellationTokenSource();
            _mainLoopTask = MaintainConnectionAsync();
        }

        public RpcClientConfig Config => _rpcClientConfig;

        private X509Certificate2 LoadeCertificate()
        {
            try
            {
                return X509CertificateLoader.LoadPkcs12(_rpcClientConfig.PfxFile, _rpcClientConfig.PfxPassword);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load PFX file.", ex);
            }
        }

        private async Task<IJsonRpcMessageHandler> GetJsonRpcMessageHandler(TcpClient tcpClient)
        {
            using var certificate = LoadeCertificate();
            var sslStream = new SslStream(tcpClient.GetStream(), false, (sender, cert, chain, errors) =>
            {
                if (cert is X509Certificate2 x509Certificate2 && x509Certificate2.Thumbprint == certificate.Thumbprint)
                {
                    return true;
                }

                return false;
            });

            await _logger.LogInformationAsync("Authenticating SSL stream...");
            var options = new SslClientAuthenticationOptions()
            {
                TargetHost = Common.CompanyName,
                ClientCertificates = new X509CertificateCollection() { certificate },
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
                EncryptionPolicy = EncryptionPolicy.RequireEncryption,
            };

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Common.SslHandshakeTimeout));
            try
            {
                await sslStream.AuthenticateAsClientAsync(options, cts.Token);
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested)
            {
                throw new TimeoutException("SSL handshake timed out.");
            }

            await _logger.LogInformationAsync($"SSL handshake completed with {tcpClient.Client.RemoteEndPoint}");

            return new LengthHeaderMessageHandler(sslStream, sslStream, MessagePackFormatterHelper.Create());
        }

        private async Task OnConnected(TcpClient tcpClient, IReadOnlyDictionary<Type, IProxy> proxies)
        {
            await _mediator.Publish(new GameServerConnected()
            {
                GameServerId = _rpcClientConfig.Id,
                RpcProxies = proxies
            });

            await _logger.LogInformationAsync($"Connected and listening for RPC with {tcpClient.Client.RemoteEndPoint}");
        }

        private async Task OnDisconnected(Exception? exception)
        {
            await _mediator.Publish(new GameServerDisconnected()
            {
                GameServerId = _rpcClientConfig.Id,
            });

            if (exception == null)
            {
                await _logger.LogInformationAsync("Disconnected from RPC server.");
            }
            else if(exception is OperationCanceledException or SocketException or IOException or TimeoutException)
            {
                await _logger.LogWarningAsync(exception, "Disconnected from RPC server.");
            }
            else
            {
                await _logger.LogErrorAsync(exception, "Disconnected from RPC server.");
            }
        }

        private async Task MaintainConnectionAsync()
        {
            // Use exponential backoff strategy for reconnection
            var minDelay = TimeSpan.FromSeconds(5);
            var maxDelay = TimeSpan.FromMinutes(5);
            var currentDelay = minDelay;
   
            while (_cts.IsCancellationRequested == false)
            {
                Exception? disconnectionException = null;
                try
                {
                    await _logger.LogInformationAsync($"Try connecting to RPC server at {_rpcClientConfig.Host}:{_rpcClientConfig.Port}...");

                    using var tcpClient = new TcpClient();
                    await tcpClient.ConnectAsync(_rpcClientConfig.Host, _rpcClientConfig.Port, _cts.Token);

                    var jsonRpcMessageHandler = await GetJsonRpcMessageHandler(tcpClient);
                    _currentRpc = new JsonRpc(jsonRpcMessageHandler);

                    _proxies = CreateProxies(_currentRpc);

                    _currentRpc.StartListening();

                    currentDelay = minDelay;
                    await OnConnected(tcpClient, _proxies);
                    await _currentRpc.Completion;
                }
                catch (OperationCanceledException ex)
                {
                    disconnectionException = ex;
                    break;
                }
                catch (Exception ex) when (ex is SocketException or IOException or TimeoutException)
                {
                    disconnectionException = ex;
                }
                catch (Exception ex)
                {
                    disconnectionException = ex;
                    break;
                }
                finally
                {
                    _proxies = null;

                    _currentRpc?.Dispose();
                    _currentRpc = null;

                    await OnDisconnected(disconnectionException);
                }

                if (_cts.IsCancellationRequested == false)
                {
                    await _logger.LogInformationAsync($"Reconnecting in {currentDelay.TotalSeconds} seconds...");
                    await Task.Delay(currentDelay, _cts.Token);
                    currentDelay = TimeSpan.FromSeconds(Math.Min(currentDelay.TotalSeconds * 2, maxDelay.TotalSeconds));
                }
            }
        }

        private static Dictionary<Type, IProxy> CreateProxies(JsonRpc jsonRpc)
        {
            var proxies = new Dictionary<Type, IProxy>();
            var types = typeof(IProxy).Assembly
                .GetExportedTypes()
                .Where(t => t.IsInterface && typeof(IProxy).IsAssignableFrom(t) && t != typeof(IProxy));

            foreach (var type in types)
            {
                string typeName = type.Name;
                var proxy = jsonRpc.Attach(type, new JsonRpcProxyOptions()
                {
                    MethodNameTransform = name => $"{typeName}.{name}",
                    EventNameTransform = name => $"{typeName}.{name}",
                });
                proxies.Add(type, (IProxy)proxy);
            }

            return proxies;
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (_cts.IsCancellationRequested == false)
            {
                _cts.Cancel();
            }

            try
            {
                _currentRpc?.Dispose();
                _currentRpc = null;
            }
            catch
            {
            }

            try
            {
                await _mainLoopTask;
            }
            catch (OperationCanceledException)
            {
            }

            _cts.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await DisposeAsyncCore();

            // Dispose of unmanaged resources.
            // Dispose(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        public bool TryGetProxy<TProxy>([MaybeNullWhen(false)] out TProxy proxy) where TProxy : class, IProxy
        {
            if( _proxies == null || _proxies.TryGetValue(typeof(TProxy), out var foundProxy) == false)
            {
                proxy = default;
                return false;
            }
            else
            {
                proxy = foundProxy as TProxy;
                return proxy != null;
            }
        }
    }
}
