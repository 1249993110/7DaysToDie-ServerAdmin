using HarmonyLib;
using LSTY.Sdtd.ServerAdmin.Config;
using LSTY.Sdtd.ServerAdmin.HarmonyPatchers;
using LSTY.Sdtd.ServerAdmin.Hooks;
using LSTY.Sdtd.ServerAdmin.Triggers;
using LSTY.Sdtd.ServerAdmin.WebApi;
using MapRendering;
using Microsoft.Owin.Hosting;
using Platform.Local;
using System.Reflection;

namespace LSTY.Sdtd.ServerAdmin
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
        public static bool IsGameStartDone { get; private set; }

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

                // Load the configuration settings.
                AppConfig.Load(modInstance.Path);

                // Patch the mod using Harmony.
                PatchByHarmony();

                // Register mod event handlers.
                RegisterModEventHandlers();

                // Start the web application.
                StartWebApp();
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
                string[] files = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Mods"), "0Harmony.dll", SearchOption.AllDirectories);
                if (files.Length == 0)
                {
                    CustomLogger.Warn("It is detected that TFP Mod (0_TFP_Harmony) is not installed, some functions may not be available.");
                    return;
                }

                Harmony = new Harmony(ModInstance.Name);
                Harmony.PatchAll(Assembly.GetExecutingAssembly());

                CustomLogger.Info("Patch all by harmony success.");
            }
            catch (Exception ex)
            {
                throw new Exception("Patch by harmony failed.", ex);
            }
        }

        private static void StartWebApp()
        {
            try
            {
                string webUrl = AppConfig.Settings.WebUrl;
                var webApp = WebApp.Start<Startup>(webUrl);
                AppConfig.OnChange += (newSettings, oldSettings) =>
                {
                    if (newSettings.WebUrl != oldSettings.WebUrl)
                    {
                        try
                        {
                            webApp.Dispose();
                            webApp = WebApp.Start<Startup>(newSettings.WebUrl);
                            CustomLogger.Info($"Web application URL changed to: {newSettings.WebUrl}");
                        }
                        catch (Exception ex)
                        {
                            webApp = WebApp.Start<Startup>(oldSettings.WebUrl);
                            CustomLogger.Error(ex, $"Failed to change web application URL to: {newSettings.WebUrl}, reverted to: {oldSettings.WebUrl}");
                        }
                    }
                };
                CustomLogger.Info("Web application running on " + webUrl);
            }
            catch (Exception ex)
            {
                throw new Exception("Start web application failed.", ex);
            }
        }

        private static void RegisterModEventHandlers()
        {
            try
            {
                var modEventHub = ModEventHub.Instance;

                Log.LogCallbacks += modEventHub.OnLogCallback;
                ModEvents.GameAwake.RegisterHandler(modEventHub.OnGameAwake);
                ModEvents.GameStartDone.RegisterHandler(modEventHub.OnGameStartDone);
                ModEvents.GameShutdown.RegisterHandler(modEventHub.OnGameShutdown);
                ModEvents.PlayerLogin.RegisterHandler(modEventHub.OnPlayerLogin);
                ModEvents.PlayerSpawnedInWorld.RegisterHandler(modEventHub.OnPlayerSpawnedInWorld);
                ModEvents.EntityKilled.RegisterHandler(modEventHub.OnEntityKilled);
                ModEvents.PlayerDisconnected.RegisterHandler(modEventHub.OnPlayerDisconnected);
                ModEvents.SavePlayerData.RegisterHandler(modEventHub.OnSavePlayerData);
                ModEvents.ChatMessage.RegisterHandler(modEventHub.OnChatMessage);
                ModEvents.PlayerSpawning.RegisterHandler(modEventHub.OnPlayerSpawning);

                SkyChangeTrigger.Init(modEventHub.OnSkyChanged);
                WorldPatcher.Init(modEventHub.OnEntitySpawned);
                ModEvents.GameStartDone.RegisterHandler(GetMapTileCache);
                ModEvents.GameStartDone.RegisterHandler(WorldStaticDataHook.ReplaceXmls);
                ModEvents.GameStartDone.RegisterHandler((ref ModEvents.SGameStartDoneData _) => { IsGameStartDone = true; });

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
                    CustomLogger.Warn("It is detected that TFP Mod (TFP_MapRendering) is not installed, some functions may not be available.");
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
