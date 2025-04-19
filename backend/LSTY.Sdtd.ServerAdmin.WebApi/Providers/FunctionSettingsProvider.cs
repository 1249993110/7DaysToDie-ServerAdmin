using LSTY.Sdtd.ServerAdmin.Services.Abstractions;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Providers
{
    /// <summary>
    /// Provides function settings for the game server.
    /// </summary>
    public class FunctionSettingsProvider : IFunctionSettingsProvider
    {
        /// <summary>
        /// Gets the function settings from the database.
        /// </summary>
        /// <param name="gameServerId"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<IReadOnlyDictionary<string, object>?> GetAsync(string gameServerId, string? functionName)
        {
            throw new NotImplementedException();
        }
    }
}
