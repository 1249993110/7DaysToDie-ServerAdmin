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
        /// Gets the function settings by the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<FunctionSettings>> Get(string id)
        {
            var entity = await DB.Find<FunctionSettings>().OneAsync(id);
            if (entity == null)
                return NotFound();

            return Ok(entity);
        }

        /// <summary>
        /// Gets the function settings by the specified game server ID and function name.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<FunctionSettings>> Get([FromHeader] string gameServerId, string? functionName)
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
        public async Task<ActionResult<FunctionSettings>> Create([FromHeader] string gameServerId, [FromBody] CreateFunctionSettingsDto dto)
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
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateFunctionSettingsDto dto)
        {
            var entity = await DB.Find<FunctionSettings>().OneAsync(id);
            if (entity == null)
                return NotFound();

            entity.Settings = dto.Settings;
            _functionManager.UpdateFunctionSettings(entity);

            await entity.SaveAsync();

            return Ok();
        }

        /// <summary>
        /// Deletes the function settings by the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var entity = await DB.Find<FunctionSettings>().OneAsync(id);
            if (entity == null)
                return NotFound();

            await entity.DeleteAsync();
            return NoContent();
        }

    }
}
