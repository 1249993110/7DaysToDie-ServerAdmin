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
    /// Chat Messages.
    /// </summary>
    [Authorize(AuthorizationPolicys.GameServerOwner)]
    [Route("api/[controller]")]
    public class ChatMessagesController : ControllerBase
    {
        /// <summary>
        /// Gets the chat messages by
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<PagedDto<ChatMessage>>> Get([FromHeader] string gameServerId, [FromQuery] ChatMessageQueryDto dto)
        {
            var query = DB.PagedSearch<ChatMessage>();

            query.Match(p => p.GameServerId == gameServerId);

            if (dto.StartDateTime.HasValue)
                query.Match(p => p.CreatedOn >= dto.StartDateTime.Value);

            if (dto.EndDateTime.HasValue)
                query.Match(p => p.CreatedOn <= dto.EndDateTime.Value);

            if (dto.ChatType.HasValue)
                query.Match(p => p.ChatType == dto.ChatType.Value);

            var filterBulid = Builders<ChatMessage>.Filter;
            if (string.IsNullOrEmpty(dto.SenderIdOrName) == false)
            {
                FilterDefinition<ChatMessage> filter;
                if (int.TryParse(dto.SenderIdOrName, out int entityId))
                {
                    filter = filterBulid.Or(
                        filterBulid.Eq(p => p.EntityId, entityId),
                        filterBulid.Eq(p => p.SenderName, dto.SenderIdOrName));
                }
                else
                {
                    filter = filterBulid.Or(
                        filterBulid.Eq(p => p.PlayerId, dto.SenderIdOrName),
                        filterBulid.Eq(p => p.SenderName, dto.SenderIdOrName));
                }
                query.Match(filter);
            }
     
            if (string.IsNullOrEmpty(dto.Keyword) == false)
            {
                query.Match(filterBulid.Regex(p => p.Message, new BsonRegularExpression(dto.Keyword, "i")));
            }

            query.Sort(sortBulid => dto.Order.ToSortDefinition(sortBulid, dto.Desc));

            query.PageSize(dto.PageSize)
                 .PageNumber(dto.PageNumber);

            var result = await query.ExecuteAsync();

            return new PagedDto<ChatMessage>()
            {
                Items = result.Results,
                Total = result.TotalCount,
            };
        }

        /// <summary>
        /// Deletes the chat message by the specified IDs.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete([FromHeader] string gameServerId, [FromQuery] IEnumerable<string> ids, [FromQuery] bool deleteAll = false)
        {
            if (deleteAll)
            {
                await DB.DeleteAsync<ChatMessage>(p => p.GameServerId == gameServerId);
            }
            else
            {
                foreach (var id in ids)
                {
                    var entity = await DB.Find<ChatMessage>().OneAsync(id);
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
