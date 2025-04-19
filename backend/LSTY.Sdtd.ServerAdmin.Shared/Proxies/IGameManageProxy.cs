using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Shared.Proxies
{
    public interface IGameManageProxy : IProxy
    {
        Task<IEnumerable<string>> ExecuteConsoleCommandAsync(string command, bool inMainThread = true);
        Task<IEnumerable<string>> SendGlobalMessageAsync(GlobalMessage globalMessage);
        Task<IEnumerable<string>> SendPrivateMessageAsync(PrivateMessage privateMessage);
    }
}
