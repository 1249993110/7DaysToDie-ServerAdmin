using LSTY.Sdtd.ServerAdmin.Services.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Services.Settings
{
    public class CommonSettings : ISettings
    {
        bool ISettings.IsEnabled { get; set; } = true;

        /// <summary>
        ///
        /// </summary>
        public string? GlobalServerName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string? WhisperServerName { get; set; }

        /// <summary>
        ///
        /// </summary>
        public char[]? ChatCommandPrefixes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AllowNoPrefix { get; set; }

        /// <summary>
        ///
        /// </summary>
        public char[]? ChatCommandSeparator { get; set; }
    }
}
