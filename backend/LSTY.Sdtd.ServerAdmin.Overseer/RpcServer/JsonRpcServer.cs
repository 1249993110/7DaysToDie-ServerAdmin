using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.Constants;
using LSTY.Sdtd.ServerAdmin.Shared.Helpers;
using Microsoft;
using StreamJsonRpc;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace LSTY.Sdtd.ServerAdmin.Overseer.RpcServer
{
    public sealed class JsonRpcServer : IDisposable
    {
        private readonly ConcurrentDictionary<int, TcpClient> _clients = new();
        private CancellationTokenSource _cts;
        private Task _acceptTask;

        private readonly TcpListener _tcpListener;
        private X509Certificate2 _sslCertificate;
        private readonly IReadOnlyDictionary<Type, IProxy> _proxies;

        public IProxy GetProxy<TProxy>() where TProxy : IProxy
        {
            if (_proxies.TryGetValue(typeof(TProxy), out var proxy))
            {
                return proxy;
            }
            throw new InvalidOperationException($"Proxy of type {typeof(TProxy).Name} not found.");
        }

        public JsonRpcServer(int port, X509Certificate2 sslCertificate, IReadOnlyDictionary<Type, IProxy> proxies)
        {
            _sslCertificate = sslCertificate;
            _proxies = proxies;
            _cts = new CancellationTokenSource();

            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _tcpListener.Server.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            _tcpListener.Start();
            CustomLogger.Info($"Json RPC Server started on port {((IPEndPoint)_tcpListener.LocalEndpoint).Port}");

            _acceptTask = AcceptClientsAsync();
        }

        public void UpdateSslCertificate(X509Certificate2 certificate)
        {
            _sslCertificate.Dispose();
            _sslCertificate = certificate;
        }

        private async Task AcceptClientsAsync()
        {
            try
            {
                while (_cts.IsCancellationRequested == false)
                {
                    var tcpClient = await _tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                    ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(HandleClientAsync), tcpClient);
                }
            }
            //catch (OperationCanceledException)
            //{
            //}
            catch (ObjectDisposedException)
            {
                // Ignore
            }
            catch (SocketException ex)
            {
                var error = ex.SocketErrorCode;
                // Skip disconnect errors
                if (error == SocketError.ConnectionAborted
                    || error == SocketError.ConnectionRefused
                    || error == SocketError.ConnectionReset
                    || error == SocketError.OperationAborted
                    || error == SocketError.Shutdown)
                    return;
                CustomLogger.Error(ex, "Error in JsonRpcServer.AcceptClientsAsync");
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in JsonRpcServer.AcceptClientsAsync");
            }
        }

        private async void HandleClientAsync(object? state)
        {
            var tcpClient = (TcpClient)state!;
            var socket = tcpClient.Client;
            int clientId = tcpClient.Client.Handle.ToInt32();
            _clients[clientId] = tcpClient;

            var remoteEndPoint = socket.RemoteEndPoint as IPEndPoint;
            CustomLogger.Info($"New client connection attempt from {remoteEndPoint}");
            JsonRpc? jsonRpc = null;
            try
            {
                var sslCertificate = _sslCertificate;
                using var networkStream = tcpClient.GetStream();
                using var sslStream = new SslStream(networkStream, false, (sender, cert, chain, errors) =>
                {
                    if (cert is X509Certificate2 x509Certificate2 && x509Certificate2.Thumbprint == sslCertificate.Thumbprint)
                    {
                        return true;
                    }

                    return false;
                });

                var authTask = sslStream.AuthenticateAsServerAsync(sslCertificate, clientCertificateRequired: true, SslProtocols.Tls12 | SslProtocols.Tls13, checkCertificateRevocation: false);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(Common.SslHandshakeTimeout));

                if (await Task.WhenAny(authTask, timeoutTask) == timeoutTask)
                {
                    throw new TimeoutException("SSL handshake timed out.");
                }

                await authTask;

                CustomLogger.Info($"SSL handshake completed with {remoteEndPoint}");

                var headerDelimitedMessageHandler = new LengthHeaderMessageHandler(sslStream, sslStream, MessagePackFormatterHelper.Create());
                jsonRpc = new JsonRpc(headerDelimitedMessageHandler);

                foreach (var item in _proxies)
                {
                    jsonRpc.AddLocalRpcTarget(item.Key, item.Value, new JsonRpcTargetOptions()
                    {
                        MethodNameTransform = name => $"{item.Key.Name}.{name}",
                        EventNameTransform = name => $"{item.Key.Name}.{name}",
                    });
                }

                jsonRpc.StartListening();

                CustomLogger.Info($"Listening for RPC with {remoteEndPoint}");

                await jsonRpc.Completion;
            }
            catch (ObjectDisposedException)
            {
                // Ignore
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, $"Error in JsonRpcServer.HandleClientAsync: {remoteEndPoint}");
            }
            finally
            {
                jsonRpc?.Dispose();
                tcpClient.Dispose();
                _clients.TryRemove(clientId, out _);
                CustomLogger.Info($"RPC Client disconnected: {remoteEndPoint}");
            }
        }

        public void Dispose()
        {
            if (_cts.IsCancellationRequested == false)
            {
                _cts.Cancel();
            }

            try
            {
                _tcpListener.Stop();

                foreach (var client in _clients.Values)
                {
                    try
                    {
                        client.Client.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }

                    try
                    {
                        client.Close();
                        client.Dispose();
                    }
                    catch
                    {
                    }
                }
                _clients.Clear();
            }
            finally
            {
                _tcpListener.Server.Dispose();
            }

            try
            {
                _acceptTask.Wait();
            }
            catch /*(OperationCanceledException)*/
            {
            }

            _cts.Dispose();

            _sslCertificate.Dispose();
        }
    }
}
