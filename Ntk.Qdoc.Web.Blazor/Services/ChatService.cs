using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ntk.Qdoc.Web.Blazor.Interfaces;
using Ntk.Qdoc.Web.Blazor.Models;
using Ntk.Qdoc.Web.Blazor.Providers;

namespace Ntk.Qdoc.Web.Blazor.Services
{

    public class ChatService : IChatService
    {
        private readonly IUserStateProvider _usersProvider;
        private readonly IMessagesPublisher _publisher;
        private readonly IMessagesConsumer _consumer;

        public ChatService(IUserStateProvider usersProvider, IMessagesPublisher publisher, IMessagesConsumer consumer)
        {
            _usersProvider = usersProvider;
            _publisher = publisher;
            _consumer = consumer;
            _consumer.MessageReceived += OnMessage;
        }

        private void OnMessage(object sender, MessageModel message)
        {
            this.MessageReceived?.Invoke(this, message);
        }

        public event EventHandler<MessageModel> MessageReceived;

        public UserModel Login(string username, ConnectedClientModel client)
        {
            var user = new UserModel(username);
            user.Connect(client);
            _usersProvider.AddOrUpdate(user);
            this.UserLoggedIn?.Invoke(this, new UserLoginEventArgs(user));
            return user;
        }

        public IEnumerable<UserModel> GetAllUsers() => _usersProvider.GetAll();

        public event EventHandler<UserLoginEventArgs> UserLoggedIn;

        public void Logout(string username)
        {
            var user = _usersProvider.GetByUsername(username);
            if (null != user)
            {
                user.Disconnect();
                _usersProvider.AddOrUpdate(user);
            }
            
            this.UserLoggedOut?.Invoke(this, new UserLogoutEventArgs(username));
        }

        public event EventHandler<UserLogoutEventArgs> UserLoggedOut;

        public async Task PostMessageAsync(UserModel user, string message)
        {
            await _publisher.PublishAsync(new MessageModel(user.Username, message, DateTime.UtcNow));
        }
    }
}
