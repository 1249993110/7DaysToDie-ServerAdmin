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
        public async Task<string?> GetAsync(string gameServerId, string functionName)
        {
            var functionSettings = await DB.Find<FunctionConfig>()
                .Match(p => p.GameServerId == gameServerId && p.FunctionName == functionName).ExecuteSingleAsync();

            return functionSettings?.Settings;
        }
    }
}
