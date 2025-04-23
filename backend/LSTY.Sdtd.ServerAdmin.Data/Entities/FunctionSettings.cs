using LSTY.Sdtd.ServerAdmin.Data.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class FunctionSettings : EntityBase
    {
        public required string GameServerId { get; set; }
        public required string? FunctionName { get; set; } // nameof(CommonSettings)
        public required Dictionary<string, object?> Settings { get; set; }

        static FunctionSettings()
        {
            DB.Index<FunctionSettings>()
                .Key(e => e.GameServerId, KeyType.Ascending)
                .Key(e => e.FunctionName, KeyType.Ascending)
                .Option(o =>
                {
                    o.Background = true;
                    o.Unique = true;
                })
                .CreateAsync();
        }
    }
}
