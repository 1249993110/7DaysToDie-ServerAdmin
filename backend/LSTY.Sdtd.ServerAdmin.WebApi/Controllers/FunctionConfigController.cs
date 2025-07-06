using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.Services.Core;
using LSTY.Sdtd.ServerAdmin.WebApi.Authorization;
using LSTY.Sdtd.ServerAdmin.WebApi.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Function Config.
    /// </summary>
    [Authorize(AuthorizationPolicys.GameServerOwner)]
    [Route("api/[controller]")]
    public class FunctionConfigController : ControllerBase
    {
        private readonly FunctionManager _functionManager;
        private readonly IOptions<JsonOptions> _jsonOptions;

        /// <summary>
        /// 
        /// </summary>
        public FunctionConfigController(FunctionManager functionManager, IOptions<JsonOptions> jsonOptions)
        {
            _functionManager = functionManager;
            _jsonOptions = jsonOptions;
        }

        /// <summary>
        /// Gets the function config by the specified game server ID and function name.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FunctionConfigDto>> Get(
            [FromHeader] Guid gameServerId, 
            [FromQuery] string? functionName)
        {
            var entity = await Db.Query<FunctionConfig>()
                .WhereEq(p => p.GameServerId, gameServerId)
                .WhereEq(p => p.FunctionName, functionName)
                .GetSingleOrDefaultAsync();

            if (entity == null)
                return NotFound();

            return Ok(new FunctionConfigDto()
            {
                GameServerId = entity.GameServerId,
                FunctionName = entity.FunctionName,
                Settings = JsonSerializer.Deserialize<Dictionary<string, object?>>(entity.Settings, _jsonOptions.Value.JsonSerializerOptions)
            });
        }

        /// <summary>
        /// Creates a new function config.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FunctionConfig>> Create(
            [FromHeader] Guid gameServerId, 
            [FromBody] FunctionConfigCreateDto dto)
        {
            var settings = JsonSerializer.Serialize(dto.Settings, _jsonOptions.Value.JsonSerializerOptions);
            var entity = new FunctionConfig()
            {
                FunctionName = dto.FunctionName,
                GameServerId = gameServerId,
                Settings = settings
            };

            if (_functionManager.UpdateFunctionConfig(entity) == false)
            {
                return BadRequest($"Function with name '{entity.FunctionName}' does not exist.");
            }

            int id = await Db.InsertAndGetId(entity).ExecuteAsync<int>();

            return CreatedAtAction(nameof(Get), new { id }, entity);
        }

        /// <summary>
        /// Updates the function config by the game server ID and the specified IDs.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(
            [FromHeader] Guid gameServerId, 
            [FromRoute] int id, 
            [FromBody] FunctionConfigUpdateDto dto)
        {
            var entity = await Db.Query<FunctionConfig>(id).GetSingleOrDefaultAsync();
            if (entity == null)
                return NotFound();

            if (entity.GameServerId != gameServerId)
            {
                return Forbid();
            }
           
            entity.Settings = JsonSerializer.Serialize(dto.Settings, _jsonOptions.Value.JsonSerializerOptions);
            _functionManager.UpdateFunctionConfig(entity);

            await Db.Update(entity).ExecuteAsync();

            return NoContent();
        }

        /// <summary>
        /// Deletes the function config by the game server ID and the specified IDs.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(
            [FromHeader] Guid gameServerId, 
            [FromQuery] int[] ids, 
            [FromQuery] bool deleteAll = false)
        {
            if (deleteAll)
            {
                await Db.Delete<FunctionConfig>().WhereEq(p => p.GameServerId, gameServerId).ExecuteAsync();
            }
            else
            {
                foreach (var id in ids)
                {
                    var entity = await Db.Query<FunctionConfig>(id).GetSingleOrDefaultAsync();
                    if (entity != null && entity.GameServerId == gameServerId)
                    {
                        await Db.Delete(id).ExecuteAsync();
                    }
                }
            }

            return NoContent();
        }
    }
}
