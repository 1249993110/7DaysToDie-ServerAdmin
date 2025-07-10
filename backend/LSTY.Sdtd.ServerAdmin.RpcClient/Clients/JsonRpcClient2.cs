//using IceCoffee.Mediator;
//using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
//using LSTY.Sdtd.ServerAdmin.Data.Notifications;
//using LSTY.Sdtd.ServerAdmin.RpcClient.Abstractions;
//using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
//using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
//using LSTY.Sdtd.ServerAdmin.Shared.Constants;
//using LSTY.Sdtd.ServerAdmin.Shared.Helpers;
//using Microsoft.Extensions.Hosting;
//using StreamJsonRpc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Security;
//using System.Net.Sockets;
//using System.Security.Authentication;
//using System.Security.Cryptography;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;

//namespace LSTY.Sdtd.ServerAdmin.RpcClient.Clients
//{
//    internal class JsonRpcClient : JsonRpcClientBase
//    {
//        private readonly RpcClientConfig _rpcClientConfig;
//        private readonly IHostApplicationLifetime _appLifetime;
//        private readonly IMediator _mediator;
//        private readonly ICustomLogger _logger;

//        public JsonRpcClient(
//            RpcClientConfig rpcClientConfig,
//            IHostApplicationLifetime appLifetime,
//            IMediator mediator,
//            ICustomLoggerFactory customLoggerFactory) : base(rpcClientConfig.Host, rpcClientConfig.Port)
//        {
//            _rpcClientConfig = rpcClientConfig;
//            _appLifetime = appLifetime;
//            _mediator = mediator;
//            _logger = customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.JsonRpcClient, _rpcClientConfig.Id);

//        }

//        public RpcClientConfig Config => _rpcClientConfig;

//        protected override async Task<IJsonRpcMessageHandler> GetJsonRpcMessageHandler(TcpClient tcpClient)
//        {
//            var sslStream = new SslStream(tcpClient.GetStream(), false, (sender, cert, chain, errors) =>
//            {
//                if (cert is X509Certificate2 x509Certificate2 && x509Certificate2.Thumbprint == _rpcClientConfig.Certificate.Thumbprint)
//                {
//                    return true;
//                }

//                return false;
//            });

//            await _logger.LogInformationAsync("Authenticating SSL stream...");
//            var options = new SslClientAuthenticationOptions()
//            {
//                TargetHost = Common.CompanyName,
//                ClientCertificates = new X509CertificateCollection { _rpcClientConfig.Certificate },
//                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
//                CertificateRevocationCheckMode = X509RevocationMode.NoCheck,
//                EncryptionPolicy = EncryptionPolicy.RequireEncryption,
//            };

//            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Common.SslHandshakeTimeout));
//            try
//            {
//                await sslStream.AuthenticateAsClientAsync(options, cts.Token);
//            }
//            catch (OperationCanceledException) when (cts.IsCancellationRequested)
//            {
//                throw new TimeoutException("SSL handshake timed out.");
//            }

//            await _logger.LogInformationAsync($"SSL handshake completed with {tcpClient.Client.RemoteEndPoint}");

//            return new LengthHeaderMessageHandler(sslStream, sslStream, MessagePackFormatterHelper.Create());
//        }

//        protected override async Task OnConnected(TcpClient tcpClient, IReadOnlyDictionary<Type, IProxy> proxies)
//        {
//            await _logger.LogInformationAsync($"Connected and listening for RPC with {tcpClient.Client.RemoteEndPoint}");
//            await _mediator.Publish(new GameServerConnected()
//            {
//                GameServerId = _rpcClientConfig.Id,
//                RpcProxies = proxies
//            });
//        }

//        //protected override async Task OnDisconnected(JsonRpcDisconnectedEventArgs args)
//        //{
//        //    await _logger.LogInformationAsync($"Disconnected from RPC server: {args.Reason}");
//        //    await _mediator.Publish(new GameServerDisconnected()
//        //    {
//        //        GameServerId = _rpcClientConfig.Id,
//        //    });

//        //    if (_appLifetime.ApplicationStopping.IsCancellationRequested == false && _appLifetime.ApplicationStopped.IsCancellationRequested == false)
//        //    {

//        //    }
//        //}

//        protected override async ValueTask DisposeAsyncCore()
//        {
//            await base.DisposeAsyncCore();
//            _rpcClientConfig.Dispose();
//        }
//    }
//}
