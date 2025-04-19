using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.Services.Abstractions;
using MongoDB.Entities;

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
        public async Task<IReadOnlyDictionary<string, object>?> GetAsync(string gameServerId, string? functionName)
        {
            var functionSettings = await DB.Find<FunctionSettings>()
                .ManyAsync(p => p.GameServerId == gameServerId && p.FunctionName == functionName);

            return functionSettings.FirstOrDefault()?.SettingsDict;
        }
    }
}
