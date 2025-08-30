using LSTY.Sdtd.ServerAdmin.Config;
using Microsoft.Owin;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LSTY.Sdtd.ServerAdmin.Commands
{
    /// <summary>
    /// Represents a command to restart the server.
    /// </summary>
    public class RestartServer : ConsoleCmdBase
    {
        /// <inheritdoc />
        public override string getDescription()
        {
            return "Restart server, optional parameter [{delay}] and [-f/force].";
        }

        /// <inheritdoc />
        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. ty-rs [-f/force]\n" +
                "  2. ty-rs {delay} [-f/force]\n" +
                "1. Restart server by shutdown or force restart by kill process\n" +
                "2. Delay for a specified number of seconds before restarting";
        }

        /// <inheritdoc />
        public override string[] getCommands()
        {
            return new[] { "ty-RestartServer", "ty-rs" };
        }

        /// <inheritdoc />
        public override async void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            Log("Server is restarting..., please wait.");

            if (_isRestarting)
            {
                return;
            }

            bool force = ContainsCaseInsensitive(args, "-f") || ContainsCaseInsensitive(args, "force");
            if (force)
            {
                args.RemoveAll(i => string.Equals(i, "-f", StringComparison.OrdinalIgnoreCase) || string.Equals(i, "force", StringComparison.OrdinalIgnoreCase));
            }

            if (args.Count > 0 && int.TryParse(args[0], out int delay))
            {
                for (int i = 0; i < delay; i++)
                {
                    await Task.Delay(1000);
                    Log($"{delay - i}");
                }
            }

            if (force)
            {
                PrepareRestart(true);
            }
            else
            {
                PrepareRestart(false);
            }
        }

        private static volatile bool _isRestarting;
        /// <summary>
        /// Gets a value indicating whether the server is restarting.
        /// </summary>
        public static bool IsRestarting => _isRestarting;

        /// <summary>
        /// Prepares the server for restart.
        /// </summary>
        /// <param name="force">Indicates whether to force restart the server.</param>
        public static void PrepareRestart(bool force = false)
        {
            SdtdConsole.Instance.ExecuteSync("sa", ModMain.CmdExecuteDelegate);

            _isRestarting = true;

            if (force)
            {
                Restart();
            }
            else
            {
                SdtdConsole.Instance.ExecuteSync("shutdown", ModMain.CmdExecuteDelegate);
            }
        }

        private static void Restart()
        {
            string? scriptName = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                scriptName = "restart-windows.bat";
                string serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startdedicated.bat");
                string path = Path.Combine(ModMain.ModInstance.Path, scriptName);
                Process.Start(path, string.Format("{0} \"{1}\"", Process.GetCurrentProcess().Id, serverPath));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                scriptName = "restart-linux.sh";
                string path = Path.Combine(ModMain.ModInstance.Path, scriptName);
                Process.Start("sh", $"{path} {Process.GetCurrentProcess().Id} {AppConfig.Settings.ServerConfigFile}");
            }
        }

        /// <summary>
        /// Handles the game shutdown event.
        /// </summary>
        private static void OnGameShutdown(object sender, EventArgs e)
        {
            if (_isRestarting)
            {
                Restart();
            }
        }

        static RestartServer()
        {
            ModEventHub.Instance.GameShutdown += OnGameShutdown;
        }
    }
}