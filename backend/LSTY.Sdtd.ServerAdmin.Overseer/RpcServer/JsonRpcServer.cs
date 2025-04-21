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
        private readonly object _lock = new object();
        private ServerState _state = ServerState.Stopped;

        private readonly ConcurrentDictionary<int, TcpClient> _clients = new();
        private CancellationTokenSource? _cts;
        private Task? _acceptTask;

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

        public ServerState State
        {
            get
            {
                lock (_lock)
                {
                    return _state;
                }
            }
        }

        public JsonRpcServer(int port, X509Certificate2 sslCertificate, IReadOnlyDictionary<Type, IProxy> proxies)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
            _tcpListener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            _tcpListener.Server.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);
            
            _sslCertificate = sslCertificate;
            _proxies = proxies;
        }

        public void Start()
        {
            lock (_lock)
            {
                if (_state != ServerState.Stopped)
                {
                    throw new InvalidOperationException($"Start is only allowed in Stopped state, current state: {_state}");
                }

                _state = ServerState.Starting;
            }

            _cts = new CancellationTokenSource();

            try
            {
                _tcpListener.Start();
            }
            catch
            {
                _cts?.Dispose();
                _cts = null;
                lock (_lock)
                {
                    _state = ServerState.Stopped;
                }
                throw;
            }

            _acceptTask = AcceptClientsAsync(_cts.Token);
            lock (_lock)
            {
                _state = ServerState.Started;
            }

            CustomLogger.Info($"Json RPC Server started on port {((IPEndPoint)_tcpListener.LocalEndpoint).Port}");
        }

        public void UpdateSslCertificate(X509Certificate2 certificate)
        {
            _sslCertificate.Dispose();
            _sslCertificate = certificate;
        }

        private async Task AcceptClientsAsync(CancellationToken token)
        {
            try
            {
                while (token.IsCancellationRequested == false)
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

        public void Stop()
        {
            lock (_lock)
            {
                if (_state == ServerState.Stopped 
                    || _state == ServerState.Stopping 
                    || _state == ServerState.Disposed
                    || _state == ServerState.Disposing)
                {
                    return;
                }

                if (_state == ServerState.Starting)
                {
                    throw new InvalidOperationException("Cannot stop server while starting.");
                }

                _state = ServerState.Stopping;
            }

            StopCore();

            lock (_lock)
            {
                _state = ServerState.Stopped;
            }
        }

        private void StopCore()
        {
            try
            {
                _tcpListener.Stop();
            }
            catch
            {
            }

            try
            {
                _cts?.Cancel();
            }
            finally
            {
                _cts?.Dispose();
                _cts = null;
            }

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

            try
            {
                _acceptTask?.Wait();
            }
            finally
            {
                _acceptTask?.Dispose();
                _acceptTask = null;
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if (_state == ServerState.Disposed || _state == ServerState.Disposing)
                {
                    return;
                }

                _state = ServerState.Disposing;
            }

            StopCore();

            _tcpListener.Server.Dispose();
            _sslCertificate.Dispose();

            lock (_lock)
            {
                _state = ServerState.Disposed;
            }
        }
    }
}
