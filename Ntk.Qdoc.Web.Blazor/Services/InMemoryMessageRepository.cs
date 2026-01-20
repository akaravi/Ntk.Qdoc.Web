using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Ntk.Qdoc.Web.Blazor.Interfaces;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Services
{
    public class InMemoryMessageRepository : IMessageRepository
    {
        private readonly ConcurrentDictionary<string, List<MessageModel>> _chatMessages;
        private readonly ConcurrentDictionary<string, DateTime> _lastReadTime; // chatId_username -> last read time
        private const int MaxMessagesPerChat = 1000;

        public InMemoryMessageRepository()
        {
            _chatMessages = new ConcurrentDictionary<string, List<MessageModel>>();
            _lastReadTime = new ConcurrentDictionary<string, DateTime>();
        }

        public void AddMessage(MessageModel message)
        {
            if (message == null)
                return;

            string key = GetMessageKey(message);

            _chatMessages.AddOrUpdate(key,
                new List<MessageModel> { message },
                (k, list) =>
                {
                    lock (list)
                    {
                        list.Add(message);
                        // محدود کردن به MaxMessagesPerChat پیام
                        if (list.Count > MaxMessagesPerChat)
                        {
                            list.RemoveAt(0);
                        }
                    }
                    return list;
                });
        }

        private string GetMessageKey(MessageModel message)
        {
            if (!string.IsNullOrEmpty(message.ChatId))
            {
                return $"chat_{message.ChatId}";
            }

            if (!string.IsNullOrEmpty(message.ReceiverUsername))
            {
                // برای چت خصوصی، key باید همیشه یکسان باشد (user1_user2 یا user2_user1)
                var users = new[] { message.Username, message.ReceiverUsername }.OrderBy(u => u).ToArray();
                return $"private_{users[0]}_{users[1]}";
            }

            return "public";
        }

        public IEnumerable<MessageModel> GetMessages(string chatId, int limit = 100)
        {
            if (string.IsNullOrEmpty(chatId))
                return Enumerable.Empty<MessageModel>();

            string key = $"chat_{chatId}";
            if (_chatMessages.TryGetValue(key, out var messages))
            {
                lock (messages)
                {
                    return messages.TakeLast(limit).ToList();
                }
            }
            return Enumerable.Empty<MessageModel>();
        }

        public IEnumerable<MessageModel> GetPrivateChatMessages(string user1, string user2, int limit = 100)
        {
            if (string.IsNullOrEmpty(user1) || string.IsNullOrEmpty(user2))
                return Enumerable.Empty<MessageModel>();

            var users = new[] { user1, user2 }.OrderBy(u => u).ToArray();
            string key = $"private_{users[0]}_{users[1]}";

            if (_chatMessages.TryGetValue(key, out var messages))
            {
                lock (messages)
                {
                    return messages.TakeLast(limit).ToList();
                }
            }
            return Enumerable.Empty<MessageModel>();
        }

        public IEnumerable<MessageModel> GetPublicMessages(int limit = 100)
        {
            if (_chatMessages.TryGetValue("public", out var messages))
            {
                lock (messages)
                {
                    return messages.TakeLast(limit).ToList();
                }
            }
            return Enumerable.Empty<MessageModel>();
        }

        public int GetUnreadCount(string chatId, string username)
        {
            if (string.IsNullOrEmpty(chatId) || string.IsNullOrEmpty(username))
                return 0;

            string key = $"chat_{chatId}";
            if (!_chatMessages.TryGetValue(key, out var messages))
                return 0;

            string readKey = $"{key}_{username}";
            DateTime lastRead = _lastReadTime.TryGetValue(readKey, out var readTime) ? readTime : DateTime.MinValue;

            lock (messages)
            {
                return messages.Count(m => m.When > lastRead);
            }
        }

        public void MarkAsRead(string chatId, string username)
        {
            if (string.IsNullOrEmpty(chatId) || string.IsNullOrEmpty(username))
                return;

            string readKey = $"chat_{chatId}_{username}";
            _lastReadTime.AddOrUpdate(readKey, DateTime.UtcNow, (k, v) => DateTime.UtcNow);
        }
    }
}
