using Microsoft.Extensions.Configuration;

namespace LSTY.Sdtd.ServerAdmin.Overseer.Config
{
    internal static class AppConfig
    {
        public static AppSettings Settings { get; private set; } = null!;

        public static AppSettings Load(string modPath)
        {
            try
            {
                string appConfigPath = Path.Combine(modPath, "Config", "appsettings.json");

                if (File.Exists(appConfigPath) == false)
                {
                    throw new FileNotFoundException($"appsettings.json not found at {appConfigPath}");
                }

                var builder = new ConfigurationBuilder()
                    .AddJsonFile(appConfigPath, optional: false, reloadOnChange: false);

                var configuration = builder.Build();
                var appSettings = configuration.Get<AppSettings>();

                if (appSettings == null)
                {
                    throw new ArgumentNullException(nameof(appSettings));
                }

                Settings = appSettings;

                return appSettings;
            }
            catch (Exception ex)
            {
                throw new Exception("Load appsettings failed.", ex);
            }
        }
    }
}
