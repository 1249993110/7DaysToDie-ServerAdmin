namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    public interface IFunctionSettingsProvider
    {
        Task<string?> GetAsync(Guid gameServerId, string functionName);
    }
}
