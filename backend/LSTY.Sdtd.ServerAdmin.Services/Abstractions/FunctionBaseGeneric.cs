namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    /// <summary>
    /// Generic function base class
    /// </summary>
    /// <typeparam name="TSettings"></typeparam>
    public abstract class FunctionBase<TSettings> : FunctionBase, IFunction where TSettings : class, ISettings
    {
        private bool _isEnabled;
        private TSettings _settings = null!;
        public override TSettings Settings => _settings;

        Type IFunction.GetSettingsType()
        {
            return typeof(TSettings);
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
                    var oldSettings = _settings;
                    _settings = changedSettings;
                    OnSettingsChanged(changedSettings, oldSettings);

                    if (_settings.IsEnabled)
                    {
                        // If the function is not running.
                        if (_isEnabled == false)
                        {
                            _isEnabled = true;
                            OnEnabled();
                        }
                    }
                    else
                    {
                        // If the function is not disabled
                        if (_isEnabled)
                        {
                            _isEnabled = false;
                            OnDisabled();
                        }
                    }

                    var subFunctions = ((IFunction)this).GetSubFunctions();
                    if (subFunctions != null)
                    {
                        foreach (var subFunction in subFunctions)
                        {
                            // Call the OnSettingsChanged method of the sub function
                            var subSettings = subFunction.GetSettingsFromParent(_settings);
                            if (subSettings == null)
                            {
                                continue;
                            }

                            subSettings.IsEnabled = _settings.IsEnabled && subSettings.IsEnabled;
                            subFunction.OnSettingsChanged(subSettings);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when the configuration changes
        /// </summary>
        protected virtual void OnSettingsChanged(TSettings newSettings, TSettings oldSettings)
        {
        }
    }
}
