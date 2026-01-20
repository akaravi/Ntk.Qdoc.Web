using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IChatThreadService _chatThreadService;
        private readonly IMessageRepository _messageRepository;

        public ChatService(
            IUserStateProvider usersProvider, 
            IMessagesPublisher publisher, 
            IMessagesConsumer consumer,
            IChatThreadService chatThreadService,
            IMessageRepository messageRepository)
        {
            _usersProvider = usersProvider;
            _publisher = publisher;
            _consumer = consumer;
            _chatThreadService = chatThreadService;
            _messageRepository = messageRepository;
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

        public UserModel CheckUserExist(UserModel user, string userCode)
        {
            return _usersProvider.GetByUsername(userCode);
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

        // Existing methods - maintain backward compatibility
        public async Task PostMessageAsync(UserModel user, string message)
        {
            await _publisher.PublishAsync(new MessageModel(user.Username, message, DateTime.UtcNow));
        }

        public async Task PostMessageAsync(UserModel user, string userCode, string message)
        {
            await _publisher.PublishAsync(new MessageModel(user.Username, userCode, message, DateTime.UtcNow));
        }

        // New group chat methods
        public ChatThreadModel CreateGroupChat(List<string> participants, string createdBy, string chatName = null)
        {
            if (participants == null || participants.Count < 2)
                throw new ArgumentException("At least 2 participants required for group chat");

            return _chatThreadService.CreateChatThread(participants, createdBy, chatName);
        }

        public async Task PostMessageToChatAsync(string chatId, UserModel user, string message)
        {
            if (string.IsNullOrEmpty(chatId))
                throw new ArgumentException("ChatId cannot be null or empty");

            var chatThread = _chatThreadService.GetChatThread(chatId);
            if (chatThread == null)
                throw new InvalidOperationException("Chat thread not found");

            if (!chatThread.IsParticipant(user.Username))
                throw new UnauthorizedAccessException("User is not a participant in this chat");

            var msg = new MessageModel(user.Username, chatId, message, DateTime.UtcNow, true);
            await _publisher.PublishAsync(msg);

            // Update last message info
            _chatThreadService.UpdateLastMessage(chatId, msg.When, message);

            // Increment unread count for other participants
            foreach (var participant in chatThread.Participants)
            {
                if (participant != user.Username)
                {
                    _chatThreadService.IncrementUnreadCount(chatId, participant);
                }
            }
        }

        public IEnumerable<MessageModel> GetMessagesForChat(string chatId, string username)
        {
            if (string.IsNullOrEmpty(chatId) || string.IsNullOrEmpty(username))
                return Enumerable.Empty<MessageModel>();

            var chatThread = _chatThreadService.GetChatThread(chatId);
            if (chatThread == null || !chatThread.IsParticipant(username))
                return Enumerable.Empty<MessageModel>();

            return _messageRepository.GetMessages(chatId, 100);
        }

        public IEnumerable<MessageModel> GetPrivateChatMessages(string user1, string user2)
        {
            if (string.IsNullOrEmpty(user1) || string.IsNullOrEmpty(user2))
                return Enumerable.Empty<MessageModel>();

            return _messageRepository.GetPrivateChatMessages(user1, user2, 100);
        }

        public IEnumerable<ChatThreadModel> GetChatThreadsForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return Enumerable.Empty<ChatThreadModel>();

            return _chatThreadService.GetChatThreadsForUser(username);
        }

        public IEnumerable<ChatThreadModel> GetActiveChatThreadsForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return Enumerable.Empty<ChatThreadModel>();

            // ابتدا threadهای گروهی فعال را می‌گیریم
            var groupChats = _chatThreadService.GetActiveChatThreadsForUser(username).ToList();
            
            // سپس چت‌های خصوصی فعال را پیدا می‌کنیم
            var allUsers = _usersProvider.GetAll();
            var privateChats = new List<ChatThreadModel>();

            foreach (var user in allUsers)
            {
                if (user.Username == username)
                    continue;

                // بررسی می‌کنیم آیا پیامی بین این دو کاربر رد و بدل شده یا نه
                var messages = _messageRepository.GetPrivateChatMessages(username, user.Username, 1);
                if (messages.Any())
                {
                    // یک ChatThreadModel موقت برای چت خصوصی ایجاد می‌کنیم
                    // برای چت خصوصی ChatId نمی‌دهیم (null باقی می‌ماند)
                    var privateThread = new ChatThreadModel(
                        new List<string> { username, user.Username },
                        username,
                        user.Username
                    );
                    
                    // ChatId را null نگه می‌داریم تا مشخص باشد که این یک چت خصوصی است
                    privateThread.ChatId = null;

                    // آخرین پیام را پیدا می‌کنیم
                    var lastMessage = messages.OrderByDescending(m => m.When).FirstOrDefault();
                    if (lastMessage != null)
                    {
                        privateThread.LastMessageTime = lastMessage.When;
                        privateThread.LastMessagePreview = lastMessage.Text;
                        if (privateThread.LastMessagePreview.Length > 50)
                        {
                            privateThread.LastMessagePreview = privateThread.LastMessagePreview.Substring(0, 50) + "...";
                        }
                    }

                    privateChats.Add(privateThread);
                }
            }

            // ترکیب threadهای گروهی و خصوصی و مرتب‌سازی بر اساس زمان آخرین پیام
            return groupChats.Concat(privateChats)
                .OrderByDescending(ct => ct.LastMessageTime)
                .ToList();
        }
    }
}
