using LSTY.Sdtd.ServerAdmin.Shared.Models;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;
using System.Text;

namespace LSTY.Sdtd.ServerAdmin.Overseer.RpcServer.Proxies
{
    public class GameManageProxy : IGameManageProxy
    {
        /// <summary>
        /// Executes a console command and returns the result.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="inMainThread"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>> ExecuteConsoleCommandAsync(string command, bool inMainThread = true)
        {
            if (inMainThread && ThreadManager.IsMainThread() == false)
            {
                IEnumerable<string> executeResult = Enumerable.Empty<string>();
                ModMain.MainThreadSyncContext.Send((state) =>
                {
                    executeResult = SdtdConsole.Instance.ExecuteSync((string)state, ModMain.CmdExecuteDelegate);
                }, command);

                return Task.FromResult(executeResult);
            }
            else
            {
                IEnumerable<string> executeResult = SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate);
                return Task.FromResult(executeResult);
            }
        }

        public Task<string> GetWelcome()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("*** Connected with 7DTD server.");
            stringBuilder.AppendLine("*** Server version: " + global::Constants.cVersionInformation.LongString + " Compatibility Version: " + global::Constants.cVersionInformation.LongStringNoBuild);
            stringBuilder.AppendLine("*** Dedicated server only build");
            stringBuilder.AppendLine(string.Empty);
            stringBuilder.AppendLine("Server IP:   " + (string.IsNullOrEmpty(GamePrefs.GetString(EnumGamePrefs.ServerIP)) ? "Any" : GamePrefs.GetString(EnumGamePrefs.ServerIP)));
            stringBuilder.AppendLine("Server port: " + GamePrefs.GetInt(EnumGamePrefs.ServerPort).ToString());
            stringBuilder.AppendLine("Max players: " + GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount).ToString());
            stringBuilder.AppendLine("Game mode:   " + GamePrefs.GetString(EnumGamePrefs.GameMode));
            stringBuilder.AppendLine("World:       " + GamePrefs.GetString(EnumGamePrefs.GameWorld));
            stringBuilder.AppendLine("Game name:   " + GamePrefs.GetString(EnumGamePrefs.GameName));
            stringBuilder.AppendLine("Difficulty:  " + GamePrefs.GetInt(EnumGamePrefs.GameDifficulty).ToString());
            stringBuilder.AppendLine(string.Empty);
            stringBuilder.AppendLine("Press 'help' to get a list of all commands.");
            stringBuilder.AppendLine(string.Empty);

            return Task.FromResult(stringBuilder.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalMessage"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>> SendGlobalMessageAsync(GlobalMessage globalMessage)
        {
            string command = string.Format("ty-say {0} {1}",
                Shared.Utils.FormatCommandArgs(globalMessage.Message),
                Shared.Utils.FormatCommandArgs(globalMessage.SenderName));
            return ExecuteConsoleCommandAsync(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="privateMessage"></param>
        /// <returns></returns>
        public Task<IEnumerable<string>> SendPrivateMessageAsync(PrivateMessage privateMessage)
        {
            string command = string.Format("ty-pm {0} {1} {2}",
                Shared.Utils.FormatCommandArgs(privateMessage.TargetPlayerIdOrName),
                Shared.Utils.FormatCommandArgs(privateMessage.Message),
                Shared.Utils.FormatCommandArgs(privateMessage.SenderName));
            return ExecuteConsoleCommandAsync(command);
        }
    }
}
