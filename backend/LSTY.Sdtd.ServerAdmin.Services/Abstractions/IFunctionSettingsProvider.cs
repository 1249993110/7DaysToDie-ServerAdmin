namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    public interface IFunctionSettingsProvider
    {
        Task<IReadOnlyDictionary<string, object>?> GetAsync(string gameServerId, string? functionName);
    }
}
