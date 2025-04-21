using LSTY.Sdtd.ServerAdmin.Services.Settings;

namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    public abstract class SubFunctionBase<TSettings> : FunctionBase<TSettings>, ISubFunction where TSettings : class, ISettings
    {
        ISettings? ISubFunction.GetSettingsFromParent(ISettings parentSettings)
        {
            return GetSettingsFromParent(parentSettings);
        }

        protected abstract TSettings? GetSettingsFromParent(ISettings parentSettings);
    }

    public abstract class SubFunctionBase : SubFunctionBase<BasicSettings>
    {
    }
}
