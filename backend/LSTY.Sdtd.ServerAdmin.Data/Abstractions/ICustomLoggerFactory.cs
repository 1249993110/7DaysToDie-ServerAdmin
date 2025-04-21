using LSTY.Sdtd.ServerAdmin.Data.Enums;

namespace LSTY.Sdtd.ServerAdmin.Data.Abstractions
{
    public interface ICustomLoggerFactory
    {
        ICustomLogger CreateLogger(ServiceModule serviceModule, string gameServerId);
    }
}
