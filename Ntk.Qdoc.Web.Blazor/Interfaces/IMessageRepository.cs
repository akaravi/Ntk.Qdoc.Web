using System.Collections.Generic;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(MessageModel message);
        IEnumerable<MessageModel> GetMessages(string chatId, int limit = 100);
        IEnumerable<MessageModel> GetPrivateChatMessages(string user1, string user2, int limit = 100);
        IEnumerable<MessageModel> GetPublicMessages(int limit = 100);
        int GetUnreadCount(string chatId, string username);
        void MarkAsRead(string chatId, string username);
    }
}
