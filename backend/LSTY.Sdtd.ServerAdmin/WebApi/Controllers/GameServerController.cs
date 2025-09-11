using HarmonyLib;
using LSTY.Sdtd.ServerAdmin.Config;
using LSTY.Sdtd.ServerAdmin.Extensions;
using LSTY.Sdtd.ServerAdmin.Shared.Models;
using Newtonsoft.Json;
using NSwag.Annotations;
using SkiaSharp;
using System.ComponentModel.DataAnnotations;
using System.Net;
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
        [Route("ExecuteConsoleCommand")]
        public Task<IEnumerable<string>> ExecuteConsoleCommand([FromBody, Required] ConsoleCommand consoleCommand)
        {
            return Utils.ExecuteConsoleCommandAsync(consoleCommand.Command, consoleCommand.InMainThread);
        }

        /// <summary>
        /// Get all allowed commands.
        /// </summary>
        [HttpGet]
        [Route("AllowedCommands")]
        public IEnumerable<AllowedCommand> GetAllowedCommands()
        {
            var consoleCommands = SdtdConsole.Instance.GetCommands();
            var allowedCommands = new List<AllowedCommand>(consoleCommands.Count);
            foreach (var consoleCommand in consoleCommands)
            {
                var commands = consoleCommand.GetCommands();
                int commandPermissionLevel = GameManager.Instance.adminTools.Commands.GetCommandPermissionLevel(commands);

                allowedCommands.Add(new AllowedCommand()
                {
                    Commands = commands,
                    PermissionLevel = commandPermissionLevel,
                    Description = consoleCommand.GetDescription(),
                    Help = consoleCommand.GetHelp(),
                });
            }

            return allowedCommands;
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
        /// Create an admin.
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
        /// Get admins.
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
        /// Delete admins.
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
        /// Get game server stats.
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
        public IHttpActionResult UpdateConfig([FromBody, Required] Dictionary<string, string> model)
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

        #region Players
        /// <summary>
        /// Get paged online players.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("OnlinePlayers")]
        public PagedDto<OnlinePlayer> GetOnlinePlayers([FromUri] PagingQueryDto<OnlinePlayerQueryOrder>? queryDto)
        {
            queryDto ??= new PagingQueryDto<OnlinePlayerQueryOrder>();
            int pageSize = queryDto.PageSize;
            string? keyword = queryDto.Keyword;
            var clientInfoMap = ConnectionManager.Instance.Clients.entityIdMap;
            IEnumerable<EntityPlayer> entityPlayers = GameManager.Instance.World.Players.list;
            int total = 0;

            if (queryDto.Order == null && string.IsNullOrEmpty(keyword))
            {
                total = GameManager.Instance.World.Players.list.Count;
                if (pageSize > 0)
                {
                    entityPlayers = entityPlayers.Skip((queryDto.PageNumber - 1) * pageSize).Take(pageSize);
                }

                var items = new List<OnlinePlayer>();
                foreach (var entityPlayer in entityPlayers)
                {
                    var clientInfo = clientInfoMap[entityPlayer.entityId];
                    items.Add(entityPlayer.ToOnlinePlayer(clientInfo));
                }

                return new PagedDto<OnlinePlayer>()
                {
                    Items = items,
                    Total = total,
                };
            }
            else
            {
                IEnumerable<OnlinePlayer> items = new List<OnlinePlayer>();
                bool isHasKeyword = string.IsNullOrEmpty(keyword) == false;
                foreach (var entityPlayer in entityPlayers)
                {
                    var clientInfo = clientInfoMap[entityPlayer.entityId];
                    if (isHasKeyword
                        && clientInfo.entityId.ToString() != keyword
                        && clientInfo.CrossplatformId.CombinedString.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1
                        && clientInfo.PlatformId.CombinedString.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1
                        && clientInfo.playerName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        continue;
                    }

                    ((List<OnlinePlayer>)items).Add(entityPlayer.ToOnlinePlayer(clientInfo));
                }

                total = ((List<OnlinePlayer>)items).Count;

                if (queryDto.Order.HasValue)
                {
                    switch (queryDto.Order.Value)
                    {
                        case OnlinePlayerQueryOrder.EntityId:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.EntityId) : items.OrderBy(k => k.EntityId);
                            break;
                        case OnlinePlayerQueryOrder.PlayerName:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.PlayerName) : items.OrderBy(k => k.PlayerName);
                            break;
                        case OnlinePlayerQueryOrder.Ping:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.Ping) : items.OrderBy(k => k.Ping);
                            break;
                        case OnlinePlayerQueryOrder.PermissionLevel:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.PermissionLevel) : items.OrderBy(k => k.PermissionLevel);
                            break;
                        case OnlinePlayerQueryOrder.ZombieKills:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.ZombieKills) : items.OrderBy(k => k.ZombieKills);
                            break;
                        case OnlinePlayerQueryOrder.PlayerKills:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.PlayerKills) : items.OrderBy(k => k.PlayerKills);
                            break;
                        case OnlinePlayerQueryOrder.Deaths:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.Deaths) : items.OrderBy(k => k.Deaths);
                            break;
                        case OnlinePlayerQueryOrder.Level:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.Level) : items.OrderBy(k => k.Level);
                            break;
                        case OnlinePlayerQueryOrder.ExpToNextLevel:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.ExpToNextLevel) : items.OrderBy(k => k.ExpToNextLevel);
                            break;
                        case OnlinePlayerQueryOrder.SkillPoints:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.SkillPoints) : items.OrderBy(k => k.SkillPoints);
                            break;
                        case OnlinePlayerQueryOrder.GameStage:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.GameStage) : items.OrderBy(k => k.GameStage);
                            break;
                        default:
                            break;
                    }
                }

                if (pageSize > 0)
                {
                    items = items.Skip((queryDto.PageNumber - 1) * pageSize).Take(pageSize);
                }

                return new PagedDto<OnlinePlayer>()
                {
                    Items = items,
                    Total = total,
                };
            }
        }

        /// <summary>
        /// Get online player by player id.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("OnlinePlayers/{playerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Shared.Models.OnlinePlayer))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(object))]
        public IHttpActionResult GetOnlinePlayerById(string playerId)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }
            var clientInfo = ConnectionManager.Instance.Clients.ForUserId(userId);
            if (clientInfo == null)
            {
                return NotFound();
            }
            var entityPlayer = GameManager.Instance.World.Players.dict.GetValueSafe(clientInfo.entityId);
            if (entityPlayer == null)
            {
                return NotFound();
            }
            return Ok(entityPlayer.ToOnlinePlayer(clientInfo));
        }

        /// <summary>
        /// Get paged history players.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("HistoryPlayers")]
        public PagedDto<HistoryPlayer> GetHistoryPlayers([FromUri] PagingQueryDto<HistoryPlayerQueryOrder>? queryDto)
        {
            queryDto ??= new PagingQueryDto<HistoryPlayerQueryOrder>();
            int pageSize = queryDto.PageSize;
            string? keyword = queryDto.Keyword;
            int total = 0;
            IEnumerable<PersistentPlayerData> persistentPlayers = GameManager.Instance.GetPersistentPlayerList().Players.Values;

            if (queryDto.Order == null && string.IsNullOrEmpty(keyword))
            {
                total = GameManager.Instance.GetPersistentPlayerList().Players.Count;

                if (pageSize > 0)
                {
                    persistentPlayers = persistentPlayers.Skip((queryDto.PageNumber - 1) * pageSize).Take(pageSize);
                }

                var items = new List<HistoryPlayer>();
                foreach (var persistentPlayer in persistentPlayers)
                {
                    var clientInfo = ConnectionManager.Instance.Clients.ForEntityId(persistentPlayer.EntityId);
                    items.Add(persistentPlayer.ToHistoryPlayer(clientInfo));
                }

                return new PagedDto<HistoryPlayer>()
                {
                    Items = items,
                    Total = total,
                };
            }
            else
            {
                IEnumerable<HistoryPlayer> items = new List<HistoryPlayer>();
                bool isHasKeyword = string.IsNullOrEmpty(keyword) == false;
                foreach (var persistentPlayer in persistentPlayers)
                {
                    if (isHasKeyword
                        && persistentPlayer.EntityId.ToString() != keyword
                        && persistentPlayer.PrimaryId.CombinedString.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1
                        && persistentPlayer.NativeId.CombinedString.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1
                        && persistentPlayer.PlayerName.DisplayName.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) == -1)
                    {
                        continue;
                    }

                    var clientInfo = ConnectionManager.Instance.Clients.ForEntityId(persistentPlayer.EntityId);
                    ((List<HistoryPlayer>)items).Add(persistentPlayer.ToHistoryPlayer(clientInfo));
                }

                total = ((List<HistoryPlayer>)items).Count;

                if (queryDto.Order.HasValue)
                {
                    switch (queryDto.Order.Value)
                    {
                        case HistoryPlayerQueryOrder.EntityId:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.EntityId) : items.OrderBy(k => k.EntityId);
                            break;
                        case HistoryPlayerQueryOrder.PlayerName:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.PlayerName) : items.OrderBy(k => k.PlayerName);
                            break;
                        case HistoryPlayerQueryOrder.PermissionLevel:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.PermissionLevel) : items.OrderBy(k => k.PermissionLevel);
                            break;
                        case HistoryPlayerQueryOrder.PlayGroup:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.PlayGroup) : items.OrderBy(k => k.PlayGroup);
                            break;
                        case HistoryPlayerQueryOrder.IsOffline:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.IsOffline) : items.OrderBy(k => k.IsOffline);
                            break;
                        case HistoryPlayerQueryOrder.LastLogin:
                            items = queryDto.Desc ? items.OrderByDescending(k => k.LastLogin) : items.OrderBy(k => k.LastLogin);
                            break;
                        default:
                            break;
                    }
                }

                if (pageSize > 0)
                {
                    items = items.Skip((queryDto.PageNumber - 1) * pageSize).Take(pageSize);
                }

                return new PagedDto<HistoryPlayer>()
                {
                    Items = items,
                    Total = total,
                };
            }
        }

        /// <summary>
        /// Get history player by player id.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("HistoryPlayers/{playerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Shared.Models.HistoryPlayer))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(object))]
        public IHttpActionResult GetHistoryPlayerById(string playerId)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }
            var persistentPlayerMap = GameManager.Instance.GetPersistentPlayerList().Players;
            if (persistentPlayerMap.TryGetValue(userId, out var persistentPlayerData) == false)
            {
                return NotFound();
            }
            var clientInfo = ConnectionManager.Instance.Clients.ForEntityId(persistentPlayerData.EntityId);
            return Ok(persistentPlayerData.ToHistoryPlayer(clientInfo));
        }

        /// <summary>
        /// Get player details by player id.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PlayerDetails/{playerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Shared.Models.PlayerDetails))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(object))]
        public IHttpActionResult GetPlayerDetails(string playerId)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }

            var persistentPlayerMap = GameManager.Instance.GetPersistentPlayerList().Players;
            if (persistentPlayerMap.TryGetValue(userId, out var persistentPlayerData) == false)
            {
                return NotFound();
            }

            var clientInfo = ConnectionManager.Instance.Clients.ForEntityId(persistentPlayerData.EntityId);
            var entityPlayer = GameManager.Instance.World.Players.dict.GetValueSafe(persistentPlayerData.EntityId);
            return Ok(persistentPlayerData.ToPlayerDetails(clientInfo, entityPlayer));
        }

        /// <summary>
        /// Get player inventory by player id.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PlayerInventory/{playerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Shared.Models.Inventory))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(object))]
        public IHttpActionResult GetPlayerInventory(string playerId, [FromUri] Language language = Language.English)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }

            var clientInfo = ConnectionManager.Instance.Clients.ForUserId(userId);
            if (clientInfo != null)
            {
                return Ok(clientInfo.latestPlayerData.GetInventory(language));
            }

            var playerDataFile = new PlayerDataFile();
            playerDataFile.Load(GameIO.GetPlayerDataDir(), userId.CombinedString);
            if (playerDataFile.bLoaded)
            {
                return Ok(playerDataFile.GetInventory(language));
            }

            return NotFound();
        }

        /// <summary>
        /// Get player skills by player id.
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PlayerSkills/{playerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<PlayerSkill>))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(object))]
        public IHttpActionResult GetPlayerSkills(string playerId, [FromUri] Language language = Language.English)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }

            var clientInfo = ConnectionManager.Instance.Clients.ForUserId(userId);
            if (clientInfo != null && GameManager.Instance.World.Players.dict.TryGetValue(clientInfo.entityId, out var entityPlayer))
            {
                var progression = entityPlayer.Progression;
                return Ok(progression.ToPlayerSkills(language));
            }
            else
            {
                var playerDataFile = new PlayerDataFile();
                playerDataFile.Load(GameIO.GetPlayerDataDir(), userId.CombinedString);
                if (playerDataFile.bLoaded == false || playerDataFile.progressionData.Length <= 0L)
                {
                    return NotFound();
                }

                using PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(false);
                pooledBinaryReader.SetBaseStream(playerDataFile.progressionData);

                entityPlayer = new EntityPlayer();
                entityPlayer.Progression = new Progression(entityPlayer);

                var progression = Progression.Read(pooledBinaryReader, entityPlayer);
                return Ok(progression.ToPlayerSkills(language));
            }
        }
        #endregion

        #region Icons
        /// <summary>
        /// Get Item Icon
        /// </summary>
        /// <remarks>
        /// e.g. airConditioner__00FF00.png Color is optional
        /// </remarks>
        /// <param name="name">Can be either an icon name or an item name. If it's an icon name, the suffix must be .png. Color is optional. See example for format.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 7200)]
        [Route("ItemIcons/{name}")]
        public IHttpActionResult GetItemIcon(string name)
        {
            if (name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false)
            {
                var itemClass = ItemClass.GetItemClass(name);
                if (itemClass == null)
                {
                    return BadRequest("Invalid item name.");
                }

                string iconFileName = itemClass.GetIconName() + ".png";
                var iconColor = itemClass.GetIconTint();

                string? iconPath = FindIconPath(iconFileName, false);
                if (iconPath == null)
                {
                    return NotFound();
                }

                if (iconColor == UnityEngine.Color.white)
                {
                    return new FileContentResult(iconPath, "image/png");
                }

                return GetIconWithColor(iconPath, iconColor);
            }
            else
            {
                int len = name.Length;
                if (len > 12 && name[len - 11] == '_' && name[len - 12] == '_')
                {
                    string iconColor = name.Substring(len - 10, 6);
                    string iconFileName = string.Concat(name.Substring(0, len - 12), ".png");

                    int r, g, b;
                    try
                    {
                        r = Convert.ToInt32(iconColor.Substring(0, 2), 16);
                        g = Convert.ToInt32(iconColor.Substring(2, 2), 16);
                        b = Convert.ToInt32(iconColor.Substring(4, 2), 16);
                    }
                    catch
                    {
                        return BadRequest("Invalid icon color.");
                    }

                    string? iconPath = FindIconPath(iconFileName, false);
                    return iconPath == null ? NotFound() : GetIconWithColor(iconPath, r, g, b);
                }
                else
                {
                    string? iconPath = FindIconPath(name, false);
                    return iconPath == null ? NotFound() : new FileContentResult(iconPath, "image/png");
                }
            }
        }

        /// <summary>
        /// Get UI Icon
        /// </summary>
        /// <remarks>
        /// e.g. Button__00FF00.png Color is optional
        /// </remarks>
        /// <param name="name">Can be either an icon name or an item name. If it's an icon name, the suffix must be .png. Color is optional. See example for format.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 7200)]
        [Route("UiIcons/{name}")]
        public IHttpActionResult GetUiIcon(string name)
        {
            if (name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false)
            {
                return BadRequest();
            }
            else
            {
                int len = name.Length;
                if (len > 12 && name[len - 11] == '_' && name[len - 12] == '_')
                {
                    string iconColor = name.Substring(len - 10, 6);
                    string iconFileName = string.Concat(name.Substring(0, len - 12), ".png");

                    int r, g, b;
                    try
                    {
                        r = Convert.ToInt32(iconColor.Substring(0, 2), 16);
                        g = Convert.ToInt32(iconColor.Substring(2, 2), 16);
                        b = Convert.ToInt32(iconColor.Substring(4, 2), 16);
                    }
                    catch
                    {
                        return BadRequest("Invalid icon color.");
                    }

                    string? iconPath = FindIconPath(name, true);
                    return iconPath == null ? NotFound() : GetIconWithColor(iconPath, r, g, b);
                }
                else
                {
                    string? iconPath = FindIconPath(name, true);
                    return iconPath == null ? NotFound() : new FileContentResult(iconPath, "image/png");
                }
            }
        }

        private IHttpActionResult GetIconWithColor(string iconPath, int r, int g, int b)
        {
            byte[] data = System.IO.File.ReadAllBytes(iconPath);
            using var skBitmap = SKBitmap.Decode(data);
            int width = skBitmap.Width;
            int height = skBitmap.Height;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var skColor = skBitmap.GetPixel(i, j);

                    skBitmap.SetPixel(i, j, new SKColor(
                        (byte)(skColor.Red * r / 255),
                        (byte)(skColor.Green * g / 255),
                        (byte)(skColor.Blue * b / 255),
                        skColor.Alpha));
                }
            }

            var stream = new MemoryStream(data.Length / 2);
            skBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            stream.Position = 0L;
            return new FileStreamResult(stream, "image/png");
        }

        private IHttpActionResult GetIconWithColor(string iconPath, UnityEngine.Color color)
        {
            byte[] data = System.IO.File.ReadAllBytes(iconPath);
            using var skBitmap = SKBitmap.Decode(data);
            int width = skBitmap.Width;
            int height = skBitmap.Height;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var skColor = skBitmap.GetPixel(i, j);

                    if (skColor.Alpha == 0)
                    {
                        continue;
                    }

                    skBitmap.SetPixel(i, j, new SKColor(
                        (byte)(skColor.Red * color.r),
                        (byte)(skColor.Green * color.g),
                        (byte)(skColor.Blue * color.b),
                        skColor.Alpha));
                }
            }

            var stream = new MemoryStream(data.Length / 2);
            skBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
            stream.Position = 0L;
            return new FileStreamResult(stream, "image/png");
        }

        private static string? FindIconPath(string iconFileName, bool isUiIcon)
        {
            string defaultPath = isUiIcon ? Path.Combine(ModMain.ModInstance.Path, "Assets/Sprites", iconFileName) : "Data/ItemIcons/" + iconFileName;
            if (File.Exists(defaultPath))
            {
                return defaultPath;
            }

            string subPath = isUiIcon ? "UIAtlases/UIAtlas" : "UIAtlases/ItemIconAtlas";
            foreach (Mod mod in ModManager.GetLoadedMods())
            {
                var di = new DirectoryInfo(Path.Combine(mod.Path, subPath));
                if (di.Exists == false)
                {
                    continue;
                }

                var files = di.GetFiles(iconFileName, SearchOption.AllDirectories);

                if (files.Length > 0)
                {
                    return files[0].FullName;
                }
            }

            return null;
        }
        #endregion
    }
}
