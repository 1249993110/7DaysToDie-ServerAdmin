using IceCoffee.Common;
using LSTY.Sdtd.ServerAdmin.Shared.Constants;
using Microsoft.Owin;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Threading.Channels;

namespace LSTY.Sdtd.ServerAdmin.WebApi.Middlewares
{
    internal class ServerSentEventsMiddleware : OwinMiddleware
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        public ServerSentEventsMiddleware(OwinMiddleware next, JsonSerializerSettings jsonSerializerSettings) : base(next)
        {
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (context.Request.Path == new PathString("/api/sse"))
            {
                if (context.Authentication.User == null || context.Authentication.User.Identity.IsAuthenticated == false)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized; // Unauthorized
                    return;
                }

                context.Response.ContentType = "text/event-stream";
                context.Response.Headers.Add("Cache-Control", new[] { "no-cache" });
                context.Response.Headers.Add("Connection", new[] { "keep-alive" });

                var queue = Channel.CreateBounded<(string, string)>(new BoundedChannelOptions(1024)
                {
                    FullMode = BoundedChannelFullMode.Wait
                });

                string jsonPayload = JsonConvert.SerializeObject(CreateWelcomeInfo(), _jsonSerializerSettings);
                await queue.Writer.WriteAsync((ModEventName.Welcome.ToString(), jsonPayload));

                var eventForwarder = new EventForwarder(ModEventHub.Instance);
                eventForwarder.EventRaised += async (eventName, eventArgs) =>
                {
                    try
                    {
                        string json = JsonConvert.SerializeObject(eventArgs, _jsonSerializerSettings);
                        await queue.Writer.WriteAsync((eventName, json), context.Request.CallCancelled);
                    }
                    catch
                    {
                    }
                };

                using (var writer = new StreamWriter(context.Response.Body, Encoding.UTF8, 1024, true))
                {
                    // Check if the client is disconnected
                    while (context.Request.CallCancelled.IsCancellationRequested == false)
                    {
                        try
                        {
                            (string eventName, string json) = await queue.Reader.ReadAsync(context.Request.CallCancelled);

                            // SSE data format: data: [your_data]\n\n
                            await writer.WriteLineAsync("event: " + eventName);
                            await writer.WriteLineAsync("data: " + json);
                            await writer.WriteLineAsync();
                            await writer.FlushAsync();
                        }
                        catch
                        {
                            break; // Exit the loop if the client is disconnected or an error occurs
                        }
                    }
                }
            }
            else
            {
                await Next.Invoke(context);
            }
        }

        private static WelcomeInfo CreateWelcomeInfo()
        {
            return new WelcomeInfo()
            {
                Version = new VersionInfo
                {
                    LongString = global::Constants.cVersionInformation.LongString,
                    CompatibilityVersion = global::Constants.cVersionInformation.LongStringNoBuild
                },
                ServerIP = string.IsNullOrEmpty(GamePrefs.GetString(EnumGamePrefs.ServerIP))
                     ? "Any"
                     : GamePrefs.GetString(EnumGamePrefs.ServerIP),
                ServerPort = GamePrefs.GetInt(EnumGamePrefs.ServerPort),
                ServerMaxPlayerCount = GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount),
                GameMode = GamePrefs.GetString(EnumGamePrefs.GameMode),
                GameWorld = GamePrefs.GetString(EnumGamePrefs.GameWorld),
                GameName = GamePrefs.GetString(EnumGamePrefs.GameName),
                GameDifficulty = GamePrefs.GetInt(EnumGamePrefs.GameDifficulty)
            };
        }

        private class VersionInfo
        {
            public required string LongString { get; set; }
            public required string CompatibilityVersion { get; set; }
        }

        private class WelcomeInfo
        {
            public required VersionInfo Version { get; set; }
            public required string ServerIP { get; set; }
            public required int ServerPort { get; set; }
            public required int ServerMaxPlayerCount { get; set; }
            public required string GameMode { get; set; }
            public required string GameWorld { get; set; }
            public required string GameName { get; set; }
            public required int GameDifficulty { get; set; }
        }
    }
}
