using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin
{
    internal static class Utils
    {
        public static string FormatCommandArgs(string? args)
        {
            if (args == null || args.Length == 0)
            {
                return string.Empty;
            }

            if (args[0] == '\"' && args[^1] == '\"')
            {
                return args;
            }

            if (args.Contains('\"'))
            {
                throw new Exception("Parameters should not contain the character double quotes.");
            }

            if (args.Contains(' '))
            {
                return $"\"{args}\"";
            }

            return args;
        }

        public static Task<IEnumerable<string>> ExecuteConsoleCommandAsync(string command, bool inMainThread = true)
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

        public static Task<IEnumerable<string>> SendGlobalMessageAsync(GlobalMessage globalMessage)
        {
            string command = string.Format("ty-say {0} {1}",
                FormatCommandArgs(globalMessage.Message),
                FormatCommandArgs(globalMessage.SenderName));
            return ExecuteConsoleCommandAsync(command);
        }

        public static Task<IEnumerable<string>> SendPrivateMessageAsync(PrivateMessage privateMessage)
        {
            string command = string.Format("ty-pm {0} {1} {2}",
                FormatCommandArgs(privateMessage.TargetPlayerIdOrName),
                FormatCommandArgs(privateMessage.Message),
                FormatCommandArgs(privateMessage.SenderName));
            return ExecuteConsoleCommandAsync(command);
        }

        public static bool TryGetBlockValue(string blockIdOrName, out BlockValue blockValue)
        {
            if (int.TryParse(blockIdOrName, out var blockId))
            {
                foreach (Block block in Block.list)
                {
                    if (block.blockID == blockId)
                    {
                        blockValue = Block.GetBlockValue(block.GetBlockName(), false);
                        return true;
                    }
                }
            }
            else
            {
                foreach (Block block in Block.list)
                {
                    string blockName = block.GetBlockName();
                    if (string.Equals(blockName, blockIdOrName, StringComparison.OrdinalIgnoreCase))
                    {
                        blockValue = Block.GetBlockValue(block.GetBlockName(), false);
                        return true;
                    }
                }
            }

            blockValue = BlockValue.Air;
            return false;
        }
    }
}
