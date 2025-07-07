using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.Services.Abstractions;
using LSTY.Sdtd.ServerAdmin.Services.Settings;
using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text.Json;

namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    public class FunctionManager
    {
        private readonly ConcurrentDictionary<Guid, FunctionGroup> _runningFunctions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFunctionSettingsProvider _functionSettingsProvider;
        private readonly ICustomLoggerFactory _customLoggerFactory;
        private readonly IOptions<Microsoft.AspNetCore.Mvc.JsonOptions> _jsonOptions;

        public FunctionManager(IServiceProvider serviceProvider, IFunctionSettingsProvider functionSettingsProvider, ILogger<FunctionManager> logger, ICustomLoggerFactory customLoggerFactory, IOptions<Microsoft.AspNetCore.Mvc.JsonOptions> jsonOptions)
        {
            _runningFunctions = new ConcurrentDictionary<Guid, FunctionGroup>();
            _serviceProvider = serviceProvider;
            _functionSettingsProvider = functionSettingsProvider;
            _customLoggerFactory = customLoggerFactory;
            _jsonOptions = jsonOptions;
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
            Guid serverId = sharedState.GameServerId;
            var logger = _customLoggerFactory.CreateLogger(Data.Enums.ServiceModule.FunctionManager, serverId);
            foreach (var type in types)
            {
                try
                {
                    var function = (IFunction)ActivatorUtilities.CreateInstance(_serviceProvider, type);
                    string functionName = function.Name;
                    string? serializedSettings = await _functionSettingsProvider.GetAsync(serverId, functionName);
                    var functionSettings = CreateSettings(function.GetSettingsType(), serializedSettings);

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

        private ISettings CreateSettings(Type settingsType, string? serializedSettings)
        {
            ISettings? settings;
            if (serializedSettings == null)
            {
                settings = Activator.CreateInstance(settingsType) as ISettings;
            }
            else
            {
                settings = JsonSerializer.Deserialize(serializedSettings, settingsType, _jsonOptions.Value.JsonSerializerOptions) as ISettings;
            }
            if (settings == null)
            {
                throw new InvalidOperationException($"Cannot create instance of {settingsType}");
            }

            return settings;
        }

        public bool UpdateFunctionConfig(FunctionConfig entity)
        {
            // Check if the function name is null, which means CommonSettings are changed
            if (entity.FunctionName == nameof(CommonSettings))
            {
                var commonSettings = CreateSettings(typeof(CommonSettings), entity.Settings);
                // Update all functions with the new CommonSettings
                if (_runningFunctions.TryGetValue(entity.GameServerId, out var functionGroup))
                {
                    functionGroup.SharedState.CommonSettings = (CommonSettings)commonSettings;
                    return true;
                }
            }
            else if (_runningFunctions.TryGetValue(entity.GameServerId, out var functionGroup))
            {
                if (functionGroup.Functions.TryGetValue(entity.FunctionName, out var function))
                {
                    var settingsType = function.GetSettingsType();
                    var _settings = CreateSettings(settingsType, entity.Settings);
                    function.OnSettingsChanged(_settings);
                    return true;
                }
            }

            return false;
        }

        public async Task RegisterFunctionsAsync(Guid serverId, IReadOnlyDictionary<Type, IProxy> rpcProxies)
        {
            // Load common settings from database
            var commonSettingsJson = await _functionSettingsProvider.GetAsync(serverId, nameof(CommonSettings));
            var commonSettings = (CommonSettings)CreateSettings(typeof(CommonSettings), commonSettingsJson);

            var modEventProxy = (IModEventProxy)rpcProxies[typeof(IModEventProxy)];
            var sharedState = new SharedState()
            {
                GameServerId = serverId,
                CommonSettings = commonSettings,
                RpcProxies = rpcProxies,
                ModEventProxy = modEventProxy,
                GameManageProxy = (IGameManageProxy)rpcProxies[typeof(IGameManageProxy)],
                ServiceProvider = _serviceProvider,
                EventForwarder = new EventForwarder(modEventProxy),
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

        public Task UnregisterFunctionsAsync(Guid serverId)
        {
            if (_runningFunctions.TryRemove(serverId, out var functionGroup))
            {
                functionGroup.Dispose();
            }

            return Task.CompletedTask;
        }

        public bool TryGetFunctionGroup(Guid serverId, out FunctionGroup? functionGroup)
        {
            return _runningFunctions.TryGetValue(serverId, out functionGroup);
        }
    }
}
