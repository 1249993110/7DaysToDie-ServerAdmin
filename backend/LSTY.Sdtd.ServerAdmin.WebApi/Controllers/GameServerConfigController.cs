using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.RpcClient.Core;
using LSTY.Sdtd.ServerAdmin.RpcClient.Models;
using LSTY.Sdtd.ServerAdmin.WebApi.Dtos;
using LSTY.Sdtd.ServerAdmin.WebApi.Extensions;
using Mapster;
using MongoDB.Entities;
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
        public async Task<ActionResult<GameServerConfigDto>> Get([FromRoute] string id)
        {
            var entity = await DB.Find<GameServerConfig>().OneAsync(id);
            if (entity == null)
                return NotFound();

            var result = entity.Adapt<GameServerConfigDto>();

            if(_manager.TryGetClient(id, out var client))
            {
                result.IsConnected = client!.State == RpcClient.Clients.ConnectionState.Connected;
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Gets all game server configs belonging to the current user.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameServerConfigDto>>> Get()
        {
            var entities = await DB.Find<GameServerConfig>().ManyAsync(p => p.UserId == User.GetUserId());
            if (entities == null)
                return NotFound();

            var result = new List<GameServerConfigDto>(entities.Count);
            foreach (var item in entities)
            {
                var dto = item.Adapt<GameServerConfigDto>();

                if (_manager.TryGetClient(item.ID, out var client))
                {
                    dto.IsConnected = client!.State == RpcClient.Clients.ConnectionState.Connected;
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
        public async Task<ActionResult<GameServerConfigDto>> Create([FromForm] GameServerConfigCreateDto dto)
        {
            int fileLength = (int)dto.PfxFile.Length;
            if (fileLength > 1024 * 1024) // 1M
            {
                return BadRequest("Pfx file size must be less than 1MB.");
            }

            var entity = new GameServerConfig()
            {
                Name = dto.Name,
                Ip = dto.Ip,
                Port = dto.Port,
                PfxPassword = dto.PfxPassword,
                IsEnabled = dto.IsEnabled,
                Description = dto.Description,
                UserId = User.GetUserId()
            };

            using var memoryStream = new MemoryStream(fileLength);
            await dto.PfxFile.CopyToAsync(memoryStream);
            X509Certificate2? x509Certificate2 = null;
            try
            {
                x509Certificate2 = X509CertificateLoader.LoadPkcs12(memoryStream.ToArray(), entity.PfxPassword);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to load PFX file: {ex.Message}");
            }

            await entity.SaveAsync();
            await entity.Data.UploadAsync(memoryStream);

            if (entity.IsEnabled)
            {
                var rpcClientConfig = new RpcClientConfig()
                {
                    Id = entity.ID,
                    Url = $"tcp://{entity.Ip}:{entity.Port}",
                    Certificate = x509Certificate2,
                    Name = entity.Name,
                };

                await _manager.AddOrUpdateClientAsync(rpcClientConfig);
            }

            return CreatedAtAction(nameof(Get), new { id = entity.ID }, entity.Adapt<GameServerConfigDto>());
        }

        /// <summary>
        /// Updates the game server config by the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update([FromRoute] string id, [FromForm] GameServerConfigCreateDto dto)
        {
            var entity = await DB.Find<GameServerConfig>().OneAsync(id);
            if (entity == null)
                return NotFound();

            if (entity.UserId != User.GetUserId())
            {
                return Forbid();
            }

            entity.Name = dto.Name;
            entity.Ip = dto.Ip;
            entity.Port = dto.Port;
            entity.PfxPassword = dto.PfxPassword;
            entity.IsEnabled = dto.IsEnabled;
            entity.Description = dto.Description;

            using var memoryStream = new MemoryStream((int)dto.PfxFile.Length);
            await dto.PfxFile.CopyToAsync(memoryStream);
            X509Certificate2? x509Certificate2 = null;
            try
            {
                x509Certificate2 = X509CertificateLoader.LoadPkcs12(memoryStream.ToArray(), entity.PfxPassword);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to load PFX file: {ex.Message}");
            }

            await entity.SaveAsync();
            await entity.Data.UploadAsync(memoryStream);

            if (entity.IsEnabled)
            {
                var rpcClientConfig = new RpcClientConfig()
                {
                    Id = entity.ID,
                    Url = $"tcp://{entity.Ip}:{entity.Port}",
                    Certificate = x509Certificate2,
                    Name = entity.Name,
                };
                await _manager.AddOrUpdateClientAsync(rpcClientConfig);
            }
            else
            {
                await _manager.RemoveClientAsync(id);
            }

            return Ok();
        }

        /// <summary>
        /// Deletes the game server config by the specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id)
        {
            var entity = await DB.Find<GameServerConfig>().OneAsync(id);
            if (entity == null)
                return NotFound();

            if (entity.UserId != User.GetUserId())
            {
                return Forbid();
            }

            await _manager.RemoveClientAsync(id);

            await entity.Data.DeleteBinaryChunks();
            await entity.DeleteAsync();

            await DB.DeleteAsync<ChatMessage>(p => p.GameServerId == id);
            await DB.DeleteAsync<LogEntry>(p => p.GameServerId == id);
            await DB.DeleteAsync<FunctionConfig>(p => p.GameServerId == id);

            return NoContent();
        }
    }
}
