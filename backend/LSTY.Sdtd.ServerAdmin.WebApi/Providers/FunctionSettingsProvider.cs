using LSTY.Sdtd.ServerAdmin.Data.Entities;
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
        public async Task<string?> GetAsync(Guid gameServerId, string functionName)
        {
            var functionSettings = await Db.Query<FunctionConfig>()
                .WhereEq(p => p.GameServerId, gameServerId)
                .WhereEq(p => p.FunctionName, functionName)
                .GetSingleOrDefaultAsync();

            return functionSettings?.Settings;
        }
    }
}
