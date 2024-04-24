using SdtdServerKit.Data.Entities;
using SdtdServerKit.Data.IRepositories;
using SdtdServerKit.Functions;
using SdtdServerKit.Managers;
using System.Collections.Immutable;

namespace SdtdServerKit.Hooks
{
    internal delegate Task<bool> ChatHook(string cmd, OnlinePlayer onlinePlayer);
    internal static class ChatMessageHook
    {
        private static ImmutableList<ChatHook> _chatHooks = ImmutableList<ChatHook>.Empty;
        private static readonly CmdCache _chatCommandCache;

        static ChatMessageHook()
        {
            _chatCommandCache = new CmdCache();
            ModEventHook.ChatMessage += OnChatMessage;
        }

        public static void AddHook(ChatHook chatHook)
        {
            _chatHooks = _chatHooks.Add(chatHook);
        }

        public static void RemoveHook(ChatHook chatHook)
        {
            _chatHooks = _chatHooks.Remove(chatHook);
        }

        private static async Task<bool> HandleChatCmd(ChatHook chatHook, string cmd, OnlinePlayer onlinePlayer)
        {
            try
            {
                return await chatHook.Invoke(cmd, onlinePlayer);
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in ChatMessageHook.HandleChatMessage");

                Utils.SendPrivateMessage(new PrivateMessage()
                {
                    Message = ConfigManager.GlobalSettings.HandleChatMessageError,
                    SenderName = ConfigManager.GlobalSettings.ServerName,
                    TargetPlayerIdOrName = onlinePlayer.CrossplatformId,
                });

                return false;
            }
        }

        private static async Task Persistent(ChatMessage chatMessage)
        {
            try
            {
                var chatRecordRepository = ModApi.ServiceContainer.Resolve<IChatRecordRepository>();
                await chatRecordRepository.InsertAsync(new T_ChatRecord()
                {
                    CreatedAt = DateTime.Now,
                    ChatType = chatMessage.ChatType,
                    PlayerId = chatMessage.PlayerId,
                    Message = chatMessage.Message,
                    SenderName = chatMessage.SenderName,
                });
            }
            catch (Exception ex)
            {
                CustomLogger.Error(ex, "Error in PersistentManager.OnChatMessage");
            }
        }

        private static void OnChatMessage(ChatMessage chatMessage)
        {
            Task.Run(async () =>
            {
                try
                {
                    string cmd = chatMessage.Message;

                    if (chatMessage.ChatType != ChatType.Global || chatMessage.EntityId <= 0)
                    {
                        return;
                    }

                    string chatPrefix = ConfigManager.GlobalSettings.ChatCommandPrefix;

                    if (string.IsNullOrEmpty(chatPrefix) == false)
                    {
                        if (cmd.StartsWith(chatPrefix) == false)
                        {
                            return;
                        }
                        else
                        {
                            cmd = cmd.Substring(chatPrefix.Length);
                        }
                    }

                    var player = ConnectionManager.Instance.Clients.ForEntityId(chatMessage.EntityId).ToOnlinePlayer();

                    var chatHook = _chatCommandCache.Get(cmd);
                    if (chatHook != null && chatHook.Target is IFunction function && function.IsEnabled)
                    {
                        bool isHandled = await HandleChatCmd(chatHook, cmd, player);

                        if (isHandled)
                        {
                            return;
                        }
                        else
                        {
                            _chatCommandCache.Remove(cmd);
                        }
                    }

                    foreach (var hook in _chatHooks)
                    {
                        if (hook == chatHook)
                        {
                            continue;
                        }

                        bool isHandled = await HandleChatCmd(hook, cmd, player);
                        // 如果命令被其他功能处理
                        if (isHandled && cmd.Length <= 8)
                        {
                            _chatCommandCache.Set(cmd, hook);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    CustomLogger.Error(ex, "Error in ChatMessageHook.OnChatMessage");

                    Utils.SendPrivateMessage(new PrivateMessage()
                    {
                        Message = ConfigManager.GlobalSettings.HandleChatMessageError,
                        SenderName = ConfigManager.GlobalSettings.ServerName,
                        TargetPlayerIdOrName = chatMessage.PlayerId,
                    });
                }

                await Persistent(chatMessage);
            });
        }

        class CmdCache
        {
            private const int CacheSize = 64;
            private readonly (string? Message, ChatHook? ChatHook)[] _cache;
            public CmdCache()
            {
                _cache = new (string? Message, ChatHook? ChatHook)[CacheSize];
            }

            public ChatHook? Get(string message)
            {
                for (int i = 0; i < CacheSize; i++)
                {
                    if (_cache[i].Message == message)
                    {
                        return _cache[i].ChatHook;
                    }
                }

                return null;
            }

            public void Remove(string message)
            {
                for (int i = 0; i < CacheSize; i++)
                {
                    if (_cache[i].Message == message)
                    {
                        _cache[i] = default;
                        return;
                    }
                }
            }

            public void Set(string message, ChatHook chatHook)
            {
                for (int i = 0; i < CacheSize; i++)
                {
                    if (_cache[i].Message == null)
                    {
                        _cache[i] = (message, chatHook);
                        return;
                    }
                }
            }
        }
    }
}