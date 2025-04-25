using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.Services.Core;
using LSTY.Sdtd.ServerAdmin.WebApi.Authorization;
using LSTY.Sdtd.ServerAdmin.WebApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Entities;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Function Settings.
    /// </summary>
    [Authorize(AuthorizationPolicys.GameServerOwner)]
    [Route("api/[controller]")]
    public class FunctionSettingsController : ControllerBase
    {
        private readonly FunctionManager _functionManager;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionManager"></param>
        public FunctionSettingsController(FunctionManager functionManager)
        {
            _functionManager = functionManager;
        }

        /// <summary>
        /// Gets the function settings by the specified game server ID and function name.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<FunctionSettings>> Get([FromHeader] string gameServerId, [FromQuery] string? functionName)
        {
            var entity = await DB.Find<FunctionSettings>()
                .Match(p => p.GameServerId == gameServerId && p.FunctionName == functionName).ExecuteSingleAsync();

            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        /// <summary>
        /// Creates a new function setting.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<FunctionSettings>> Create([FromHeader] string gameServerId, [FromBody] FunctionSettingsCreateDto dto)
        {
            var entity = new FunctionSettings()
            {
                FunctionName = dto.FunctionName,
                GameServerId = gameServerId,
                Settings = dto.Settings
            };

            if (_functionManager.UpdateFunctionSettings(entity) == false)
            {
                return BadRequest($"Function with name '{entity.FunctionName}' does not exist.");
            }

            await entity.SaveAsync();

            return CreatedAtAction(nameof(Get), new { id = entity.ID }, entity);
        }

        /// <summary>
        /// Updates the function settings by the specified ID.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromHeader] string gameServerId, [FromRoute] string id, [FromBody] FunctionSettingsUpdateDto dto)
        {
            var entity = await DB.Find<FunctionSettings>().OneAsync(id);
            if (entity == null)
                return NotFound();

            if (entity.GameServerId != gameServerId)
            {
                return Forbid();
            }

            entity.Settings = dto.Settings;
            _functionManager.UpdateFunctionSettings(entity);

            await entity.SaveAsync();

            return Ok();
        }

        /// <summary>
        /// Deletes the function settings by the specified IDs.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromHeader] string gameServerId, [FromQuery] IEnumerable<string> ids, [FromQuery] bool deleteAll = false)
        {
            if (deleteAll)
            {
                await DB.DeleteAsync<FunctionSettings>(p => p.GameServerId == gameServerId);
            }
            else
            {
                foreach (var id in ids)
                {
                    var entity = await DB.Find<FunctionSettings>().OneAsync(id);
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
