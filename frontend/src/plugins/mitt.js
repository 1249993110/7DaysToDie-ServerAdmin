import mitt from 'mitt';

const emitter = mitt();

export const EVENT_TYPES = {
  GAME: {
    WELCOME: 'Game.Welcome',
    LOG_CALLBACK: 'Game.LogCallback',
    GAME_AWAKE: 'Game.GameAwake',
    GAME_START_DONE: 'Game.GameStartDone',
    GAME_UPDATE: 'Game.GameUpdate',
    GAME_SHUTDOWN: 'Game.GameShutdown',
    CALC_CHUNK_COLORS_DONE: 'Game.CalcChunkColorsDone',
    CHAT_MESSAGE: 'Game.ChatMessage',
    ENTITY_KILLED: 'Game.EntityKilled',
    ENTITY_SPAWNED: 'Game.EntitySpawned',
    PLAYER_DISCONNECTED: 'Game.PlayerDisconnected',
    PLAYER_LOGIN: 'Game.PlayerLogin',
    PLAYER_SPAWNED_IN_WORLD: 'Game.PlayerSpawnedInWorld',
    PLAYER_SPAWNING: 'Game.PlayerSpawning',
    SAVE_PLAYER_DATA: 'Game.SavePlayerData',
    SKY_CHANGED: 'Game.SkyChanged',
  },
  // UI: {
  //   THEME_CHANGE: 'UI:ThemeChange',
  // },
};

export default emitter;