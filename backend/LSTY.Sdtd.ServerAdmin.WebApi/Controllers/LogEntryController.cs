using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.Data.Extensions;
using LSTY.Sdtd.ServerAdmin.WebApi.Authorization;
using LSTY.Sdtd.ServerAdmin.WebApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;

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
        public async Task<ActionResult<PagedDto<LogEntry>>> Get([FromHeader] string gameServerId, [FromQuery] LogEntryQueryDto dto)
        {
            var query = DB.PagedSearch<LogEntry>();

            query.Match(p => p.GameServerId == gameServerId);

            if (dto.StartDateTime.HasValue)
                query.Match(p => p.CreatedOn >= dto.StartDateTime.Value);

            if (dto.EndDateTime.HasValue)
                query.Match(p => p.CreatedOn <= dto.EndDateTime.Value);

            if (dto.LogLevel.HasValue)
                query.Match(p => p.LogLevel == dto.LogLevel.Value);

            if (string.IsNullOrEmpty(dto.Keyword) == false)
            {
                var filterBulid = Builders<LogEntry>.Filter;
                var exp = new BsonRegularExpression(dto.Keyword, "i");
                query.Match(filterBulid.Regex(p => p.Content, exp) | filterBulid.Regex(p => p.AdditionalData, exp));
            }

            query.Sort(sortBulid => dto.Order.ToSortDefinition(sortBulid, dto.Desc));

            query.PageSize(dto.PageSize)
                 .PageNumber(dto.PageNumber);

            var result = await query.ExecuteAsync();

            return new PagedDto<LogEntry>()
            {
                Items = result.Results,
                Total = result.TotalCount,
            };
        }

        /// <summary>
        /// Deletes the log entries by the specified game server ID and the specified IDs or deletes all messages if deleteAll is true.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromHeader] string gameServerId, [FromQuery] IEnumerable<string> ids, [FromQuery] bool deleteAll = false)
        {
            if (deleteAll)
            {
                await DB.DeleteAsync<LogEntry>(p => p.GameServerId == gameServerId);
            }
            else
            {
                foreach (var id in ids)
                {
                    var entity = await DB.Find<LogEntry>().OneAsync(id);
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
