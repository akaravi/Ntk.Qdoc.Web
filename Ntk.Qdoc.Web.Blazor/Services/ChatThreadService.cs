using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Ntk.Qdoc.Web.Blazor.Interfaces;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Services
{
    public class ChatThreadService : IChatThreadService
    {
        private readonly ConcurrentDictionary<string, ChatThreadModel> _chatThreads;

        public ChatThreadService()
        {
            _chatThreads = new ConcurrentDictionary<string, ChatThreadModel>();
        }

        public ChatThreadModel CreateChatThread(List<string> participants, string createdBy, string chatName = null)
        {
            var chatThread = new ChatThreadModel(participants, createdBy, chatName);
            _chatThreads.TryAdd(chatThread.ChatId, chatThread);
            return chatThread;
        }

        public ChatThreadModel GetChatThread(string chatId)
        {
            if (string.IsNullOrEmpty(chatId))
                return null;

            _chatThreads.TryGetValue(chatId, out var chatThread);
            return chatThread;
        }

        public IEnumerable<ChatThreadModel> GetChatThreadsForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return Enumerable.Empty<ChatThreadModel>();

            return _chatThreads.Values
                .Where(ct => ct.IsParticipant(username))
                .OrderByDescending(ct => ct.LastMessageTime)
                .ToList();
        }

        public IEnumerable<ChatThreadModel> GetActiveChatThreadsForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return Enumerable.Empty<ChatThreadModel>();

            // فقط threadهایی که پیامی دارند (LastMessageTime بعد از CreatedAt است یا LastMessagePreview خالی نیست)
            return _chatThreads.Values
                .Where(ct => ct.IsParticipant(username) && 
                            (!string.IsNullOrEmpty(ct.LastMessagePreview) || 
                             ct.LastMessageTime > ct.CreatedAt.AddSeconds(1))) // حداقل 1 ثانیه بعد از ایجاد
                .OrderByDescending(ct => ct.LastMessageTime)
                .ToList();
        }

        public void AddParticipant(string chatId, string username)
        {
            var chatThread = GetChatThread(chatId);
            if (chatThread != null)
            {
                chatThread.AddParticipant(username);
            }
        }

        public void RemoveParticipant(string chatId, string username)
        {
            var chatThread = GetChatThread(chatId);
            if (chatThread != null)
            {
                chatThread.RemoveParticipant(username);
                
                // اگر هیچ شرکت‌کننده‌ای باقی نماند، چت را حذف کنیم
                if (chatThread.Participants.Count == 0)
                {
                    _chatThreads.TryRemove(chatId, out _);
                }
            }
        }

        public void UpdateLastMessage(string chatId, DateTime when, string preview)
        {
            var chatThread = GetChatThread(chatId);
            if (chatThread != null)
            {
                chatThread.LastMessageTime = when;
                chatThread.LastMessagePreview = preview ?? string.Empty;
                if (chatThread.LastMessagePreview.Length > 50)
                {
                    chatThread.LastMessagePreview = chatThread.LastMessagePreview.Substring(0, 50) + "...";
                }
            }
        }

        public void IncrementUnreadCount(string chatId, string username)
        {
            var chatThread = GetChatThread(chatId);
            chatThread?.IncrementUnreadCount(username);
        }

        public void ResetUnreadCount(string chatId, string username)
        {
            var chatThread = GetChatThread(chatId);
            chatThread?.ResetUnreadCount(username);
        }

        public bool IsUserInChat(string chatId, string username)
        {
            var chatThread = GetChatThread(chatId);
            return chatThread != null && chatThread.IsParticipant(username);
        }
    }
}
