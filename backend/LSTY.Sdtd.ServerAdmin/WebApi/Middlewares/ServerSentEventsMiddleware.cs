using IceCoffee.Common;
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
                context.Response.Headers.Add("X-Accel-Buffering", new[] { "no" });

                var queue = Channel.CreateBounded<(string, string)>(new BoundedChannelOptions(1024)
                {
                    FullMode = BoundedChannelFullMode.Wait
                });

                string jsonPayload = JsonConvert.SerializeObject(new { message = CreateWelcomeMessage() }, _jsonSerializerSettings);
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

        private string CreateWelcomeMessage()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("*** Connected with 7DTD server.");
            stringBuilder.AppendLine("*** Server version: " + global::Constants.cVersionInformation.LongString + " Compatibility Version: " + global::Constants.cVersionInformation.LongStringNoBuild);
            stringBuilder.AppendLine("*** Dedicated server only build");
            stringBuilder.AppendLine(string.Empty);
            stringBuilder.AppendLine("Server IP:   " + (string.IsNullOrEmpty(GamePrefs.GetString(EnumGamePrefs.ServerIP)) ? "Any" : GamePrefs.GetString(EnumGamePrefs.ServerIP)));
            stringBuilder.AppendLine("Server port: " + GamePrefs.GetInt(EnumGamePrefs.ServerPort).ToString());
            stringBuilder.AppendLine("Max players: " + GamePrefs.GetInt(EnumGamePrefs.ServerMaxPlayerCount).ToString());
            stringBuilder.AppendLine("Game mode:   " + GamePrefs.GetString(EnumGamePrefs.GameMode));
            stringBuilder.AppendLine("World:       " + GamePrefs.GetString(EnumGamePrefs.GameWorld));
            stringBuilder.AppendLine("Game name:   " + GamePrefs.GetString(EnumGamePrefs.GameName));
            stringBuilder.AppendLine("Difficulty:  " + GamePrefs.GetInt(EnumGamePrefs.GameDifficulty).ToString());
            stringBuilder.AppendLine(string.Empty);
            stringBuilder.AppendLine("Press 'help' to get a list of all commands.");
            stringBuilder.AppendLine(string.Empty);

            return stringBuilder.ToString();
        }
    }
}
