using LSTY.Sdtd.ServerAdmin.Data.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Data.Entities
{
    public class FunctionConfig : EntityBase
    {
        public required string GameServerId { get; set; }

        /// <summary>
        /// Function name.
        /// </summary>
        /// <remarks>
        /// Use `CommonSettings` for common settings.
        /// </remarks>
        public required string FunctionName { get; set; }

        /// <summary>
        /// Serialized settings.
        /// </summary>
        public required string Settings { get; set; }

        static FunctionConfig()
        {
            DB.Index<FunctionConfig>()
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
