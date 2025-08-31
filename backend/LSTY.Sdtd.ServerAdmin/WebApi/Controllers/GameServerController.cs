using LSTY.Sdtd.ServerAdmin.Config;
using LSTY.Sdtd.ServerAdmin.Shared.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Web.Http;
using System.Xml;
using UnityEngine;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Provides an interface to access the game server.
    /// Contains endpoints for querying status, executing commands, and configuration management, etc., which are related to the life cycle of the game server.
    /// </summary>
    [Authorize]
    [RequireGameStartDone]
    [RoutePrefix("api/GameServer")]
    public class GameServerController : ApiController
    {
        #region Execute Console Command
        /// <summary>
        /// Executes a console command and returns the result.
        /// </summary>
        /// <returns>List of results indicating success or failure of the command execution.</returns>
        [HttpPost]
        [Route(nameof(ExecuteConsoleCommand))]
        public Task<IEnumerable<string>> ExecuteConsoleCommand([FromBody] ConsoleCommand consoleCommand)
        {
            return Utils.ExecuteConsoleCommandAsync(consoleCommand.Command, consoleCommand.InMainThread);
        }
        #endregion

        #region Send Message
        /// <summary>
        /// Send a global message to all players.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendGlobalMessage")]
        public Task<IEnumerable<string>> SendGlobalMessage([FromBody, Required] GlobalMessage message)
        {
            return Utils.SendGlobalMessageAsync(message);
        }

        /// <summary>
        /// Send a private message to a specific player.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("SendPrivateMessage")]
        public Task<IEnumerable<string>> SendPrivateMessage([FromBody, Required] PrivateMessage message)
        {
            return Utils.SendPrivateMessageAsync(message);
        }
        #endregion

        #region Admins
        /// <summary>
        /// Create an admin
        /// </summary>
        /// <param name="admin">Admin entry details.</param>
        /// <returns>List of results indicating success or failure for the admin creation.</returns>
        [HttpPost]
        [Route("Admins")]
        public IEnumerable<string> CreateAdmin([FromBody, Required] AdminEntry admin)
        {
            string command = $"admin add {admin.PlayerId} {admin.PermissionLevel} {Utils.FormatCommandArgs(admin.DisplayName)}";
            var result = SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate);
            return result;
        }

        /// <summary>
        /// Get admins
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Admins")]
        public IEnumerable<AdminEntry> GetAdmins()
        {
            var result = new List<AdminEntry>();
            foreach (var item in GameManager.Instance.adminTools.Users.GetUsers().Values)
            {
                result.Add(new AdminEntry()
                {
                    PlayerId = item.UserIdentifier.CombinedString,
                    PermissionLevel = item.PermissionLevel,
                    DisplayName = item.Name,
                });
            }

            return result;
        }

        /// <summary>
        /// Remove admins
        /// </summary>
        /// <param name="playerIds">Array of player IDs to remove.</param>
        /// <returns>List of results indicating success or failure for each admin removal.</returns>
        [HttpDelete]
        [Route("Admins")]
        public IEnumerable<string> RemoveAdmins([FromUri, Required] string[] playerIds)
        {
            var result = new List<string>();
            foreach (var item in playerIds)
            {
                string command = $"admin remove {item}";
                result.AddRange(SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate));
            }

            return result;
        }
        #endregion

        #region Statistics
        /// <summary>
        /// Get game server stats
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Stats")]
        public Stats GetStatistics()
        {
            var gameManager = GameManager.Instance;
            var world = GameManager.Instance.World;
            var worldTime = world.GetWorldTime();
            var entityList = world.Entities.list;

            int zombies = 0;
            int animals = 0;
            foreach (var entity in entityList)
            {
                if (entity.IsAlive())
                {
                    if (entity is EntityEnemy)
                    {
                        zombies++;
                    }
                    else if (entity is EntityAnimal)
                    {
                        animals++;
                    }
                }
            }

            int onlinePlayerCount = world.Players.Count;
            int offlinePlayerCount = gameManager.GetPersistentPlayerList().Players.Count - onlinePlayerCount;
            return new Stats()
            {
                Uptime = Time.timeSinceLevelLoad,
                GameTime = new GameTime()
                {
                    Days = GameUtils.WorldTimeToDays(worldTime),
                    Hours = GameUtils.WorldTimeToHours(worldTime),
                    Minutes = GameUtils.WorldTimeToMinutes(worldTime),
                },
                Animals = animals,
                MaxAnimals = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedAnimals),
                Zombies = zombies,
                MaxZombies = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies),
                Entities = world.Entities.Count,
                OnlinePlayers = onlinePlayerCount,
                MaxOnlinePlayers = GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount),
                HistoryPlayers = gameManager.GetPersistentPlayerList().Players.Count,
                OfflinePlayers = offlinePlayerCount,
                IsBloodMoon = world.aiDirector.BloodMoonComponent.BloodMoonActive,
                FPS = gameManager.fps.Counter,
                Heap = (float)GC.GetTotalMemory(false) / 1048576F,
                MaxHeap = (float)GameManager.MaxMemoryConsumption / 1048576F,
                Chunks = Chunk.InstanceCount,
                ChunkGameObjects = world.m_ChunkManager.GetDisplayedChunkGameObjectsCount(),
                Items = EntityItem.ItemInstanceCount,
                ChunkObservedEntities = world.m_ChunkManager.m_ObservedEntities.Count,
                ResidentSetSize = (float)SystemInformation.GetRSS.GetCurrentRSS() / 1048576F,
                ServerName = GamePrefs.GetString(EnumGamePrefs.ServerName),
                Region = GamePrefs.GetString(EnumGamePrefs.Region),
                Language = GamePrefs.GetString(EnumGamePrefs.Language),
                ServerVersion = global::Constants.cVersionInformation.LongString,
                ServerIp = GamePrefs.GetString(EnumGamePrefs.ServerIP),
                ServerPort = GamePrefs.GetInt(EnumGamePrefs.ServerPort),
                GameMode = GamePrefs.GetString(EnumGamePrefs.GameMode),
                GameWorld = GamePrefs.GetString(EnumGamePrefs.GameWorld),
                GameName = GamePrefs.GetString(EnumGamePrefs.GameName),
                GameDifficulty = GamePrefs.GetInt(EnumGamePrefs.GameDifficulty),
            };
        }
        #endregion

        #region Configuration
        /// <summary>
        /// Get server config.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Config")]
        public Dictionary<string, string> GetConfig()
        {
            var result = new Dictionary<string, string>();

            string path = Path.Combine(AppContext.BaseDirectory, AppConfig.Settings.ServerConfigFile);
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            var xmlRoot = xmlDocument.DocumentElement;

            string name;
            string value;
            foreach (XmlNode node in xmlRoot.ChildNodes)
            {
                if (node.Attributes != null)
                {
                    name = node.Attributes["name"].Value;
                    value = node.Attributes["value"].Value;
                    result.Add(name, value);
                }
            }

            return result;
        }

        /// <summary>
        /// Update server config.
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("Config")]
        public IHttpActionResult UpdateConfig([FromBody] Dictionary<string, string> model)
        {
            string path = Path.Combine(AppContext.BaseDirectory, AppConfig.Settings.ServerConfigFile);
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            var rootNode = xmlDocument.SelectSingleNode("ServerSettings");

            foreach (var item in model)
            {
                var node = rootNode.SelectSingleNode(string.Format("property[@name='{0}']", item.Key));
                if (node != null && node.Attributes != null)
                {
                    node.Attributes["value"].Value = item.Value;
                }
            }

            xmlDocument.Save(path);
            return Ok();
        }
        #endregion
    }
}
