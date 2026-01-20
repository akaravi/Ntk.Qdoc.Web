using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ntk.Qdoc.Web.Blazor.Models;
using Ntk.Qdoc.Web.Blazor.Services;

namespace Ntk.Qdoc.Web.Blazor.Interfaces
{
    public interface IChatService
    {
        event EventHandler<UserLoginEventArgs> UserLoggedIn;
        event EventHandler<UserLogoutEventArgs> UserLoggedOut;
        event EventHandler<MessageModel> MessageReceived;

        UserModel Login(string username, ConnectedClientModel client);
        UserModel CheckUserExist(UserModel user, string userCode);
        IEnumerable<UserModel> GetAllUsers();
        void Logout(string username);
        
        // Existing message posting methods
        Task PostMessageAsync(UserModel user, string message);
        Task PostMessageAsync(UserModel user, string receiverUsername, string message);
        
        // New group chat methods
        ChatThreadModel CreateGroupChat(List<string> participants, string createdBy, string chatName = null);
        Task PostMessageToChatAsync(string chatId, UserModel user, string message);
        IEnumerable<MessageModel> GetMessagesForChat(string chatId, string username);
        IEnumerable<MessageModel> GetPrivateChatMessages(string user1, string user2);
        IEnumerable<ChatThreadModel> GetChatThreadsForUser(string username);
        IEnumerable<ChatThreadModel> GetActiveChatThreadsForUser(string username);
    }
}
