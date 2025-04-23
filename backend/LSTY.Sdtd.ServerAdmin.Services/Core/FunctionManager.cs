using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Services.Abstractions;
using LSTY.Sdtd.ServerAdmin.Services.Settings;
using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    public class FunctionManager
    {
        private readonly ConcurrentDictionary<string, FunctionGroup> _runningFunctions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFunctionSettingsProvider _functionSettingsProvider;
        private readonly ICustomLoggerFactory _customLoggerFactory;

        public FunctionManager(IServiceProvider serviceProvider, IFunctionSettingsProvider functionSettingsProvider, ILogger<FunctionManager> logger, ICustomLoggerFactory customLoggerFactory)
        {
            _runningFunctions = new ConcurrentDictionary<string, FunctionGroup>();
            _serviceProvider = serviceProvider;
            _functionSettingsProvider = functionSettingsProvider;
            _customLoggerFactory = customLoggerFactory;
        }

        private static IEnumerable<Type> GetFunctionTypes()
        {
            return typeof(IFunction).Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && t.IsAbstract == false && typeof(IFunction).IsAssignableFrom(t) && typeof(ISubFunction).IsAssignableFrom(t) == false);
        }

        private async Task<Dictionary<string, IFunction>> CreateFunctionsAsync(SharedState sharedState, CommandRegistry commandRegistry)
        {
            var result = new Dictionary<string, IFunction>();
            var types = GetFunctionTypes();
            string serverId = sharedState.GameServerId;
            var logger = _customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.FunctionManager, serverId);
            foreach (var type in types)
            {
                try
                {
                    var function = (IFunction)ActivatorUtilities.CreateInstance(_serviceProvider, type);
                    string functionName = function.Name;
                    var settingsDict = await _functionSettingsProvider.GetAsync(serverId, functionName);
                    var functionSettings = AdaptSettings(function.GetSettingsType(), settingsDict);

                    function.Init(sharedState, commandRegistry);
                    function.OnSettingsChanged(functionSettings);

                    result.Add(functionName, function);
                }
                catch (Exception ex)
                {
                    await logger.LogErrorAsync(ex, $"Failed to create function {type} for server {serverId}");
                }
            }

            return result;
        }

        private static ISettings AdaptSettings(Type settingsType, IReadOnlyDictionary<string, object>? settingsDict)
        {
            var settings = Activator.CreateInstance(settingsType) as ISettings;
            if (settings == null)
            {
                throw new InvalidOperationException($"Cannot create instance of {settingsType}");
            }

            if(settingsDict == null)
            {
                return settings;
            }

            settingsDict.Adapt(settings);
            return settings;
        }

        public void UpdateFunctionSettings(string serverId, string? functionName, IReadOnlyDictionary<string, object> settingsDict)
        {
            // Check if the function name is null, which means CommonSettings are changed
            if (string.IsNullOrEmpty(functionName))
            {
                var commonSettings = AdaptSettings(typeof(CommonSettings), settingsDict);
                // Update all functions with the new CommonSettings
                if (_runningFunctions.TryGetValue(serverId, out var functionGroup))
                {
                    functionGroup.SharedState.CommonSettings = (CommonSettings)commonSettings;
                }
            }
            else if (_runningFunctions.TryGetValue(serverId, out var functionGroup))
            {
                if (functionGroup.Functions.TryGetValue(functionName, out var function))
                {
                    var settingsType = function.GetSettingsType();
                    var _settings = AdaptSettings(settingsType, settingsDict);
                    function.OnSettingsChanged(_settings);
                }
            }
        }

        public async Task RegisterFunctionsAsync(string serverId, IReadOnlyDictionary<Type, IProxy> rpcProxies)
        {
            // Load common settings from database
            var commonSettingsDict = await _functionSettingsProvider.GetAsync(serverId, null);
            var commonSettings = (CommonSettings)AdaptSettings(typeof(CommonSettings), commonSettingsDict);

            var sharedState = new SharedState()
            {
                GameServerId = serverId,
                CommonSettings = commonSettings,
                RpcProxies = rpcProxies,
                ModEventProxy = (IModEventProxy)rpcProxies[typeof(IModEventProxy)],
                GameManageProxy = (IGameManageProxy)rpcProxies[typeof(IGameManageProxy)],
                ServiceProvider = _serviceProvider,
            };

            var commandRegistry = new CommandRegistry();
            var chatCommandProcessorLogger = _customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.ChatCommandProcessor, serverId);
            var chatCommandProcessor = new ChatCommandProcessor(chatCommandProcessorLogger, commandRegistry, sharedState);
            sharedState.ModEventProxy.ChatMessage += chatCommandProcessor.Process;

            var functions = await CreateFunctionsAsync(sharedState, commandRegistry);
            var functionGroup = new FunctionGroup()
            {
                Functions = functions,
                SharedState = sharedState,
                ChatCommandProcessor = chatCommandProcessor,
            };

            _runningFunctions.AddOrUpdate(serverId, (gameServerId, newValue) =>
            {
                return newValue;
            }, (gameServerId, oldValue, newValue) =>
            {
                oldValue.Dispose();
                return newValue;
            }, functionGroup);
        }

        public void UnregisterFunctions(string serverId)
        {
            if (_runningFunctions.TryRemove(serverId, out var functionGroup))
            {
                functionGroup.Dispose();
            }
        }
    }
}
