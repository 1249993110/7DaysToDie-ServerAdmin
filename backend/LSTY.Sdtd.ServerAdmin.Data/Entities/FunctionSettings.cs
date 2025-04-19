using LSTY.Sdtd.ServerAdmin.Data.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class FunctionSettings : EntityBase
    {
        public required string GameServerId { get; set; }
        public required string? FunctionName { get; set; }
        public required Dictionary<string, object> SettingsDict { get; set; }
    }
}
