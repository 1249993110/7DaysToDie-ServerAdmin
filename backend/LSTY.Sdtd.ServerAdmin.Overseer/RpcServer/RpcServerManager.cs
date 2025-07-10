using LSTY.Sdtd.ServerAdmin.Overseer.Helpers;
using LSTY.Sdtd.ServerAdmin.Overseer.RpcServer.Proxies;
using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.Constants;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace LSTY.Sdtd.ServerAdmin.Overseer.RpcServer
{
    public static class RpcServerManager
    {
        private static JsonRpcServer? _jsonRpcServer;
        private static string _certPath = null!;
        private static string? _certPassword;

        public static JsonRpcServer JsonRpcServer => _jsonRpcServer ?? throw new InvalidOperationException("The RPC server has not been initialized.");
        public static ModEventProxy ModEventProxy => (ModEventProxy)JsonRpcServer.GetProxy<IModEventProxy>();

        public static void Init(int port, string certPath, string? certPassword)
        {
            _certPath = certPath;
            _certPassword = certPassword;

            var certificate = LoadCertificate();
            CustomLogger.Info($"Loaded certificate from {_certPath}.");

            _jsonRpcServer = new JsonRpcServer(port, certificate, CreateProxies());
        }

        private static Dictionary<Type, IProxy> CreateProxies()
        {
            var proxies = new Dictionary<Type, IProxy>();
            var types = Assembly.GetExecutingAssembly()
                .GetExportedTypes()
                .Where(t => t.IsClass && t.IsAbstract == false && typeof(IProxy).IsAssignableFrom(t));
            foreach (var type in types)
            {
                var proxy = (IProxy)Activator.CreateInstance(type);
                proxies.Add(type.GetInterfaces().First(i => i != typeof(IProxy)), proxy);
            }
            return proxies;
        }

        private static X509Certificate2 LoadCertificate()
        {
            try
            {
                if (File.Exists(_certPath) == false)
                {
                    CustomLogger.Info($"Certificate not found at {_certPath}. Generating a new one.");
                    CertificateHelper.GenerateSignedCertificate(_certPath, Common.CompanyName, _certPassword);
                }

                return X509CertificateLoader.LoadPkcs12FromFile(_certPath, _certPassword);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to load certificate.", ex);
            }
        }

        public static void UpdateSslCertificate()
        {
            if (File.Exists(_certPath))
            {
                File.Delete(_certPath);
            }

            var certificate = LoadCertificate();
            JsonRpcServer.UpdateSslCertificate(certificate);
        }

        public static void Dispose(ref ModEvents.SGameShutdownData sGameShutdownData)
        {
            try
            {
                _jsonRpcServer?.Dispose();
                _jsonRpcServer = null;
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Failed to dispose RPC server.");
            }
        }
    }
}
