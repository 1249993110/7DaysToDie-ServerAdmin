namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    public interface IFunctionSettingsProvider
    {
        Task<string?> GetAsync(string gameServerId, string functionName);
    }
}
