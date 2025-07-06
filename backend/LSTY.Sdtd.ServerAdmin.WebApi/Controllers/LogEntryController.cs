using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.WebApi.Authorization;
using LSTY.Sdtd.ServerAdmin.WebApi.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Log Entry.
    /// </summary>
    [Authorize(AuthorizationPolicys.GameServerOwner)]
    [Route("api/[controller]")]
    public class LogEntryController : ControllerBase
    {
        /// <summary>
        /// Gets the log entries by the game server ID and log entry query DTO.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedDto<LogEntry>>> Get([FromHeader] Guid gameServerId, [FromQuery] LogEntryQueryDto dto)
        {
            var query = Db.QueryPaged<LogEntry>()
                .WhereEq(p => p.GameServerId, gameServerId)
                .WhereGte(dto.StartDateTime.HasValue, p => p.CreatedAt, dto.StartDateTime)
                .WhereLte(dto.EndDateTime.HasValue, p => p.CreatedAt, dto.EndDateTime)
                .WhereEq(dto.LogLevel.HasValue, p => p.LogLevel, dto.LogLevel)
                .WhereLike(string.IsNullOrEmpty(dto.Keyword) == false, p => p.Message, dto.Keyword)
                .OrderBy(dto.Order, dto.Desc)
                .PageSize(dto.PageSize)
                .PageNumber(dto.PageNumber);

            var pagedResult = await query.GetPagedResultAsync();
            return Ok(pagedResult.Adapt<PagedDto<LogEntry>>());
        }

        /// <summary>
        /// Deletes the log entries by the specified game server ID and the specified IDs or deletes all messages if deleteAll is true.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromHeader] Guid gameServerId, [FromQuery] int[] ids, [FromQuery] bool deleteAll = false)
        {
            if (deleteAll)
            {
                await Db.Delete<LogEntry>().WhereEq(p => p.GameServerId, gameServerId).ExecuteAsync();
            }
            else
            {
                foreach (var id in ids)
                {
                    var entity = await Db.Query<LogEntry>(id).GetSingleOrDefaultAsync();
                    if (entity != null && entity.GameServerId == gameServerId)
                    {
                        await Db.Delete<LogEntry>(id).ExecuteAsync();
                    }
                }
            }

            return NoContent();
        }
    }
}
