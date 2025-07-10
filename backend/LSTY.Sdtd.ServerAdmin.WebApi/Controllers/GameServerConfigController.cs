using IceCoffee.Db4Net.Extensions;
using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.RpcClient.Core;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using LSTY.Sdtd.ServerAdmin.WebApi.Dtos;
using LSTY.Sdtd.ServerAdmin.WebApi.Extensions;
using System.Security.Cryptography.X509Certificates;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Game Server Config.
    /// </summary>
    [Route("api/[controller]")]
    public class GameServerConfigController : ControllerBase
    {
        private readonly RpcClientManager _manager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameServerConfigController"/> class.
        /// </summary>
        /// <param name="manager"></param>
        public GameServerConfigController(RpcClientManager manager)
        {
            _manager = manager;
        }

        /// <summary>
        /// Gets the game server config by the specified ID.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GameServerConfigDto>> Get([FromRoute] Guid id)
        {
            var entity = await Db.Query<GameServerConfig>(id).GetSingleOrDefaultAsync();
            if (entity == null)
                return NotFound();

            var result = entity.Adapt<GameServerConfigDto>();

            if(_manager.TryGetClient(id, out var client))
            {
                result.IsConnected = client.IsConnected;
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Gets all game server configs belonging to the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<GameServerConfigDto>>> Get()
        {
            var entities = await Db.Query<GameServerConfig>().WhereEq(p => p.UserId, User.GetUserId()).GetListAsync();
            if (entities.Any() == false)
                return NotFound();

            var result = new List<GameServerConfigDto>();
            foreach (var item in entities)
            {
                var dto = item.Adapt<GameServerConfigDto>();

                if (_manager.TryGetClient(item.Id, out var client))
                {
                    dto.IsConnected = client.IsConnected;
                }

                result.Add(dto);
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new game server config.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GameServerConfigDto>> Create([FromForm] GameServerConfigCreateDto dto)
        {
            int fileLength = (int)dto.PfxFile.Length;
            if (fileLength > 1024 * 1024) // 1M
            {
                return BadRequest("Pfx file size must be less than 1MB.");
            }

            using var memoryStream = new MemoryStream(fileLength);
            await dto.PfxFile.CopyToAsync(memoryStream);

            var entity = new GameServerConfig()
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Host = dto.Host,
                Port = dto.Port,
                PfxFile = memoryStream.ToArray(),
                PfxPassword = dto.PfxPassword,
                IsEnabled = dto.IsEnabled,
                Description = dto.Description,
                UserId = User.GetUserId()
            };

            await Db.Insert(entity).ExecuteAsync();

            if (entity.IsEnabled)
            {
                X509Certificate2? x509Certificate2 = null;
                try
                {
                    x509Certificate2 = X509CertificateLoader.LoadPkcs12(entity.PfxFile, dto.PfxPassword);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to load PFX file: {ex.Message}");
                }

                var rpcClientConfig = new RpcClientConfig()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Host = entity.Host,
                    Port = entity.Port,
                    PfxFile = entity.PfxFile,
                    PfxPassword = entity.PfxPassword,
                };

                await _manager.AddOrUpdateClientAsync(rpcClientConfig);
            }

            return CreatedAtAction(nameof(Get), new { entity.Id }, entity.Adapt<GameServerConfigDto>());
        }

        /// <summary>
        /// Updates the game server config by the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Update([FromRoute] Guid id, [FromForm] GameServerConfigCreateDto dto)
        {
            var entity = await Db.Query<GameServerConfig>(id).GetSingleOrDefaultAsync();
            if (entity == null)
                return NotFound();

            if (entity.UserId != User.GetUserId())
            {
                return Forbid();
            }

            using var memoryStream = new MemoryStream((int)dto.PfxFile.Length);
            await dto.PfxFile.CopyToAsync(memoryStream);

            entity.Name = dto.Name;
            entity.Host = dto.Host;
            entity.Port = dto.Port;
            entity.PfxFile = memoryStream.ToArray();
            entity.PfxPassword = dto.PfxPassword;
            entity.IsEnabled = dto.IsEnabled;
            entity.Description = dto.Description;

            
            await Db.Update(entity).ExecuteAsync();

            if (entity.IsEnabled)
            {
                X509Certificate2? x509Certificate2 = null;
                try
                {
                    x509Certificate2 = X509CertificateLoader.LoadPkcs12(entity.PfxFile, entity.PfxPassword);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Failed to load PFX file: {ex.Message}");
                }

                var rpcClientConfig = new RpcClientConfig()
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Host = entity.Host,
                    Port = entity.Port,
                    PfxFile = entity.PfxFile,
                    PfxPassword = entity.PfxPassword,
                };
                await _manager.AddOrUpdateClientAsync(rpcClientConfig);
            }
            else
            {
                await _manager.RemoveClientAsync(id);
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes the game server config by the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> Delete([FromRoute] Guid id)
        {
            var entity = await Db.Query<GameServerConfig>(id).GetSingleOrDefaultAsync();
            if (entity == null)
                return NotFound();

            if (entity.UserId != User.GetUserId())
            {
                return Forbid();
            }

            await _manager.RemoveClientAsync(id);

            using var unitOfWork = Db.CreateUnitOfWork();

            await Db.Delete<GameServerConfig>(id).ExecuteAsync(unitOfWork.DbTransaction);
            await Db.Delete<ChatMessage>().WhereEq(p => p.GameServerId, id).ExecuteAsync(unitOfWork.DbTransaction);
            await Db.Delete<LogEntry>().WhereEq(p => p.GameServerId, id).ExecuteAsync(unitOfWork.DbTransaction);
            await Db.Delete<FunctionConfig>().WhereEq(p => p.GameServerId, id).ExecuteAsync(unitOfWork.DbTransaction);

            await unitOfWork.CommitAsync();

            return NoContent();
        }
    }
}
