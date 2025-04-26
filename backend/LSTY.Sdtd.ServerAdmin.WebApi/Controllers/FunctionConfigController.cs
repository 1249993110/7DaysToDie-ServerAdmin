using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.Services.Core;
using LSTY.Sdtd.ServerAdmin.WebApi.Authorization;
using LSTY.Sdtd.ServerAdmin.WebApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Entities;

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
        public async Task<ActionResult<FunctionConfigDto>> Get(
            [FromHeader] string gameServerId, 
            [FromQuery] string? functionName)
        {
            var entity = await DB.Find<FunctionConfig>()
                .Match(p => p.GameServerId == gameServerId && p.FunctionName == functionName).ExecuteSingleAsync();

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
        public async Task<ActionResult<FunctionConfig>> Create(
            [FromHeader] string gameServerId, 
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

            await entity.SaveAsync();

            return CreatedAtAction(nameof(Get), new { id = entity.ID }, entity);
        }

        /// <summary>
        /// Updates the function config by the game server ID and the specified IDs.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            [FromHeader] string gameServerId, 
            [FromRoute] string id, 
            [FromBody] FunctionConfigUpdateDto dto)
        {
            var entity = await DB.Find<FunctionConfig>().OneAsync(id);
            if (entity == null)
                return NotFound();

            if (entity.GameServerId != gameServerId)
            {
                return Forbid();
            }
           
            entity.Settings = JsonSerializer.Serialize(dto.Settings, _jsonOptions.Value.JsonSerializerOptions);
            _functionManager.UpdateFunctionConfig(entity);

            await entity.SaveAsync();

            return Ok();
        }

        /// <summary>
        /// Deletes the function config by the game server ID and the specified IDs.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(
            [FromHeader] string gameServerId, 
            [FromQuery] IEnumerable<string> ids, 
            [FromQuery] bool deleteAll = false)
        {
            if (deleteAll)
            {
                await DB.DeleteAsync<FunctionConfig>(p => p.GameServerId == gameServerId);
            }
            else
            {
                foreach (var id in ids)
                {
                    var entity = await DB.Find<FunctionConfig>().OneAsync(id);
                    if (entity != null && entity.GameServerId != gameServerId)
                    {
                        await entity.DeleteAsync();
                    }
                }
            }

            return NoContent();
        }
    }
}
