using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Enums;

namespace LSTY.Sdtd.ServerAdmin.Data.Logging
{
    public class CustomLoggerFactory : ICustomLoggerFactory
    {
        public ICustomLogger CreateLogger(ServiceModule serviceModule, string gameServerId)
        {
            return new CustomLogger(serviceModule, gameServerId);
        }
    }
}
