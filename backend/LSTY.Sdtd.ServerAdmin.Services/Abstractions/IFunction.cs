using LSTY.Sdtd.ServerAdmin.Services.Core;

namespace LSTY.Sdtd.ServerAdmin.Services.Abstractions
{
    public interface IFunction
    {
        /// <summary>
        /// The name of the function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The settings for the function.
        /// </summary>
        ISettings Settings { get; }

        internal void Init(SharedState sharedState, CommandRegistry commandRegistry);
        internal Type GetSettingsType();
        internal void OnSettingsChanged(ISettings settings);
        internal List<ISubFunction>? GetSubFunctions();
    }
}