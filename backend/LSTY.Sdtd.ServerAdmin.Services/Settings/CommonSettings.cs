﻿using LSTY.Sdtd.ServerAdmin.Services.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.Services.Settings
{
    public class CommonSettings : ISettings
    {
        bool ISettings.IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the global server name.
        /// </summary>
        public required string GlobalServerName { get; set; }

        /// <summary>
        /// Gets or sets the whisper server name.
        /// </summary>
        public required string WhisperServerName { get; set; }

        /// <summary>
        /// Gets or sets the chat command prefix.
        /// </summary>
        public required string ChatCommandPrefix { get; set; }

        /// <summary>
        /// Gets or sets the chat command separator.
        /// </summary>
        public required string ChatCommandSeparator { get; set; }
    }
}
