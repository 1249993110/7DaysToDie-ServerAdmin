using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.Data.Enums;
using LogLevel = LSTY.Sdtd.ServerAdmin.Data.Enums.LogLevel;

namespace LSTY.Sdtd.ServerAdmin.Data.Logging
{
    public class CustomLogger : ICustomLogger
    {
        private readonly ServiceModule _serviceModule;
        private readonly string _gameServerId;

        public CustomLogger(ServiceModule serviceModule, string gameServerId)
        {
            _serviceModule = serviceModule;
            _gameServerId = gameServerId;
        }

        public Task<string> LogInformationAsync(Exception exception, string message, params object?[] args)
        {
            return SaveLog(LogLevel.Info, string.Format(message, args), exception);
        }

        public Task<string> LogInformationAsync(string message, params object?[] args)
        {
            return SaveLog(LogLevel.Info, string.Format(message, args));
        }

        public Task<string> LogWarningAsync(Exception exception, string message, params object?[] args)
        {
            return SaveLog(LogLevel.Warning, string.Format(message, args), exception);
        }

        public Task<string> LogWarningAsync(string message, params object?[] args)
        {
            return SaveLog(LogLevel.Warning, string.Format(message, args));
        }

        public Task<string> LogErrorAsync(Exception exception, string message, params object?[] args)
        {
            return SaveLog(LogLevel.Error, string.Format(message, args), exception);
        }

        public Task<string> LogErrorAsync(string message, params object?[] args)
        {
            return SaveLog(LogLevel.Error, string.Format(message, args));
        }

        private async Task<string> SaveLog(LogLevel level, string message, Exception? exception = null)
        {
            var logEntry = new LogEntry()
            {
                CreatedOn = DateTime.UtcNow,
                Level = level,
                Content = message,
                AdditionalData = exception?.ToString(),
                ServiceModule = _serviceModule,
                GameServerId = _gameServerId,
            };

            await DB.SaveAsync(logEntry);

            return logEntry.ID;
        }
    }
}
