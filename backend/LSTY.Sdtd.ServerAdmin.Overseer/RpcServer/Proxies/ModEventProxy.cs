﻿using LSTY.Sdtd.ServerAdmin.Overseer.Extensions;
using LSTY.Sdtd.ServerAdmin.Shared.Constants;
using LSTY.Sdtd.ServerAdmin.Shared.EventArgs;
using LSTY.Sdtd.ServerAdmin.Shared.Models;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;
using System.Text;
using static ModEvents;

namespace LSTY.Sdtd.ServerAdmin.Overseer.RpcServer.Proxies
{
    public class ModEventProxy : IModEventProxy
    {
        #region Event Handlers

        /// <summary>
        /// Event that is triggered when a log entry is received.
        /// </summary>
        public event EventHandler<LogCallbackEventArgs>? LogCallback;

        /// <summary>
        /// Event that is triggered when the game is awake and ready for interaction.
        /// </summary>
        public event EventHandler? GameAwake;

        /// <summary>
        /// Event that is triggered when the game has finished starting and players can join.
        /// </summary>
        public event EventHandler? GameStartDone;

        /// <summary>
        /// Event that is triggered when the game is about to shut down.
        /// </summary>
        public event EventHandler? GameShutdown;

        //public event Action CalcChunkColorsDone;

        /// <summary>
        /// Event that is triggered when a chat message is received.
        /// </summary>
        public event EventHandler<ChatMessageEventArgs>? ChatMessage;

        /// <summary>
        /// Event that is triggered when an entity is killed.
        /// </summary>
        public event EventHandler<EntityKilledEventArgs>? EntityKilled;

        /// <summary>
        /// Event that is triggered when an entity is spawned.
        /// </summary>
        public event EventHandler<EntitySpawnedEventArgs>? EntitySpawned;

        /// <summary>
        /// Event that is triggered when a player disconnects.
        /// </summary>
        public event EventHandler<PlayerDisconnectedEventArgs>? PlayerDisconnected;

        /// <summary>
        /// Event that is triggered when a player logs in.
        /// </summary>
        public event EventHandler<PlayerLoginEventArgs>? PlayerLogin;

        /// <summary>
        /// Event that is triggered when a player is spawned in the world.
        /// </summary>
        public event EventHandler<PlayerSpawnedInWorldEventArgs>? PlayerSpawnedInWorld;

        /// <summary>
        /// Event that is triggered when a player is about to spawn in the world.
        /// </summary>
        public event EventHandler<PlayerSpawningEventArgs>? PlayerSpawning;

        /// <summary>
        /// Event that is triggered when player data is saved.
        /// </summary>
        public event EventHandler<SavePlayerDataEventArgs>? SavePlayerData;

        /// <summary>
        /// Event that is triggered when the sky changes.
        /// </summary>
        public event EventHandler<SkyChangedEventArgs>? SkyChanged;
        #endregion

        /// <summary>
        /// Runs when LogCallback has been called.
        /// </summary>
        /// <param name="message">The log message.</param>
        /// <param name="trace">The stack trace.</param>
        /// <param name="type">The log type.</param>
        internal void OnLogCallback(string message, string trace, UnityEngine.LogType type)
        {
            var logEntry = new LogCallbackEventArgs()
            {
                Message = message,
                StackTrace = trace,
                LogLevel = (LogLevel)type,
                Timestamp = DateTime.UtcNow,
            };
            LogCallback?.Invoke(this, logEntry);
        }

        /// <summary>
        /// Runs once when the server is ready for interaction and GameManager.Instance.World is set.
        /// </summary>
        internal void OnGameAwake(ref SGameAwakeData sGameAwakeData)
        {
            GameAwake?.Invoke(this, null);
        }

        /// <summary>
        /// Runs once when the server is ready for players to join.
        /// </summary>
        internal void OnGameStartDone(ref SGameStartDoneData sGameStartDoneData)
        {
            GameStartDone?.Invoke(this, null);
        }

        /// <summary>
        /// Runs once when the server is about to shut down.
        /// </summary>
        internal void OnGameShutdown(ref SGameShutdownData sGameShutdownData)
        {
            GameShutdown?.Invoke(this, null);
        }

        ///// <summary>
        ///// Runs each time a chunk has its colors re-calculated. For example, this is used to generate the images for Allocs Game Map mod.
        ///// </summary>
        ///// <param name="chunk">The chunk.</param>
        //internal void OnCalcChunkColorsDone(Chunk chunk)
        //{
        //    CalcChunkColorsDone?.Invoke(chunk);
        //}

        /// <summary>
        /// Handles a chat message.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="eChatType">The chat type.</param>
        /// <param name="senderEntityId">The sender entity ID.</param>
        /// <param name="message">The chat message.</param>
        /// <param name="mainName">The main name.</param>
        /// <param name="recipientEntityIds">The recipient entity IDs.</param>
        /// <returns>True to pass the message on to the next mod or output to chat, false to prevent the message from being passed on or output to chat.</returns>
        public EModEventResult OnChatMessage(ref SChatMessageData sChatMessageData)
        {
            var clientInfo = sChatMessageData.ClientInfo;
            string message = sChatMessageData.Message;
            int senderEntityId = sChatMessageData.SenderEntityId;
            string? mainName = sChatMessageData.MainName;
            EChatType eChatType = sChatMessageData.ChatType;
            var recipientEntityIds = sChatMessageData.RecipientEntityIds;

            string senderName;
            if (clientInfo == ModMain.CmdExecuteDelegate)
            {
                string[] parts = message.Split(new string[] { ": " }, 2, StringSplitOptions.None);
                senderName = parts[0];
                message = parts[1];
            }
            else if (senderEntityId == -1)
            {
                senderName = Localization.Get("xuiChatServer", false);
            }
            else
            {
                senderName = mainName ?? Common.NonPlayer;
            }

            var chatMessage = new ChatMessageEventArgs()
            {
                EntityId = senderEntityId,
                PlayerId = clientInfo?.CrossplatformId.CombinedString,
                ChatType = (ChatType)eChatType,
                Message = message,
                SenderName = senderName,
                Timestamp = DateTime.UtcNow,
                RecipientEntityIds = recipientEntityIds
            };

            ChatMessage?.Invoke(this, chatMessage);

            return EModEventResult.Continue;
        }

        /// <summary>
        /// Runs on initial connection from a player. The client info is usually null at this point.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="compatibilityVersion">The compatibility version.</param>
        /// <param name="stringBuilder">The string builder for kick player.</param>
        /// <returns>True to allow the player to log in, false otherwise.</returns>
        public EModEventResult OnPlayerLogin(ref SPlayerLoginData sPlayerLoginData)
        {
            PlayerLogin?.Invoke(this, new PlayerLoginEventArgs()
            {
                PlayerInfo = sPlayerLoginData.ClientInfo.ToPlayerInfo(),
                CompatibilityVersion = sPlayerLoginData.CustomMessage,
                CustomMessage = sPlayerLoginData.CustomMessage,
                Timestamp = DateTime.UtcNow,
            });

            return EModEventResult.Continue;
        }

        /// <summary>
        /// Runs when an entity is killed.
        /// </summary>
        /// <param name="victim">The killed entity.</param>
        /// <param name="killer">The entity that killed the entity.</param>
        internal void OnEntityKilled(ref SEntityKilledData sEntityKilledData)
        {
            var victim = sEntityKilledData.KilledEntitiy;
            var killer = sEntityKilledData.KillingEntity;

            if (EntityKilled == null)
            {
                return;
            }

            if (victim is EntityAlive diedEntity
                && killer is EntityPlayer entityPlayer
                && killer.IsClientControlled())
            {
                EntityKilled.Invoke(this, new EntityKilledEventArgs() 
                { 
                    Victim = diedEntity.ToEntityInfo(), 
                    Killer = entityPlayer.ToEntityInfo(),
                    Timestamp = DateTime.UtcNow,
                });
            }
        }

        /// <summary>
        /// Runs when an entity is spawned.
        /// </summary>
        /// <param name="entity">The spawned entity.</param>
        internal void OnEntitySpawned(EntityInfo entity)
        {
            EntitySpawned?.Invoke(this, new EntitySpawnedEventArgs() 
            { 
                SpawnedEntity = entity,
                Timestamp = DateTime.UtcNow,
            });
        }

        /// <summary>
        /// Runs on each player disconnect.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="shutdown">Indicates if the server is shutting down.</param>
        internal void OnPlayerDisconnected(ref SPlayerDisconnectedData sPlayerDisconnectedData)
        {
            PlayerDisconnected?.Invoke(this, new PlayerDisconnectedEventArgs() 
            { 
                PlayerInfo = sPlayerDisconnectedData.ClientInfo.ToPlayerInfo(),
                GameShuttingDown = sPlayerDisconnectedData.GameShuttingDown,
                Timestamp = DateTime.UtcNow,
            });
        }


        /// <summary>
        /// Runs each time a player spawns, including on login, respawn from death, and teleport.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="respawnType">The respawn type.</param>
        /// <param name="position">The position.</param>
        internal void OnPlayerSpawnedInWorld(ref SPlayerSpawnedInWorldData sPlayerSpawnedInWorldData)
        {
            PlayerSpawnedInWorld?.Invoke(this, new PlayerSpawnedInWorldEventArgs()
            {
                PlayerInfo = sPlayerSpawnedInWorldData.ClientInfo.ToPlayerInfo(sPlayerSpawnedInWorldData.Position.ToPosition()),
                RespawnType = (Shared.Models.RespawnType)sPlayerSpawnedInWorldData.RespawnType,
                Timestamp = DateTime.UtcNow,
            });
        }

        /// <summary>
        /// Runs just before a player is spawned in the world.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="chunkViewDim">The chunk view dimension.</param>
        /// <param name="playerProfile">The player profile.</param>
        internal void OnPlayerSpawning(ref SPlayerSpawningData sPlayerSpawningData)
        {
            PlayerSpawning?.Invoke(this, new PlayerSpawningEventArgs()
            {
                PlayerInfo = sPlayerSpawningData.ClientInfo.ToPlayerInfo(),
                ChunkViewDim = sPlayerSpawningData.ChunkViewDim,
                PlayerProfile = sPlayerSpawningData.PlayerProfile.ToModel(),
                Timestamp = DateTime.UtcNow,
            });
        }

        /// <summary>
        /// Runs each time a player file is saved from the client to the server.
        /// This will usually run about every 30 seconds per player as well as triggered updates such as dying.
        /// </summary>
        /// <param name="clientInfo">The client info.</param>
        /// <param name="pdf">The player data file.</param>
        internal void OnSavePlayerData(ref SSavePlayerDataData sSavePlayerDataData)
        {
            SavePlayerData?.Invoke(this, new SavePlayerDataEventArgs()
            {
                PlayerDetails = sSavePlayerDataData.ClientInfo.ToPlayerDetails(),
                Timestamp = DateTime.UtcNow,
            });
        }

        /// <summary>
        /// Runs when the game sky changes.
        /// </summary>
        /// <param name="skyChangedEventArgs">The sky changed event arguments.</param>
        internal void OnSkyChanged(SkyChangedEventArgs skyChangedEventArgs)
        {
            SkyChanged?.Invoke(this, skyChangedEventArgs);
        }
    }
}
