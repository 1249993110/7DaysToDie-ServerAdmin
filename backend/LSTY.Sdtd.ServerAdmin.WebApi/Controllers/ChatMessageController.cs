using IceCoffee.Db4Net.Core.SqlBuilders;
using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.WebApi.Authorization;
using LSTY.Sdtd.ServerAdmin.WebApi.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Chat Message.
    /// </summary>
    [Authorize(AuthorizationPolicys.GameServerOwner)]
    [Route("api/[controller]")]
    public class ChatMessageController : ControllerBase
    {
        /// <summary>
        /// Gets the chat messages by the specified game server ID and chat message query DTO.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedDto<ChatMessage>>> Get([FromHeader] Guid gameServerId, [FromQuery] ChatMessageQueryDto dto)
        {
            var query = Db.QueryPaged<ChatMessage>()
                .WhereEq(p => p.GameServerId, gameServerId)
                .WhereGte(dto.StartDateTime.HasValue, p => p.CreatedAt, dto.StartDateTime)
                .WhereLte(dto.EndDateTime.HasValue, p => p.CreatedAt, dto.EndDateTime)
                .WhereLike(string.IsNullOrEmpty(dto.Keyword) == false, p => p.Message, dto.Keyword)
                .OrderBy(dto.Order, dto.Desc)
                .PageSize(dto.PageSize)
                .PageNumber(dto.PageNumber);

            var filter = SqlBuilder<ChatMessage>.Filter;
            if (string.IsNullOrEmpty(dto.SenderIdOrName) == false)
            {
                if (int.TryParse(dto.SenderIdOrName, out int entityId))
                {
                    query.Where(filter.Eq(p => p.EntityId, entityId) | filter.Eq(p => p.SenderName, dto.SenderIdOrName));
                }
                else
                {
                    query.Where(filter.Eq(p => p.PlayerId, dto.SenderIdOrName) | filter.Eq(p => p.SenderName, dto.SenderIdOrName));
                }
            }

            var pagedResult = await query.GetPagedResultAsync();
            return Ok(pagedResult.Adapt<PagedDto<ChatMessage>>());
        }

        /// <summary>
        /// Deletes the chat messages by the specified game server ID and the specified IDs or deletes all messages if deleteAll is true.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete([FromHeader] Guid gameServerId, [FromQuery] int[] ids, [FromQuery] bool deleteAll = false)
        {
            if (deleteAll)
            {
                await Db.Delete<ChatMessage>().WhereEq(p => p.GameServerId, gameServerId).ExecuteAsync();
            }
            else
            {
                foreach (var id in ids)
                {
                    var entity = await Db.Query<ChatMessage>(id).GetSingleOrDefaultAsync();
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
