namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    public delegate Task<bool> ChatHook(string command, CommandSender commandSender);
}
