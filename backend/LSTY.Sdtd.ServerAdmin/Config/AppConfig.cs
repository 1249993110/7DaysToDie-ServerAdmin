using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Runtime;
using System.Text;

namespace LSTY.Sdtd.ServerAdmin.Config
{
    internal static class AppConfig
    {
        public static AppSettings Settings { get; private set; } = Activator.CreateInstance<AppSettings>();
        private static string _productionAppConfigPath = null!;
        private static readonly object _configLock = new object();
        public static event Action<AppSettings, AppSettings>? OnChange;

        public static void Load(string modPath)
        {
            try
            {
                string defaultAppConfigPath = Path.Combine(modPath, "Config", "appsettings.Default.json");
                _productionAppConfigPath = Path.Combine(modPath, "Config", "appsettings.json");

                var builder = new ConfigurationBuilder()
                    .AddJsonFile(defaultAppConfigPath, optional: false, reloadOnChange: false)
                    .AddJsonFile(_productionAppConfigPath, optional: true, reloadOnChange: true);

                var configuration = builder.Build();
                configuration.Bind(Settings);

                ChangeToken.OnChange(
                    () => configuration.GetReloadToken(),
                    () =>
                    {

                        lock (_configLock)
                        {
                            var oldSettings = Settings;
                            var newSettings = Activator.CreateInstance<AppSettings>();
                            configuration.Bind(newSettings);
                            Settings = newSettings;
                            try
                            {
                                OnChange?.Invoke(newSettings, oldSettings);
                            }
                            catch (Exception ex)
                            {
                                CustomLogger.Warn(ex, "An error occurred during configuration reload.");
                            }
                        }
                    }
                );

                if (File.Exists(_productionAppConfigPath) == false)
                {
                    Write();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Load appsettings failed.", ex);
            }
        }

        public static void SaveSettings(AppSettings appSettings)
        {
            try
            {
                Settings = appSettings;
                Write();
            }
            catch (Exception ex)
            {
                throw new Exception("Save appsettings failed.", ex);
            }
        }

        private static void Write()
        {
            string json = JsonConvert.SerializeObject(Settings, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
            });
            lock (_configLock)
            {
                File.WriteAllText(_productionAppConfigPath, json, Encoding.UTF8);
            }
        }
    }
}
