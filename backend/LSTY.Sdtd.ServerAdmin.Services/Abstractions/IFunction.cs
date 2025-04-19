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

        internal void Init(SharedState sharedState, ChatCommandHandler chatCommandHandler);
        internal void OnSettingsChanged(ISettings settings);
        internal Type GetSettingsType();
    }
}