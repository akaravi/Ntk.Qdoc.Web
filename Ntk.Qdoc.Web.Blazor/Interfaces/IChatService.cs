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
        IEnumerable<UserModel> GetAllUsers();
        void Logout(string username);
        Task PostMessageAsync(UserModel user, string message);
    }
}
