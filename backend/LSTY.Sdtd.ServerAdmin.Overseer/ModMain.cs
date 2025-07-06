using HarmonyLib;
using LSTY.Sdtd.ServerAdmin.Overseer.Config;
using LSTY.Sdtd.ServerAdmin.Overseer.HarmonyPatchers;
using LSTY.Sdtd.ServerAdmin.Overseer.Hooks;
using LSTY.Sdtd.ServerAdmin.Overseer.RpcServer;
using LSTY.Sdtd.ServerAdmin.Overseer.Triggers;
using MapRendering;
using Platform.Local;
using System.Diagnostics;
using System.Reflection;

namespace LSTY.Sdtd.ServerAdmin.Overseer
{
    /// <summary>
    /// Main class for the mod.
    /// </summary>
    public class ModMain : IModApi
    {
        /// <summary>
        /// ModInstance
        /// </summary>
        public static Mod ModInstance { get; private set; } = null!;

        /// <summary>
        /// Main thread(the ui thread) synchronization context
        /// </summary>
        public static SynchronizationContext MainThreadSyncContext { get; private set; } = null!;

        /// <summary>
        /// Delegate used for executing commands.
        /// </summary>
        public static ClientInfo CmdExecuteDelegate { get; private set; } = null!;

        /// <summary>
        /// Gets the Harmony instance used for patching the mod.
        /// </summary>
        public static Harmony Harmony { get; private set; } = null!;

        /// <summary>
        /// Get the map tile cache.
        /// </summary>
        /// <returns>The map tile cache.</returns>
        public static MapTileCache? MapTileCache { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the game has started.
        /// </summary>
        public static bool IsGameStartDone => GameManager.Instance.IsStartingGame == false;

        /// <summary>
        /// Initializes the mod.
        /// </summary>
        /// <param name="modInstance"></param>
        [Conditional("DEBUG")]
        public void InitMod(Mod modInstance, bool debug)
        {
            try
            {
                ModInstance = modInstance;
                MainThreadSyncContext = SynchronizationContext.Current;

                // Load the configuration settings
                var settings = AppConfig.Load(modInstance.Path);

                // Initialize the RPC server with the specified port and certificate path and optional password
                string certPath = Path.IsPathRooted(settings.CertPath) ? settings.CertPath : Path.Combine(modInstance.Path, settings.CertPath);
                RpcServerManager.Init(settings.Port, certPath, settings.CertPassword);
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Initialize mod: " + modInstance.Name + " failed.");
            }
        }

        /// <summary>
        /// Initializes the mod.
        /// </summary>
        /// <param name="modInstance"></param>
        public void InitMod(Mod modInstance)
        {
            try
            {
                ModInstance = modInstance;
                MainThreadSyncContext = SynchronizationContext.Current;

                CmdExecuteDelegate = new ClientInfo()
                {
                    PlatformId = new UserIdentifierLocal(modInstance.Name),
                };

                // Load the configuration settings
                var settings = AppConfig.Load(modInstance.Path);

                // Patch the mod using Harmony
                PatchByHarmony();

                // Initialize the RPC server with the specified port and certificate path and optional password
                string certPath = Path.IsPathRooted(settings.CertPath) ? settings.CertPath : Path.Combine(modInstance.Path, settings.CertPath);
                RpcServerManager.Init(settings.Port, certPath, settings.CertPassword);

                // Register mod event handlers
                RegisterModEventHandlers();
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Initialize mod: " + modInstance.Name + " failed.");
            }
        }

        /// <summary>
        /// Patch the mod using Harmony.
        /// </summary>
        private static void PatchByHarmony()
        {
            try
            {
                Harmony = new Harmony(ModInstance.Name);
                Harmony.PatchAll(Assembly.GetExecutingAssembly());

                CustomLogger.Info("Patch all by harmony success.");
            }
            catch (Exception ex)
            {
                throw new Exception("Patch by harmony failed.", ex);
            }
        }

        private static void RegisterModEventHandlers()
        {
            try
            {
                var modEventProxy = RpcServerManager.ModEventProxy;

                Log.LogCallbacks += modEventProxy.OnLogCallback;
                ModEvents.GameAwake.RegisterHandler(modEventProxy.OnGameAwake);
                ModEvents.GameStartDone.RegisterHandler(modEventProxy.OnGameStartDone);
                ModEvents.GameShutdown.RegisterHandler(modEventProxy.OnGameShutdown);
                ModEvents.PlayerLogin.RegisterHandler(modEventProxy.OnPlayerLogin);
                ModEvents.PlayerSpawnedInWorld.RegisterHandler(modEventProxy.OnPlayerSpawnedInWorld);
                ModEvents.EntityKilled.RegisterHandler(modEventProxy.OnEntityKilled);
                ModEvents.PlayerDisconnected.RegisterHandler(modEventProxy.OnPlayerDisconnected);
                ModEvents.SavePlayerData.RegisterHandler(modEventProxy.OnSavePlayerData);
                ModEvents.ChatMessage.RegisterHandler(modEventProxy.OnChatMessage);
                ModEvents.PlayerSpawning.RegisterHandler(modEventProxy.OnPlayerSpawning);

                SkyChangeTrigger.Init(modEventProxy.OnSkyChanged);
                WorldPatcher.Init(modEventProxy.OnEntitySpawned);
                ModEvents.GameStartDone.RegisterHandler(GetMapTileCache);
                ModEvents.GameStartDone.RegisterHandler(WorldStaticDataHook.ReplaceXmls);

                CustomLogger.Info("Registered mod event handlers success.");
            }
            catch (Exception ex)
            {
                throw new Exception("Register mod event handlers failed.", ex);
            }
        }

        private static void GetMapTileCache(ref ModEvents.SGameStartDoneData sGameStartDoneData)
        {
            try
            {
                string[] files = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Mods"), "MapRendering.dll", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    CustomLogger.Warn("It is detected that TFP Mod is not installed, some functions may not be available.");
                }

                MapTileCache = (MapTileCache)MapRenderer.GetTileCache();
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Load map tile cache failed, Please do not delete the default mod, You can verify the integrity of the game to solve this problem.");
            }
        }
    }
}
