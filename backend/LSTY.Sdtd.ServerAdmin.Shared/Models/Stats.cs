namespace LSTY.Sdtd.ServerAdmin.Shared.Models
{
    /// <summary>
    /// Statistics
    /// </summary>
    public class Stats
    {
        /// <summary>
        /// Server uptime
        /// </summary>
        public float Uptime { get; set; }

        /// <summary>
        /// Game time
        /// </summary>
        public required GameTime GameTime { get; set; }

        /// <summary>
        /// Number of animals
        /// </summary>
        public int Animals { get; set; }

        /// <summary>
        /// Max Spawned Animals
        /// </summary>
        public int MaxAnimals { get; set; }

        /// <summary>
        /// Number of zombies
        /// </summary>
        public int Zombies { get; set; }

        /// <summary>
        /// Max Spawned Zombies
        /// </summary>
        public int MaxZombies { get; set; }

        /// <summary>
        /// Number of entities
        /// </summary>
        public int Entities { get; set; }

        /// <summary>
        /// Number of online players
        /// </summary>
        public int OnlinePlayers { get; set; }

        /// <summary>
        /// Maximum number of online players
        /// </summary>
        public int MaxOnlinePlayers { get; set; }

        /// <summary>
        /// Number of history players
        /// </summary>
        public int HistoryPlayers { get; set; }

        /// <summary>
        /// Number of offline players
        /// </summary>
        public int OfflinePlayers { get; set; }

        /// <summary>
        /// Whether it is a blood moon
        /// </summary>
        public bool IsBloodMoon { get; set; }

        /// <summary>
        /// Frames Per Second
        /// </summary>
        [JsonProperty("fps")]
        public float FPS { get; set; }

        /// <summary>
        /// Heap memory usage, in megabytes (MB), representing the current heap memory size used by the game
        /// </summary>
        public float Heap { get; set; }

        /// <summary>
        /// The maximum heap memory limit, in megabytes (MB), representing the maximum available heap memory capacity
        /// </summary>
        public float MaxHeap { get; set; }

        /// <summary>
        /// Number of chunks in the game
        /// </summary>
        public int Chunks { get; set; }

        /// <summary>
        /// Indicates the number of active objects in the current game
        /// </summary>
        public int ChunkGameObjects { get; set; }

        /// <summary>
        /// Number of items
        /// </summary>
        public int Items { get; set; }

        /// <summary>
        /// Number of chunk observed entities
        /// </summary>
        public int ChunkObservedEntities { get; set; }

        /// <summary>
        /// Resident set size, representing the physical memory size occupied by the current game
        /// </summary>
        public float ResidentSetSize { get; set; }

        /// <summary>
        /// Server Name
        /// </summary>
        public required string ServerName { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        public required string Region { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        public required string Language { get; set; }

        /// <summary>
        /// Server Version
        /// </summary>
        public required string ServerVersion { get; set; }

        /// <summary>
        /// Server IP
        /// </summary>
        public required string ServerIp { get; set; }

        /// <summary>
        /// ServerPort
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// Game Mode
        /// </summary>
        public required string GameMode { get; set; }

        /// <summary>
        /// Game World
        /// </summary>
        public required string GameWorld { get; set; }

        /// <summary>
        /// Game Name
        /// </summary>
        public required string GameName { get; set; }

        /// <summary>
        /// Game Difficulty
        /// </summary>
        public int GameDifficulty { get; set; }
    }
}
