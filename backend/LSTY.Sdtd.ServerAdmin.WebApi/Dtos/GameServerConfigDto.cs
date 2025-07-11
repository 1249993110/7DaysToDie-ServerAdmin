﻿namespace LSTY.Sdtd.ServerAdmin.WebApi.Dtos
{
    /// <summary>
    /// Represents the configuration details of a game server.
    /// </summary>
    public class GameServerConfigDto
    {
        /// <summary>
        /// The unique identifier of the game server configuration.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// The date and time when the configuration was created.
        /// </summary>
        public required DateTime CreatedAt { get; set; }

        /// <summary>
        /// The name of the game server.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// The host of the game server.
        /// </summary>
        public required string Host { get; set; }

        /// <summary>
        /// The port number used by the game server.
        /// </summary>
        public required int Port { get; set; }

        /// <summary>
        /// A value indicating whether the game server is enabled.
        /// </summary>
        public required bool IsEnabled { get; set; }

        /// <summary>
        /// The optional description of the game server.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The user ID associated with the game server configuration.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// The connection state of the game server.
        /// </summary>
        public required bool IsConnected { get; set; }
    }
}
