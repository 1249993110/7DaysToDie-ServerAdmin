using LSTY.Sdtd.ServerAdmin.Services.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Services.Settings
{
    public sealed class BasicSettings : ISettings
    {
        public bool IsEnabled { get; set; }
    }
}
