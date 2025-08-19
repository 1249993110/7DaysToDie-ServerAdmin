using LSTY.Sdtd.ServerAdmin.Shared.EventArgs;

namespace LSTY.Sdtd.ServerAdmin.Shared
{
    public interface IModEventHub
    {
        /// <summary>
        /// Event that is triggered when a log entry is received.
        /// </summary>
        event EventHandler<LogCallbackEventArgs>? LogCallback;

        /// <summary>
        /// Event that is triggered when the game is awake and ready for interaction.
        /// </summary>
        event EventHandler? GameAwake;

        /// <summary>
        /// Event that is triggered when the game has finished starting and players can join.
        /// </summary>
        event EventHandler? GameStartDone;

        /// <summary>
        /// Event that is triggered when the game is about to shut down.
        /// </summary>
        event EventHandler? GameShutdown;

        //event Action CalcChunkColorsDone;

        /// <summary>
        /// Event that is triggered when a chat message is received.
        /// </summary>
        event EventHandler<ChatMessageEventArgs>? ChatMessage;

        /// <summary>
        /// Event that is triggered when an entity is killed.
        /// </summary>
        event EventHandler<EntityKilledEventArgs>? EntityKilled;

        /// <summary>
        /// Event that is triggered when an entity is spawned.
        /// </summary>
        event EventHandler<EntitySpawnedEventArgs>? EntitySpawned;

        /// <summary>
        /// Event that is triggered when a player disconnects.
        /// </summary>
        event EventHandler<PlayerDisconnectedEventArgs>? PlayerDisconnected;

        /// <summary>
        /// Event that is triggered when a player logs in.
        /// </summary>
        event EventHandler<PlayerLoginEventArgs>? PlayerLogin;

        /// <summary>
        /// Event that is triggered when a player is spawned in the world.
        /// </summary>
        event EventHandler<PlayerSpawnedInWorldEventArgs>? PlayerSpawnedInWorld;

        /// <summary>
        /// Event that is triggered when a player is about to spawn in the world.
        /// </summary>
        event EventHandler<PlayerSpawningEventArgs>? PlayerSpawning;

        /// <summary>
        /// Event that is triggered when player data is saved.
        /// </summary>
        event EventHandler<SavePlayerDataEventArgs>? SavePlayerData;

        /// <summary>
        /// Event that is triggered when the sky changes.
        /// </summary>
        event EventHandler<SkyChangedEventArgs>? SkyChanged;
    }
}
