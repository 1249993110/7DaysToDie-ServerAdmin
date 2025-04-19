using LSTY.Sdtd.ServerAdmin.Services.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Services.Settings
{
    public class GeneralSettings : ISettings
    {
        public bool IsEnabled { get; set; }
    }
}
