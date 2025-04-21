namespace LSTY.Sdtd.ServerAdmin.Data.Abstractions
{
    public interface ICustomLogger
    {
        Task<string> LogInformationAsync(Exception exception, string message, params object?[] args);
        Task<string> LogInformationAsync(string message, params object?[] args);
        Task<string> LogWarningAsync(Exception exception, string message, params object?[] args);
        Task<string> LogWarningAsync(string message, params object?[] args);
        Task<string> LogErrorAsync(Exception exception, string message, params object?[] args);
        Task<string> LogErrorAsync(string message, params object?[] args);
    }
}
