using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Notifications;
using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.Constants;
using LSTY.Sdtd.ServerAdmin.Shared.Helpers;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace LSTY.Sdtd.ServerAdmin.RpcClient.Clients
{
    /// <summary>
    /// Represents a client for establishing and managing JSON-RPC connections over a secure TCP connection.
    /// </summary>
    /// <remarks>The <see cref="JsonRpcClient"/> class provides functionality to connect to a remote server
    /// using JSON-RPC protocol over a secure SSL/TLS connection. It supports automatic reconnection attempts, proxy
    /// creation for remote procedure calls, and state management for the connection lifecycle. This class is
    /// thread-safe and ensures proper resource cleanup during disconnection or disposal.</remarks>
    public class JsonRpcClient : IRpcClient
    {
        private readonly object _lock = new object();
        private ConnectionState _state = ConnectionState.Disconnected;

        private readonly IHostApplicationLifetime _appLifetime;
        private TcpClient? _client;
        private JsonRpc? _jsonRpc;
        private Dictionary<Type, IProxy>? _proxies;
        private readonly IMediator _mediator;
        private readonly ICustomLogger _logger;

        private readonly Guid _id;
        private readonly string _name;
        private readonly string _url;
        private readonly X509Certificate2 _certificate;
        private int _retryCount = 0;
        private const int _maxRetry = 3;
        private bool _isReconnecting = false;

        public Guid Id => _id;
        public string Name => _name;
        public string Url => _url;
        public ConnectionState State
        {
            get
            {
                lock (_lock)
                {
                    return _state;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRpcClient"/> class with the specified configuration, application lifetime, mediator, and custom logger factory.
        /// </summary>
        /// <remarks>This constructor initializes the <see cref="JsonRpcClient"/> with the provided
        /// dependencies and configuration.  The <paramref name="rpcClientConfig"/> parameter is used to set the
        /// client's unique identifier, name, URL, and certificate. The <paramref name="customLoggerFactory"/> is used
        /// to create a logger specific to the <see cref="JsonRpcClient"/> module.</remarks>
        /// <param name="rpcClientConfig">The configuration settings for the RPC client, including its ID, name, URL, and certificate.</param>
        /// <param name="appLifetime">The application lifetime instance used to monitor application start and shutdown events.</param>
        /// <param name="mediator">The mediator instance used for handling inter-component communication within the application.</param>
        /// <param name="customLoggerFactory">The custom logger factory used to create a specialized logger for the <see cref="JsonRpcClient"/>.</param>
        public JsonRpcClient(
            RpcClientConfig rpcClientConfig, 
            IHostApplicationLifetime appLifetime, 
            IMediator mediator,
            ICustomLoggerFactory customLoggerFactory)
        {
            _id = rpcClientConfig.Id;
            _name = rpcClientConfig.Name;
            _url = rpcClientConfig.Url;
            _certificate = rpcClientConfig.Certificate;
            _logger = customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.JsonRpcClient, _id);
            _appLifetime = appLifetime;
            _mediator = mediator;
        }

        private async Task TryReconnectAsync()
        {
            if(_appLifetime.ApplicationStopping.IsCancellationRequested || _appLifetime.ApplicationStopped.IsCancellationRequested)
            {
                return;
            }

            if (Interlocked.Exchange(ref _isReconnecting, true) == true)
            {
                return;
            }

            try
            {
                while (_retryCount < _maxRetry)
                {
                    _retryCount++;

                    await Task.Delay(5000);

                    if(State == ConnectionState.Connecting || State == ConnectionState.Connected)
                    {
                        await _logger.LogInformationAsync("Already connected or connecting. No need to reconnect.");
                        return;
                    }

                    await _logger.LogInformationAsync($"Reconnect attempt {_retryCount}...");
                    if (await ConnectAsync())
                    {
                        _retryCount = 0;
                        await _logger.LogInformationAsync("Reconnected successfully.");
                        return;
                    }
                }

                await _logger.LogInformationAsync("Max reconnect attempts reached. Giving up.");
            }
            finally
            {
                Interlocked.Exchange(ref _isReconnecting, false);
            }
        }

        private async void OnDisconnected(object? sender, JsonRpcDisconnectedEventArgs args)
        {
            _jsonRpc?.Dispose();
            _jsonRpc = null;
            _client?.Dispose();
            _client = null;

            lock (_lock)
            {
                _state = ConnectionState.Disconnected;
            }

            await _logger.LogInformationAsync("Disconnected from server.");

            if (args.Exception != null)
            {
                if (args.Exception.InnerException is SocketException ex)
                {
                    var error = ex.SocketErrorCode;
                    // Skip disconnect errors
                    if ((error == SocketError.ConnectionAborted)
                        || (error == SocketError.ConnectionRefused)
                        || (error == SocketError.ConnectionReset)
                        || (error == SocketError.OperationAborted)
                        || (error == SocketError.Shutdown))
                        goto Reconnect;
                }

                await _logger.LogInformationAsync(args.Exception, "Disconnection error.");
            }

        Reconnect:
            await TryReconnectAsync();
        }

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                lock (_lock)
                {
                    if (_state != ConnectionState.Disconnected)
                    {
                        throw new InvalidOperationException($"Connect is only allowed in Disconnected state, current state: {_state}");
                    }

                    _state = ConnectionState.Connecting;
                }

                await _logger.LogInformationAsync($"Connecting to {_url}...");

                var uri = new Uri(_url);
                _client = new TcpClient();

                await _client.ConnectAsync(uri.Host, uri.Port, cancellationToken);

                var sslStream = new SslStream(_client.GetStream(), false, (sender, cert, chain, errors) =>
                {
                    if (cert is X509Certificate2 x509Certificate2 && x509Certificate2.Thumbprint == _certificate.Thumbprint)
                    {
                        return true;
                    }

                    return false;
                });

                await _logger.LogInformationAsync("Authenticating SSL stream...");
                var options = new SslClientAuthenticationOptions()
                {
                    TargetHost = Common.CompanyName,
                    ClientCertificates = new X509CertificateCollection { _certificate },
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

                await _logger.LogInformationAsync($"SSL handshake completed with {_client.Client.RemoteEndPoint}");

                var headerDelimitedMessageHandler  = new LengthHeaderMessageHandler(sslStream, sslStream, MessagePackFormatterHelper.Create());
                _jsonRpc = new JsonRpc(headerDelimitedMessageHandler);
                _jsonRpc.Disconnected += OnDisconnected;

                _proxies = CreateProxies(_jsonRpc);

                _jsonRpc.StartListening();

                await _logger.LogInformationAsync($"Connected and listening for RPC with {_client.Client.RemoteEndPoint}");

                lock (_lock)
                {
                    _state = ConnectionState.Connected;
                }

                await _mediator.Publish(new GameServerConnected() 
                {
                    GameServerId = _id,
                    RpcProxies = _proxies 
                }, cancellationToken);

                _ = _jsonRpc.Completion.ContinueWith(async task =>
                {
                    await _mediator.Publish(new GameServerDisconnected()
                    {
                        GameServerId = _id,
                    });
                }, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _client?.Dispose();
                _client = null;

                lock (_lock)
                {
                    _state = ConnectionState.Disconnected;
                }

                await _logger.LogErrorAsync(ex, "Error in JsonRpcClient.ConnectAsync");

                return false;
            }
        }

        public async Task DisconnectAsync()
        {
            lock (_lock)
            {
                if (_state == ConnectionState.Disconnected
                    || _state == ConnectionState.Disconnecting
                    || _state == ConnectionState.Disposed
                    || _state == ConnectionState.Disposing)
                {
                    return;
                }

                if (_state == ConnectionState.Connecting)
                {
                    throw new InvalidOperationException("Cannot disconnect client while connecting.");
                }

                _state = ConnectionState.Disconnecting;
            }

            await _logger.LogInformationAsync("Disconnecting...");

            try
            {
                try
                {
                    _client?.Client.Shutdown(SocketShutdown.Both);

                }
                catch (SocketException)
                {
                }

                await Task.Delay(20);

                _client?.Close();
            }
            catch (ObjectDisposedException)
            {
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_state == ConnectionState.Disposed || _state == ConnectionState.Disposing)
                {
                    return;
                }

                _state = ConnectionState.Disposing;
            }

            _jsonRpc?.Dispose();
            _jsonRpc = null;
            _client?.Dispose();
            _client = null;
            _certificate.Dispose();

            lock (_lock)
            {
                _state = ConnectionState.Disposed;
            }
        }

        public bool TryGetProxy<TProxy>(out TProxy? proxy) where TProxy : class, IProxy
        {
            if(_proxies == null)
            {
                proxy = default;
                return false;
            }

            bool result = _proxies.TryGetValue(typeof(TProxy), out var _proxy);
            proxy = _proxy as TProxy;
            return result;
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
    }
}
