using IceCoffee.Common.Extensions;
using LSTY.Sdtd.ServerAdmin.Services.Core;
using LSTY.Sdtd.ServerAdmin.Shared.Constants;
using LSTY.Sdtd.ServerAdmin.Shared.Proxies;
using LSTY.Sdtd.ServerAdmin.WebApi.Authorization;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Net.WebSockets;
using System.Text;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Controllers
{
    /// <summary>
    /// Enhanced Telnet Controller.
    /// </summary>
    [Authorize(AuthorizationPolicys.GameServerOwner)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class EnhancedTelnetController : ControllerBase
    {
        private readonly FunctionManager _functionManager;
        private readonly ILogger<EnhancedTelnetController> _logger;

        private readonly static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Converters = new JsonConverter[] { new StringEnumConverter() }
        };

        /// <summary>
        /// Constructor.
        /// </summary>
        public EnhancedTelnetController(FunctionManager functionManager, ILogger<EnhancedTelnetController> logger)
        {
            _functionManager = functionManager;
            _logger = logger;
        }

        /// <summary>
        /// WebSocket endpoint.
        /// </summary>
        /// <returns></returns>
        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocketAcceptContext = new WebSocketAcceptContext()
                {
                    DangerousEnableCompression = true,
                    SubProtocol = HttpContext.WebSockets.WebSocketRequestedProtocols[1]
                };

                Guid gameServerId = webSocketAcceptContext.SubProtocol.ToGuid();
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync(webSocketAcceptContext);

                if (_functionManager.TryGetFunctionGroup(gameServerId, out var functionGroup))
                {
                    string welcome = await functionGroup!.SharedState.GameManageProxy.GetWelcome();
                    string json = JsonConvert.SerializeObject(new
                    {
                        eventName = ModEventName.Welcome.ToString(),
                        eventArgs = welcome
                    }, _jsonSerializerSettings);
                    await SendAsync(webSocket, json);

                    void OnEventRaised(string eventName, EventArgs eventArgs)
                    {
                        if (webSocket.State == WebSocketState.Open)
                        {
                            string json = JsonConvert.SerializeObject(new { eventName, eventArgs }, _jsonSerializerSettings);
                            webSocket.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }

                    EventForwarder? eventForwarder = null;
                    eventForwarder = functionGroup.SharedState.EventForwarder;
                    eventForwarder.EventRaised += OnEventRaised;

                    try
                    {
                        await Telnet(webSocket, functionGroup.SharedState.GameManageProxy);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error in EnhancedTelnetController.Telnet");
                    }
                    finally
                    {
                        if (eventForwarder != null)
                        {
                            eventForwarder.EventRaised -= OnEventRaised;
                        }
                    }
                }
                else
                {
                    string message = $"Game server [{gameServerId}] not connected.";
                    await SendAsync(webSocket, message);
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private static async Task Telnet(WebSocket webSocket, IGameManageProxy gameManageProxy)
        {
            var buffer = new byte[1024];
            var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (receiveResult.CloseStatus.HasValue == false)
            {
                string command = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                var result = await gameManageProxy.ExecuteConsoleCommandAsync(command, true);
                if (result.Any())
                {
                    string json = JsonConvert.SerializeObject(new 
                    { 
                        eventName = ModEventName.CommandExecutionReply.ToString(), 
                        eventArgs = result 
                    }, _jsonSerializerSettings);
                    await SendAsync(webSocket, json);
                }

                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
        }

        private static Task SendAsync(WebSocket webSocket, string content)
        {
            return webSocket.SendAsync(Encoding.UTF8.GetBytes(content), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
