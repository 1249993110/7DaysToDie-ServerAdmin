using LSTY.Sdtd.ServerAdmin.Services.Abstractions;
using LSTY.Sdtd.ServerAdmin.Shared.EventArgs;
using LSTY.Sdtd.ServerAdmin.Shared.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;

namespace LSTY.Sdtd.ServerAdmin.Services.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ChatCommandHandler : IDisposable
    {
        private ImmutableList<ChatHook> _chatHooks = ImmutableList<ChatHook>.Empty;
        private readonly ILogger<ChatCommandHandler> _logger;
        private readonly MemoryCache _cache;
        private readonly SharedState _sharedState;

        public ChatCommandHandler(ILogger<ChatCommandHandler> logger, SharedState sharedState)
        {
            _cache = new MemoryCache(new MemoryCacheOptions()
            {
                SizeLimit = 1024
            });
            _logger = logger;
            _sharedState = sharedState;
        }

        public void AddHook(ChatHook chatHook)
        {
            _chatHooks = _chatHooks.Add(chatHook);
        }

        public void RemoveHook(ChatHook chatHook)
        {
            _chatHooks = _chatHooks.Remove(chatHook);
        }

        /// <summary>
        /// 处理聊天消息。
        /// </summary>
        /// <param name="chatMessage">聊天消息。</param>
        internal async void Handle(object? sender, ChatMessageEventArgs chatMessageEventArgs)
        {
            try
            {
                string? playerId = chatMessageEventArgs.PlayerId;
                if (playerId != null && chatMessageEventArgs.ChatType == ChatType.Global)
                {
                    string command = chatMessageEventArgs.Message;
                    string chatPrefix = _sharedState.CommonSettings.ChatCommandPrefix;

                    if (string.IsNullOrEmpty(chatPrefix) == false)
                    {
                        if (command.StartsWith(chatPrefix) == false)
                        {
                            return;
                        }
                        else
                        {
                            command = command.Substring(chatPrefix.Length);
                        }
                    }

                    var chatCommand = new ChatCommand()
                    {
                        EntityId = chatMessageEventArgs.EntityId,
                        PlayerId = playerId,
                        PlayerName = chatMessageEventArgs.SenderName,
                        Command = command
                    };

                    if (_cache.TryGetValue(command, out ChatHook? chatHook))
                    {
                        if (chatHook != null && chatHook.Target is IFunction function && function.Settings.IsEnabled)
                        {
                            bool isHandled = await chatHook.Invoke(chatCommand);
                            if (isHandled)
                            {
                                return;
                            }
                            else
                            {
                                _cache.Remove(command);
                            }
                        }
                    }

                    foreach (var hook in _chatHooks)
                    {
                        if (hook == chatHook)
                        {
                            // Skip the current hook if it's already been processed.
                            continue;
                        }

                        bool isHandled = await hook.Invoke(chatCommand);
                        // If the command is handled, we can stop processing further hooks.
                        if (isHandled)
                        {
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                            {
                                Size = command.Length,
                            };
                            _cache.Set(command, hook, cacheEntryOptions);

                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChatCommandHandler.Handle");
            }
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }
}