using LSTY.Sdtd.ServerAdmin.Services.Core;
using LSTY.Sdtd.ServerAdmin.Services.Settings;
using LSTY.Sdtd.ServerAdmin.Shared.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.EventArgs;
using LSTY.Sdtd.ServerAdmin.Shared.Models;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;

namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    public abstract class FunctionBase<TSettings> : IFunction where TSettings : class, ISettings
    {
        private readonly string _name;
        private bool _isEnabled;
        private TSettings _settings = null!;
        private SharedState _sharedState = null!;
        private ChatCommandHandler _chatCommandHandler = null!;

        protected SharedState SharedState => _sharedState;
        protected CommonSettings CommonSettings => _sharedState.CommonSettings;
        protected IModEventProxy ModEventProxy => _sharedState.ModEventProxy;
        protected IGameManageProxy GameManageProxy => _sharedState.GameManageProxy;
        protected TSettings Settings => _settings;

        public string Name => _name;
        ISettings IFunction.Settings => _settings;

        public FunctionBase()
        {
            _name = GetType().Name;
        }

        void IFunction.Init(SharedState sharedState, ChatCommandHandler chatCommandHandler)
        {
            _sharedState = sharedState;
            _chatCommandHandler = chatCommandHandler;
        }

        /// <summary>
        /// If the settings are changed, update the settings and call the protected OnSettingsChanged method
        /// </summary>
        /// <param name="settings">Settings</param>
        void IFunction.OnSettingsChanged(ISettings? settings)
        {
            if (settings is TSettings changedSettings)
            {
                lock (this)
                {
                    _settings = changedSettings;
                    if (_settings.IsEnabled)
                    {
                        // If the function is not running.
                        if (_isEnabled == false)
                        {
                            _isEnabled = true;
                            OnEnableFunction();
                        }
                    }
                    else
                    {
                        // If the function is not disabled
                        if (_isEnabled)
                        {
                            _isEnabled = false;
                            OnDisableFunction();
                        }
                    }
                    OnSettingsChanged();
                }
            }
        }

        Type IFunction.GetSettingsType()
        {
            return typeof(TSettings);
        }

        /// <summary>
        /// Called when the configuration changes, will be automatically called once during function initialization if the settings exist
        /// </summary>
        protected virtual void OnSettingsChanged()
        {
        }

        /// <summary>
        /// Called when capturing player chat messages, returns true if the message is handled by the current function
        /// </summary>
        /// <param name="command"></param>
        /// <param name="chatMessageEventArgs"></param>
        /// <returns>True if the message is handled by the current function</returns>
        protected virtual Task<bool> OnChatCommand(ChatCommand chatCommand)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Disabled function
        /// </summary>
        protected virtual void OnDisableFunction()
        {
            _chatCommandHandler.RemoveHook(OnChatCommand);
        }

        /// <summary>
        /// Enabled function
        /// </summary>
        protected virtual void OnEnableFunction()
        {
            _chatCommandHandler.AddHook(OnChatCommand);
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
                SenderName = CommonSettings.WhisperServerName
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
