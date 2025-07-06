namespace LSTY.Sdtd.ServerAdmin.Data.Abstractions
{
    public interface ICustomLogger
    {
        Task<Guid> LogInformationAsync(Exception exception, string message);
        Task<Guid> LogInformationAsync(string message);
        Task<Guid> LogWarningAsync(Exception exception, string message);
        Task<Guid> LogWarningAsync(string message);
        Task<Guid> LogErrorAsync(Exception exception, string message);
        Task<Guid> LogErrorAsync(string message);
    }
}
