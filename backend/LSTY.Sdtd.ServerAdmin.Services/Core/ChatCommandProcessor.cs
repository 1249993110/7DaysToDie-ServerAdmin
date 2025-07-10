using LSTY.Sdtd.ServerAdmin.Data.Abstractions;
using LSTY.Sdtd.ServerAdmin.Data.Entities;
using LSTY.Sdtd.ServerAdmin.Shared.EventArgs;
using LSTY.Sdtd.ServerAdmin.Shared.Models;

namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatCommandProcessor
    {
        private readonly ICustomLogger _logger;
        private readonly CommandRegistry _commandRegistry;
        private readonly SharedState _sharedState;

        public ChatCommandProcessor(ICustomLogger logger, CommandRegistry commandRegistry, SharedState sharedState)
        {
            _logger = logger;
            _commandRegistry = commandRegistry;
            _sharedState = sharedState;
        }

        internal async void Process(object? sender, ChatMessageEventArgs chatMessageEventArgs)
        {
            try
            {
                string? playerId = chatMessageEventArgs.PlayerId;
                if (playerId != null && chatMessageEventArgs.ChatType == ChatType.Global)
                {
                    try
                    {
                        var commonSettings = _sharedState.CommonSettings;
                        var commandParser = new CommandParser(commonSettings.ChatCommandPrefixes, commonSettings.AllowNoPrefix, commonSettings.ChatCommandSeparator);
                        var commandParseResult = commandParser.Parse(chatMessageEventArgs.Message);

                        if (commandParseResult.IsCommand)
                        {
                            if (_commandRegistry.TryGetCommand(commandParseResult.CommandName, out var commandInfo))
                            {
                                var commandSender = new CommandSender()
                                {
                                    EntityId = chatMessageEventArgs.EntityId,
                                    PlayerId = playerId,
                                    PlayerName = chatMessageEventArgs.SenderName,
                                };

                                await commandInfo.Execute(commandParseResult.Arguments, commandSender);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Guid correlationId = await _logger.LogErrorAsync(ex, "An error occurred while processing a chat command.");

                        await _sharedState.GameManageProxy.SendPrivateMessageAsync(new PrivateMessage()
                        {
                            Message = $"An error occurred while processing your command. Please contact the server administrator. Error ID: {correlationId}",
                            SenderName = _sharedState.CommonSettings.WhisperServerName,
                            TargetPlayerIdOrName = playerId,
                        });
                    }
                }
            }
            finally
            {
                var chatMessage = new ChatMessage()
                {
                    CreatedAt = chatMessageEventArgs.Timestamp,
                    EntityId = chatMessageEventArgs.EntityId,
                    PlayerId = chatMessageEventArgs.PlayerId,
                    SenderName = chatMessageEventArgs.SenderName,
                    ChatType = chatMessageEventArgs.ChatType,
                    Message = chatMessageEventArgs.Message,
                    GameServerId = _sharedState.GameServerId,
                };
                await Db.Insert(chatMessage).ExecuteAsync();
            }
        }
    }
}