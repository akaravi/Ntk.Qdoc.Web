using System;
using System.Collections.Generic;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Interfaces
{
    public interface IChatThreadService
    {
        ChatThreadModel CreateChatThread(List<string> participants, string createdBy, string chatName = null);
        ChatThreadModel GetChatThread(string chatId);
        IEnumerable<ChatThreadModel> GetChatThreadsForUser(string username);
        IEnumerable<ChatThreadModel> GetActiveChatThreadsForUser(string username);
        void AddParticipant(string chatId, string username);
        void RemoveParticipant(string chatId, string username);
        void UpdateLastMessage(string chatId, DateTime when, string preview);
        void IncrementUnreadCount(string chatId, string username);
        void ResetUnreadCount(string chatId, string username);
        bool IsUserInChat(string chatId, string username);
    }
}
