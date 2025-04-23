using LSTY.Sdtd.ServerAdmin.Shared.Models;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;

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
