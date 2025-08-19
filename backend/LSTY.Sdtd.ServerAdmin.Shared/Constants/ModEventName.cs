namespace LSTY.Sdtd.ServerAdmin.Shared.Constants
{
    /// <summary>
    /// Represents the names of mod events that can occur in the system.
    /// </summary>
    public enum ModEventName
    {
        /// <summary>
        /// Welcome event.
        /// </summary>
        Welcome,

        /// <summary>
        /// Command execution reply event.
        /// </summary>
        CommandExecutionReply,

        /// <summary>
        /// Log callback event.
        /// </summary>
        LogCallback,

        /// <summary>
        /// Game awake event.
        /// </summary>
        GameAwake,

        /// <summary>
        /// Game start completed event.
        /// </summary>
        GameStartDone,

        /// <summary>
        /// Game update event.
        /// </summary>
        GameUpdate,

        /// <summary>
        /// Game shutdown event.
        /// </summary>
        GameShutdown,

        /// <summary>
        /// Chunk color calculation completed event.
        /// </summary>
        CalcChunkColorsDone,

        /// <summary>
        /// Chat message event.
        /// </summary>
        ChatMessage,

        /// <summary>
        /// Entity killed event.
        /// </summary>
        EntityKilled,

        /// <summary>
        /// Entity spawned event.
        /// </summary>
        EntitySpawned,

        /// <summary>
        /// Player disconnected event.
        /// </summary>
        PlayerDisconnected,

        /// <summary>
        /// Player login event.
        /// </summary>
        PlayerLogin,

        /// <summary>
        /// Player spawned in the world event.
        /// </summary>
        PlayerSpawnedInWorld,

        /// <summary>
        /// Player spawning event.
        /// </summary>
        PlayerSpawning,

        /// <summary>
        /// Save player data event.
        /// </summary>
        SavePlayerData,

        /// <summary>
        /// Sky changed event.
        /// </summary>
        SkyChanged,
    }
}
