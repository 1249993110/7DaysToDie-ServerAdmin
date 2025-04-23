namespace LSTY.Sdtd.ServerAdmin.Data.Abstractions
{
    public interface ICustomLogger
    {
        Task<string> LogInformationAsync(Exception exception, string message);
        Task<string> LogInformationAsync(string message);
        Task<string> LogWarningAsync(Exception exception, string message);
        Task<string> LogWarningAsync(string message);
        Task<string> LogErrorAsync(Exception exception, string message);
        Task<string> LogErrorAsync(string message);
    }
}
