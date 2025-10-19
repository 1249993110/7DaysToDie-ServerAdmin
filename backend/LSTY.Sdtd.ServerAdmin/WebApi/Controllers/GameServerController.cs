using HarmonyLib;
using LSTY.Sdtd.ServerAdmin.Config;
using LSTY.Sdtd.ServerAdmin.Extensions;
using LSTY.Sdtd.ServerAdmin.Shared.Models;
using MapRendering;
using ModInfo;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NSwag.Annotations;
using SkiaSharp;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Xml;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Windows;
using static AdminCommands;
using CommandPermission = LSTY.Sdtd.ServerAdmin.Shared.Models.CommandPermission;

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

        #region Admin Users
        /// <summary>
        /// Create an admin.
        /// </summary>
        /// <param name="model">Admin entry details.</param>
        /// <returns>List of results indicating success or failure for the admin creation.</returns>
        [HttpPost]
        [Route("AdminUsers")]
        public IEnumerable<string> CreateAdminUser([FromBody, Required] AdminUser model)
        {
            string command = $"admin add {model.PlayerId} {model.PermissionLevel} {Utils.FormatCommandArgs(model.DisplayName)}";
            var result = SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate);
            return result;
        }

        /// <summary>
        /// Get admins.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("AdminUsers")]
        public IEnumerable<AdminUser> GetAdminUsers()
        {
            var result = new List<AdminUser>();
            foreach (var item in GameManager.Instance.adminTools.Users.GetUsers().Values)
            {
                result.Add(new AdminUser()
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
        [Route("AdminUsers")]
        public IEnumerable<string> RemoveAdminUsers([FromUri, Required] string[] playerIds)
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

        #region Command Permissions
        /// <summary>
        /// Creates a new command permission based on the specified model and returns the result messages.
        /// </summary>
        /// <param name="model">An object containing the command name and the desired permission level. Must not be null.</param>
        /// <returns>An enumerable collection of strings containing the result messages from the command execution.</returns>
        [HttpPost]
        [Route("CommandPermissions")]
        public IEnumerable<string> CreateCommandPermission([FromBody, Required] CommandPermissionCreate model)
        {
            string command = $"cp add {model.Command} {model.PermissionLevel}";
            var result = SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate);
            return result;
        }

        /// <summary>
        /// Retrieves a collection of available commands and their associated permission levels.
        /// </summary>
        /// <remarks>Use this method to enumerate all commands exposed by the system, along with their
        /// descriptions and required permission levels. This is typically used for administrative interfaces or
        /// permission management tools.</remarks>
        /// <returns>An enumerable collection of <see cref="CommandPermission"/> objects, each representing a command and its
        /// required permission level. The collection will be empty if no commands are available.</returns>
        [HttpGet]
        [Route("CommandPermissions")]
        public IEnumerable<CommandPermission> GetCommandPermissions()
        {
            var result = new List<CommandPermission>();
            foreach (var item in GameManager.Instance.adminTools.Commands.GetCommands().Values)
            {
                result.Add(new CommandPermission()
                {
                    Command = item.Command,
                    PermissionLevel = item.PermissionLevel,
                    Description = SdtdConsole.Instance.GetCommand(item.Command)?.GetDescription(),
                });
            }
            return result;
        }

        /// <summary>
        /// Removes permissions for the specified commands and returns the results of each removal operation.
        /// </summary>
        /// <param name="commands">An array of command names for which permissions should be removed. Cannot be null or empty.</param>
        /// <returns>An enumerable collection of strings containing the results of each command permission removal. The
        /// collection will be empty if no commands are provided.</returns>
        [HttpDelete]
        [Route("CommandPermissions")]
        public IEnumerable<string> RemoveCommandPermissions([FromUri, Required] string[] commands)
        {
            var result = new List<string>();
            foreach (var item in commands)
            {
                string command = $"cp remove {item}";
                result.AddRange(SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate));
            }
            return result;
        }
        #endregion

        #region Bans
        /// <summary>
        /// Creates a new ban entry for a player using the specified ban details.
        /// </summary>
        /// <remarks>The ban duration is calculated based on the difference between the current time and
        /// the specified end time. The method executes the ban command synchronously and returns any output generated
        /// by the command.</remarks>
        /// <param name="model">An object containing the details of the ban, including the player's ID, the ban duration, reason, and
        /// display name. Must not be null.</param>
        /// <returns>An enumerable collection of strings containing the results of the ban command execution.</returns>
        [HttpPost]
        [Route("Bans")]
        public IEnumerable<string> CreateBan([FromBody, Required] BanEntry model)
        {
            string command = $"ban add {model.PlayerId} {(int)(model.BannedUntil - DateTime.UtcNow).TotalMinutes} minutes {Utils.FormatCommandArgs(model.Reason)} {Utils.FormatCommandArgs(model.DisplayName)}";
            return SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate);
        }

        /// <summary>
        /// Retrieves a collection of ban entries representing players who are currently banned from the game.
        /// </summary>
        /// <remarks>Each ban entry includes the player's identifier, ban expiration time, reason for the
        /// ban, and display name. This method is typically used by administrators to review active bans.</remarks>
        /// <returns>An enumerable collection of <see cref="BanEntry"/> objects, each containing details about a banned player.
        /// The collection will be empty if no players are currently banned.</returns>
        [HttpGet]
        [Route("Bans")]
        public IEnumerable<BanEntry> GetBans()
        {
            var result = new List<BanEntry>();
            foreach (var item in GameManager.Instance.adminTools.Blacklist.GetBanned())
            {
                result.Add(new BanEntry()
                {
                    PlayerId = item.UserIdentifier.CombinedString,
                    BannedUntil = item.BannedUntil,
                    Reason = item.BanReason,
                    DisplayName = item.Name
                });
            }

            return result;
        }

        /// <summary>
        /// Removes bans for the specified players and returns the results of each ban removal operation.
        /// </summary>
        /// <remarks>This method executes a ban removal command for each provided player ID. The returned
        /// collection contains the output messages from the underlying command execution. The order of results
        /// corresponds to the order of the input player IDs.</remarks>
        /// <param name="playerIds">An array of player IDs for which bans should be removed. Cannot be null or empty.</param>
        /// <returns>An enumerable collection of strings containing the results of each ban removal command. Each string
        /// represents the output from the ban removal operation for a player.</returns>
        [HttpDelete]
        [Route("Bans")]
        public IEnumerable<string> RemoveBans([FromUri, Required] string[] playerIds)
        {
            var result = new List<string>();
            foreach (var item in playerIds)
            {
                string command = $"ban remove {item}";
                result.AddRange(SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate));
            }
            return result;
        }
        #endregion

        #region Whitelist
        /// <summary>
        /// Adds a new entry to the whitelist using the specified player information.
        /// </summary>
        /// <remarks>This method executes a server command to add the player to the whitelist. The
        /// returned collection contains the output messages from the server, which may include success or error
        /// information.</remarks>
        /// <param name="model">The whitelist entry containing the player's ID and display name to be added. Cannot be null.</param>
        /// <returns>An enumerable collection of strings containing the results of the whitelist command execution.</returns>
        [HttpPost]
        [Route("Whitelist")]
        public IEnumerable<string> CreateWhitelistEntry([FromBody, Required] WhitelistEntry model)
        {
            string command = $"whitelist add {model.PlayerId} {Utils.FormatCommandArgs(model.DisplayName)}";
            var result = SdtdConsole.Instance.ExecuteSync(command, ModMain.CmdExecuteDelegate);
            return result;
        }

        /// <summary>
        /// Retrieves the list of all users currently included in the whitelist.
        /// </summary>
        /// <remarks>This method is typically used to display or manage the whitelist for administrative
        /// purposes. The returned entries include both the player's unique identifier and display name.</remarks>
        /// <returns>An enumerable collection of <see cref="WhitelistEntry"/> objects representing each whitelisted user. The
        /// collection will be empty if no users are whitelisted.</returns>
        [HttpGet]
        [Route("Whitelist")]
        public IEnumerable<WhitelistEntry> GetWhitelistEntries()
        {
            var result = new List<WhitelistEntry>();
            foreach (var item in GameManager.Instance.adminTools.Whitelist.GetUsers().Values)
            {
                result.Add(new WhitelistEntry()
                {
                    PlayerId = item.UserIdentifier.CombinedString,
                    DisplayName = item.Name,
                });
            }
            return result;
        }

        /// <summary>
        /// Removes the specified players from the server whitelist.
        /// </summary>
        /// <remarks>This method executes a removal command for each provided player ID. The returned
        /// collection may include error messages if a player ID is not found or if removal fails.</remarks>
        /// <param name="playerIds">An array of player IDs to remove from the whitelist. Cannot be null.</param>
        /// <returns>A collection of strings containing the results of each whitelist removal command. Each entry represents the
        /// server's response for the corresponding player ID.</returns>
        [HttpDelete]
        [Route("Whitelist")]
        public IEnumerable<string> RemoveWhitelistEntries([FromUri, Required] string[] playerIds)
        {
            var result = new List<string>();
            foreach (var item in playerIds)
            {
                string command = $"whitelist remove {item}";
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
        /// e.g. airConditioner.png or airConditioner__00FF00.png    Color is optional
        /// </remarks>
        /// <param name="name">Can be either an icon name or an item name. If it's an icon name, the suffix must be .png. Color is optional. See example for format.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 7200)]
        [Route("ItemIcons/{name}")]
        [AllowAnonymous]
        public IHttpActionResult GetItemIcon(string name)
        {
            return InternalGetIcon(name, false);
        }

        /// <summary>
        /// Get UI Icon
        /// </summary>
        /// <remarks>
        /// e.g. Button.png or Button__00FF00.png    Color is optional
        /// </remarks>
        /// <param name="name">Can be either an icon name or an item name. If it's an icon name, the suffix must be .png. Color is optional. See example for format.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseCache(Duration = 7200)]
        [Route("UiIcons/{name}")]
        [AllowAnonymous]
        public IHttpActionResult GetUiIcon(string name)
        {
            return InternalGetIcon(name, true);
        }

        private IHttpActionResult InternalGetIcon(string name, bool isUiIcon)
        {
            if (name.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false)
            {
                return BadRequest("Invalid icon name.");
            }

            int len = name.Length;
            if (len > 12 && name[len - 11] == '_' && name[len - 12] == '_')
            {
                string iconColor = name.Substring(len - 10, 6);
                name = name.Substring(0, len - 12) + ".png";

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

                string? iconPath = FindIconPath(name, isUiIcon);
                return iconPath == null ? NotFound() : GetIconWithColor(iconPath, r, g, b);
            }
            else
            {
                string? iconPath = FindIconPath(name, isUiIcon);
                return iconPath == null ? NotFound() : new FileContentResult(iconPath, "image/png");
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

        #region Map
        /// <summary>
        /// Get map info.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("MapInfo")]
        public MapInfo GetMapInfo()
        {
            var mapInfo = new MapInfo()
            {
                BlockSize = MapRendering.Constants.MapBlockSize,
                MaxZoom = MapRendering.Constants.Zoomlevels - 1
            };
            return mapInfo;
        }

        /// <summary>
        /// Get map tile.
        /// </summary>
        /// <param name="z">zoom</param>
        /// <param name="x"></param>
        /// <param name="y">y</param>
        /// <returns></returns>
        [HttpGet]
        [Route("MapTile/{z:int}/{x:int}/{y:int}")]
        public IHttpActionResult GetMapTile(int z, int x, int y)
        {
            string fileName = MapRendering.Constants.MapDirectory + $"/{z}/{x}/{y}.png";

            if (File.Exists(fileName))
            {
                return new FileStreamResult(File.OpenRead(fileName), "image/png");
            }

            if (ModMain.MapTileCache == null)
            {
                return NotFound();
            }

            byte[] data = ModMain.MapTileCache.GetFileContent(fileName);
            return new FileContentResult(data, "image/png");
        }

        /// <summary>
        /// Render full map.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RenderFullMap")]
        [SwaggerResponse(typeof(IEnumerable<string>))]
        public async Task<IHttpActionResult> RenderFullMap()
        {
            var result = await Utils.ExecuteConsoleCommandAsync("visitmap full");
            return Ok(result);
        }

        /// <summary>
        /// Render explored area.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("RenderExploredArea")]
        [SwaggerResponse(typeof(IEnumerable<string>))]
        public async Task<IHttpActionResult> RenderExploredArea()
        {
            var result = await Utils.ExecuteConsoleCommandAsync("rendermap");
            return Ok(result);
        }
        #endregion

        #region Locations
        /// <summary>
        /// Get locations by entity type.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Locations")]
        public IEnumerable<EntityBasicInfo> GetLocations(Shared.Models.EntityType entityType)
        {
            var locations = new List<EntityBasicInfo>();

            if (entityType == Shared.Models.EntityType.OfflinePlayer)
            {
                var online = GameManager.Instance.World.Players.list.Select(i => ConnectionManager.Instance.Clients.ForEntityId(i.entityId).InternalId).ToHashSet();
                foreach (var item in GameManager.Instance.GetPersistentPlayerList().Players)
                {
                    if (online.Contains(item.Key) == false)
                    {
                        var player = item.Value;
                        locations.Add(new EntityBasicInfo()
                        {
                            EntityId = player.EntityId,
                            EntityName = player.PlayerName.DisplayName,
                            Position = player.Position.ToPosition(),
                            EntityType = Shared.Models.EntityType.OfflinePlayer,
                            PlayerId = player.PrimaryId.CombinedString,
                        });
                    }
                }
            }
            else if (entityType == Shared.Models.EntityType.OnlinePlayer)
            {
                foreach (var player in GameManager.Instance.World.Players.list)
                {
                    locations.Add(new EntityBasicInfo()
                    {
                        EntityId = player.entityId,
                        EntityName = player.EntityName,
                        Position = player.GetPosition().ToPosition(),
                        EntityType = Shared.Models.EntityType.OnlinePlayer,
                        PlayerId = ConnectionManager.Instance.Clients.ForEntityId(player.entityId).InternalId.CombinedString,
                    });
                }
            }
            else if (entityType == Shared.Models.EntityType.Animal)
            {
                foreach (var entity in GameManager.Instance.World.Entities.list)
                {
                    if (entity is EntityAnimal entityAnimal && entity.IsAlive())
                    {
                        locations.Add(new EntityBasicInfo()
                        {
                            EntityId = entityAnimal.entityId,
                            EntityName = entityAnimal.EntityName ?? ("animal class #" + entityAnimal.entityClass),
                            Position = entityAnimal.GetPosition().ToPosition(),
                            EntityType = Shared.Models.EntityType.Animal,
                        });
                    }
                }
            }
            else if (entityType == Shared.Models.EntityType.Hostiles)
            {
                foreach (var entity in GameManager.Instance.World.Entities.list)
                {
                    if (entity is EntityEnemy entityEnemy && entity.IsAlive())
                    {
                        locations.Add(new EntityBasicInfo()
                        {
                            EntityId = entityEnemy.entityId,
                            EntityName = entityEnemy.EntityName ?? ("enemy class #" + entityEnemy.entityClass),
                            Position = entityEnemy.GetPosition().ToPosition(),
                            EntityType = (Shared.Models.EntityType)entityEnemy.entityType
                        });
                    }
                }
            }
            else if (entityType == Shared.Models.EntityType.Zombie)
            {
                foreach (var entity in GameManager.Instance.World.Entities.list)
                {
                    if (entity is EntityZombie entityZombie && entity.IsAlive())
                    {
                        locations.Add(new EntityBasicInfo()
                        {
                            EntityId = entityZombie.entityId,
                            EntityName = entityZombie.EntityName ?? ("zombie class #" + entityZombie.entityClass),
                            Position = entityZombie.GetPosition().ToPosition(),
                            EntityType = (Shared.Models.EntityType)entityZombie.entityType
                        });
                    }
                }
            }
            else if (entityType == Shared.Models.EntityType.Bandit)
            {
                foreach (var entity in GameManager.Instance.World.Entities.list)
                {
                    if (entity is EntityBandit entityBandit && entity.IsAlive())
                    {
                        locations.Add(new EntityBasicInfo()
                        {
                            EntityId = entityBandit.entityId,
                            EntityName = entityBandit.EntityName ?? ("bandit class #" + entityBandit.entityClass),
                            Position = entityBandit.GetPosition().ToPosition(),
                            EntityType = (Shared.Models.EntityType)entityBandit.entityType
                        });
                    }
                }
            }

            return locations;
        }

        /// <summary>
        /// Get locations by entity id.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Locations/{entityId:int}")]
        [SwaggerResponse(typeof(EntityBasicInfo))]
        public IHttpActionResult GetLocation(int entityId)
        {
            if (GameManager.Instance.World.Players.dict.TryGetValue(entityId, out var player))
            {
                return Ok(new EntityBasicInfo()
                {
                    EntityId = player.entityId,
                    EntityName = player.EntityName,
                    Position = player.GetPosition().ToPosition(),
                    EntityType = Shared.Models.EntityType.OnlinePlayer,
                    PlayerId = ConnectionManager.Instance.Clients.ForEntityId(player.entityId)?.CrossplatformId.CombinedString,
                });
            }

            if (GameManager.Instance.World.Entities.dict.TryGetValue(entityId, out var entity))
            {
                string entityName = (entity is EntityAlive entityAlive) ? entityAlive.EntityName : "entity class #" + entity.entityClass;
                return Ok(new EntityBasicInfo()
                {
                    EntityId = entity.entityId,
                    EntityName = entityName,
                    Position = entity.GetPosition().ToPosition(),
                    EntityType = (Shared.Models.EntityType)entity.entityType,
                });
            }

            return NotFound();
        }
        #endregion

        #region Localization
        /// <summary>
        /// Get all localization strings for a specified language.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Localization")]
        [ResponseCache(Duration = 7200)]
        [SwaggerResponse(typeof(Dictionary<string, string>))]
        public IHttpActionResult GetLocalization(Language language)
        {
            string _language = language.ToString().ToLower();
            var dict = Localization.dictionary;
            int languageIndex = Array.LastIndexOf(dict["KEY"], _language);

            if (languageIndex < 0)
            {
                return NotFound();
            }

            return Ok(dict.ToDictionary(p => p.Key, p => p.Value[languageIndex]));
        }

        /// <summary>
        /// Get a specific localization string by key and language.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="language">language</param>
        /// <param name="caseInsensitive"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Localization/{key}")]
        [ResponseCache(Duration = 7200)]
        [SwaggerResponse(typeof(string))]
        public IHttpActionResult GetLocalization(string key, [FromUri] Language language, [FromUri] bool caseInsensitive = false)
        {
            return Ok(Utils.GetLocalization(key, language, caseInsensitive));
        }

        /// <summary>
        /// Get all known languages.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("KnownLanguages")]
        [ResponseCache(Duration = 7200)]
        [SwaggerResponse(typeof(string))]
        public string[] GetKnownLanguages()
        {
            return Localization.dictionary["KEY"];
        }
        #endregion

        #region LandClaims
        /// <summary>
        /// Get land claims by player id.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LandClaims/{playerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ClaimOwner))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(object))]
        public IHttpActionResult GetLandClaims(string playerId)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }

            var persistentPlayerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(userId);
            if (persistentPlayerData == null)
            {
                return NotFound();
            }

            var claimOwner = new ClaimOwner()
            {
                EntityId = persistentPlayerData.EntityId,
                PlatformId = persistentPlayerData.NativeId.CombinedString,
                PlayerId = persistentPlayerData.PrimaryId.CombinedString,
                PlayerName = persistentPlayerData.PlayerName.DisplayName,
                ClaimActive = GameManager.Instance.World.IsLandProtectionValidForPlayer(persistentPlayerData),
                ClaimPositions = persistentPlayerData.LPBlocks.ToPositions(),
                LastLogin = persistentPlayerData.LastLogin,
                Position = persistentPlayerData.Position.ToPosition()
            };

            return Ok(claimOwner);
        }

        /// <summary>
        /// Get all land claims.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LandClaims")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(LandClaims))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(object))]
        public LandClaims GetLandClaims()
        {
            var claimOwners = new List<ClaimOwner>();

            foreach (var item in GameManager.Instance.GetPersistentPlayerList().Players)
            {
                var persistentPlayerData = item.Value;
                if (persistentPlayerData != null && persistentPlayerData.LPBlocks != null)
                {
                    claimOwners.Add(new ClaimOwner()
                    {
                        EntityId = persistentPlayerData.EntityId,
                        PlatformId = persistentPlayerData.NativeId.CombinedString,
                        PlayerId = persistentPlayerData.PrimaryId.CombinedString,
                        PlayerName = persistentPlayerData.PlayerName.DisplayName,
                        ClaimActive = GameManager.Instance.World.IsLandProtectionValidForPlayer(persistentPlayerData),
                        ClaimPositions = persistentPlayerData.LPBlocks.ToPositions(),
                        LastLogin = persistentPlayerData.LastLogin,
                        Position = persistentPlayerData.Position.ToPosition()
                    });
                }
            }

            return new LandClaims()
            {
                ClaimOwners = claimOwners,
                ClaimSize = GamePrefs.GetInt(EnumGamePrefs.LandClaimSize)
            };
        }

        /// <summary>
        /// Remove land claims by player id.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("LandClaims/{playerId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<string>))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(object))]
        public async Task<IHttpActionResult> RemovePlayerLandClaim(string playerId)
        {
            var userId = PlatformUserIdentifierAbs.FromCombinedString(playerId);
            if (userId == null)
            {
                return NotFound();
            }
            var persistentPlayerData = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(userId);
            if (persistentPlayerData == null)
            {
                return NotFound();
            }

            var result = await Utils.ExecuteConsoleCommandAsync($"ty-rplc {userId.CombinedString}");
            return Ok(result);
        }

        /// <summary>
        /// Remove land claim by block position.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("LandClaims")]
        [SwaggerResponse(typeof(IEnumerable<string>))]
        public async Task<IHttpActionResult> RemovePlayerLandClaim([FromBody] Position position)
        {
            var result = await Utils.ExecuteConsoleCommandAsync($"ty-rplc {position}");
            return Ok(result);
        }
        #endregion

        #region ServerSettings
        /// <summary>
        /// Get server settings.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ServerSettings")]
        [SwaggerResponse(typeof(Dictionary<string, string>))]
        public HttpResponseMessage Settings()
        {
            var settingsMap = new Dictionary<string, string>();

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
                    settingsMap.Add(name, value);
                }
            }

            var serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
            };

            string json = JsonConvert.SerializeObject(settingsMap, serializerSettings);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            return response;
        }

        /// <summary>
        /// Update server settings.
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("ServerSettings")]
        public IHttpActionResult Settings([FromBody] Dictionary<string, string> model)
        {
            string path = Path.Combine(AppContext.BaseDirectory, AppConfig.Settings.ServerConfigFile);
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            var rootNode = xmlDocument.SelectSingleNode("ServerSettings");

            foreach (var item in model)
            {
                var node = rootNode.SelectSingleNode($"property[@name='{item.Key}']");
                if (node != null && node.Attributes != null)
                {
                    node.Attributes["value"].Value = item.Value;
                }
            }

            xmlDocument.Save(path);
            return Ok();
        }
        #endregion

        #region Mods
        /// <summary>
        /// Retrieves information about all available mods by scanning the Mods directory.
        /// </summary>
        /// <remarks>Each mod is identified by the presence of a ModInfo.xml or _ModInfo.xml file within
        /// its folder. Only mods with valid metadata files are included in the result. The returned collection reflects
        /// the current state of the Mods directory at the time of the request.</remarks>
        /// <returns>An enumerable collection of <see cref="Shared.Models.ModInfo"/> objects, each representing a mod found in
        /// the Mods directory. Returns an empty collection if no mods are available.</returns>
        [HttpGet]
        [Route("Mods")]
        public IEnumerable<Shared.Models.ModInfo> GetMods()
        {
            var mods = new List<Shared.Models.ModInfo>();

            string modPath = Path.Combine(AppContext.BaseDirectory, "Mods");
            if (Directory.Exists(modPath) == false)
            {
                return Array.Empty<Shared.Models.ModInfo>();
            }

            foreach (var dir in Directory.GetDirectories(modPath))
            {
                bool isUninstalled = false;
                string path = Path.Combine(dir, "ModInfo.xml");
                if (File.Exists(path) == false)
                {
                    path = Path.Combine(dir, "_ModInfo.xml");
                    if (File.Exists(path) == false)
                    {
                        continue;
                    }
                    else
                    {
                        isUninstalled = true;
                    }
                }

                var xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                var rootNode = xmlDocument.DocumentElement;

                string GetValue(string nodeName)
                {
                    var node = rootNode.SelectSingleNode(nodeName);
                    return node?.Attributes?["value"]?.Value ?? string.Empty;
                }

                var mod = new Shared.Models.ModInfo()
                {
                    Name = GetValue("Name"),
                    DisplayName = GetValue("DisplayName"),
                    Author = GetValue("Author"),
                    Description = GetValue("Description"),
                    Version = GetValue("Version"),
                    Website = GetValue("Website"),
                    FolderName = new DirectoryInfo(dir).Name,
                };
                mod.IsLoaded = ModManager.loadedMods.dict.ContainsKey(mod.Name);
                mod.IsUninstalled = isUninstalled;
                mods.Add(mod);
            }

            return mods;
        }

        //private bool IsSingleRootFolder(string tempZipPath, out string? rootFolderName)
        //{
        //    rootFolderName = null;
        //    try
        //    {
        //        using (var archive = ZipFile.OpenRead(tempZipPath))
        //        {
        //            var topLevelItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        //            foreach (var entry in archive.Entries.Where(e => !string.IsNullOrEmpty(e.Name)))
        //            {
        //                var parts = entry.FullName.Split(new char[] { '/', '\\' },
        //                                                  2,
        //                                                  StringSplitOptions.RemoveEmptyEntries);

        //                if (parts.Length > 0)
        //                {
        //                    topLevelItems.Add(parts[0].TrimEnd('/', '\\'));
        //                }
        //            }

        //            if (topLevelItems.Count == 1)
        //            {
        //                rootFolderName = topLevelItems.First();
        //                return true;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }

        //    return false;
        //}

        ///// <summary>
        ///// Handles a file upload via multipart/form-data and extracts the uploaded ZIP file to the server,
        ///// conditionally creating a target directory based on the ZIP's structure.
        ///// </summary>
        ///// <remarks>If the uploaded ZIP contains a single root folder, its contents are extracted
        ///// directly into the Mods directory. Otherwise, a new subdirectory is created under Mods using the ZIP file
        ///// name. Existing directories with the same name will cause the operation to fail with a conflict response. The
        ///// method deletes the temporary uploaded file after extraction. This endpoint expects requests with
        ///// 'multipart/form-data' content type.</remarks>
        ///// <returns>An HTTP response indicating the result of the upload and extraction operation. Returns 200 (OK) if
        ///// extraction succeeds, 400 (Bad Request) if no file is uploaded, 409 (Conflict) if the target directory
        ///// already exists, 415 (Unsupported Media Type) if the request content type is invalid, or 500 (Internal Server
        ///// Error) if an error occurs during upload or extraction.</returns>
        //[HttpPost]
        //[Route("Mods")]
        //public async Task<HttpResponseMessage> UploadAndConditionalUnzip(MultipartFileData multipartFileData)
        //{
        //    if (Request.Content.IsMimeMultipartContent() == false)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Unsupported content type.");
        //    }

        //    string extractionBasePath = Path.Combine(AppContext.BaseDirectory, "Mods");
        //    string tempFolder = Path.GetTempPath();
        //    if (Directory.Exists(extractionBasePath) == false)
        //    {
        //        Directory.CreateDirectory(extractionBasePath);
        //    }
        //    if (Directory.Exists(tempFolder) == false)
        //    {
        //        Directory.CreateDirectory(tempFolder);
        //    }

        //    var provider = new MultipartFormDataStreamProvider(tempFolder);
        //    string tempFilePath = string.Empty;

        //    try
        //    {
        //        await Request.Content.ReadAsMultipartAsync(provider);
        //        var fileData = provider.FileData.FirstOrDefault();

        //        if (fileData == null) return Request.CreateResponse(HttpStatusCode.BadRequest, "No file uploaded.");

        //        tempFilePath = fileData.LocalFileName;
        //        string originalFileName = fileData.Headers.ContentDisposition.FileName.Trim('"');
        //        string zipNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFileName);

        //        bool isSingleRoot = IsSingleRootFolder(tempFilePath, out string? rootFolderName);

        //        string targetDirectory;
        //        if (isSingleRoot)
        //        {
        //            targetDirectory = extractionBasePath;
        //            string rootPathInBase = Path.Combine(targetDirectory, rootFolderName!);
        //            if (Directory.Exists(rootPathInBase))
        //            {
        //                return Request.CreateResponse(HttpStatusCode.Conflict,
        //                    $"Extraction aborted. The target directory '{rootFolderName}' already exists.");
        //            }
        //        }
        //        else
        //        {
        //            targetDirectory = Path.Combine(extractionBasePath, zipNameWithoutExtension);
        //            if (Directory.Exists(targetDirectory))
        //            {
        //                return Request.CreateResponse(HttpStatusCode.Conflict,
        //                    $"Extraction aborted. The target directory '{zipNameWithoutExtension}' already exists.");
        //            }
        //        }

        //        ZipFile.ExtractToDirectory(tempFilePath, targetDirectory);
        //        File.Delete(tempFilePath);

        //        return Request.CreateResponse(HttpStatusCode.OK,
        //            $"ZIP file '{originalFileName}' successfully extracted. Single-Root: {isSingleRoot}. Final Path: {targetDirectory}");
        //    }
        //    catch (Exception e)
        //    {
        //        if (string.IsNullOrEmpty(tempFilePath) == false && File.Exists(tempFilePath))
        //        {
        //            File.Delete(tempFilePath);
        //        }
        //        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "File upload or decompression failed.", e);
        //    }
        //}

        /// <summary>
        /// Toggles the activation status of a mod by renaming its ModInfo.xml file within the specified folder.
        /// </summary>
        /// <remarks>This method enables or disables a mod by renaming its ModInfo.xml file to
        /// _ModInfo.xml or vice versa. The mod folder must exist within the application's Mods directory. The operation
        /// is atomic and does not modify the contents of the mod files.</remarks>
        /// <param name="folderName">The name of the mod folder to update. Cannot be null, empty, or consist only of whitespace.</param>
        /// <returns>An HTTP response indicating the result of the operation: 200 OK if the status was toggled successfully; 400
        /// Bad Request if the folder name is invalid; 404 Not Found if the folder or mod info file does not exist; or
        /// 500 Internal Server Error if an unexpected error occurs.</returns>
        [HttpPut]
        [Route("Mods")]
        public IHttpActionResult ToggleModStatus([FromUri] string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                return BadRequest("Folder name cannot be empty.");
            }
            string modPath = Path.Combine(AppContext.BaseDirectory, "Mods", folderName);
            if (Directory.Exists(modPath) == false)
            {
                return NotFound();
            }
            try
            {
                string oldPath = Path.Combine(modPath, "ModInfo.xml");
                string newPath = Path.Combine(modPath, "_ModInfo.xml");

                if (File.Exists(oldPath))
                {
                    File.Move(oldPath, newPath);
                }
                else if (File.Exists(newPath))
                {
                    File.Move(newPath, oldPath);
                }
                else
                {
                    return NotFound();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Failed to delete mod folder.", ex));
            }
        }
        #endregion
    }
}
