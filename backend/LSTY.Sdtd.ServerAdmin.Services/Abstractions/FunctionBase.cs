using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Enums;
using LSTY.Sdtd.ServerAdmin.Services.Core;
using LSTY.Sdtd.ServerAdmin.Services.Settings;
using LSTY.Sdtd.ServerAdmin.Shared.Models;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;
using Microsoft.Extensions.DependencyInjection;

namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    public abstract class FunctionBase : IFunction
    {
        private readonly string _name;
        private SharedState _sharedState = null!;
        private CommandRegistry _commandRegistry = null!;
        private List<ISubFunction>? _subFunctions;
        private ICustomLogger _logger = null!;

        protected SharedState SharedState => _sharedState;
        protected CommonSettings CommonSettings => _sharedState.CommonSettings;
        protected IModEventProxy ModEventProxy => _sharedState.ModEventProxy;
        protected IGameManageProxy GameManageProxy => _sharedState.GameManageProxy;
        protected CommandRegistry CommandRegistry => _commandRegistry;
        protected ICustomLogger Logger => _logger;

        public string Name => _name;
        public abstract ISettings Settings { get; }

        void IFunction.OnSettingsChanged(ISettings settings)
        {
            throw new NotImplementedException();
        }

        Type IFunction.GetSettingsType()
        {
            throw new NotImplementedException();
        }

        List<ISubFunction>? IFunction.GetSubFunctions()
        {
            return _subFunctions;
        }

        public FunctionBase()
        {
            _name = GetType().Name;
        }

        protected void AddSubFunction<TSubFunction>() where TSubFunction : class, ISubFunction
        {
            if (_subFunctions == null)
            {
                _subFunctions = new List<ISubFunction>();
            }

            var function = ActivatorUtilities.CreateInstance<TSubFunction>(SharedState.ServiceProvider);
            if (function is FunctionBase functionBase)
            {
                functionBase._logger = _logger;
                function.Init(_sharedState, _commandRegistry);
                _subFunctions.Add(function);
            }
        }

        void IFunction.Init(SharedState sharedState, CommandRegistry commandRegistry)
        {
            _sharedState = sharedState;
            _commandRegistry = commandRegistry;

            if (this is not ISubFunction)
            {
                var serviceModule = Enum.Parse<ServiceModule>(_name);
                _logger = sharedState.ServiceProvider.GetRequiredService<ICustomLoggerFactory>().CreateLogger(serviceModule, sharedState.GameServerId);
            }

            OnInit();
        }

        protected virtual void OnInit()
        {
            // This method can be overridden in derived classes to perform additional initialization tasks, such as add sub functions.
        }

        /// <summary>
        /// Enabled function
        /// </summary>
        protected virtual void OnEnabled()
        {
        }

        /// <summary>
        /// Disabled function
        /// </summary>
        protected virtual void OnDisabled()
        {
        }

        /// <summary>
        /// Send global message
        /// </summary>
        /// <param name="message">Message</param>
        protected Task SendGlobalMessage(string message)
        {
            return GameManageProxy.SendGlobalMessageAsync(new GlobalMessage()
            {
                Message = message,
                SenderName = CommonSettings.GlobalServerName
            });
        }

        /// <summary>
        /// Send private message
        /// </summary>
        /// <param name="playerIdOrName">Player ID or name</param>
        /// <param name="message">Message</param>
        protected Task SendPrivateMessage(string playerIdOrName, string message)
        {
            return GameManageProxy.SendPrivateMessageAsync(new PrivateMessage()
            {
                TargetPlayerIdOrName = playerIdOrName,
                Message = message,
                SenderName = CommonSettings.WhisperServerName
            });
        }
    }
}
