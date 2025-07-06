using IceCoffee.Db4Net.Extensions;
using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.Data.Enums;
using LogLevel = LSTY.Sdtd.ServerAdmin.Data.Enums.LogLevel;

namespace LSTY.Sdtd.ServerAdmin.Data.Logging
{
    public class CustomLogger : ICustomLogger
    {
        private readonly ServiceModule _serviceModule;
        private readonly Guid _gameServerId;

        public CustomLogger(ServiceModule serviceModule, Guid gameServerId)
        {
            _serviceModule = serviceModule;
            _gameServerId = gameServerId;
        }

        public Task<Guid> LogInformationAsync(Exception exception, string message)
        {
            return SaveLog(LogLevel.Info, message, exception);
        }

        public Task<Guid> LogInformationAsync(string message)
        {
            return SaveLog(LogLevel.Info, message);
        }

        public Task<Guid> LogWarningAsync(Exception exception, string message)
        {
            return SaveLog(LogLevel.Warning, message, exception);
        }

        public Task<Guid> LogWarningAsync(string message)
        {
            return SaveLog(LogLevel.Warning, message);
        }

        public Task<Guid> LogErrorAsync(Exception exception, string message)
        {
            return SaveLog(LogLevel.Error, message, exception);
        }

        public Task<Guid> LogErrorAsync(string message)
        {
            return SaveLog(LogLevel.Error, message);
        }

        private async Task<Guid> SaveLog(LogLevel level, string message, Exception? exception = null)
        {
            var logEntry = new LogEntry()
            {
                LogLevel = level,
                Message = message,
                Exception = exception?.ToString(),
                ServiceModule = _serviceModule,
                GameServerId = _gameServerId,
                CorrelationId = Guid.NewGuid(),
            };

            await Db.Insert(logEntry).ExecuteAsync();
            return logEntry.CorrelationId;
        }
    }
}
