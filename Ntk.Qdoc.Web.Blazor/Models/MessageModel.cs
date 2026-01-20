using System;

namespace Ntk.Qdoc.Web.Blazor.Models
{
    public class MessageModel
    {
        // Constructor for public messages
        public MessageModel(string username, string text, DateTime when)
        {
            Username = username;
            Text = text;
            When = when;
            ChatId = null;
            ReceiverUsername = null;
        }

        // Constructor for private messages (1-to-1)
        public MessageModel(string username, string receiverUsername, string text, DateTime when)
        {
            Username = username;
            ReceiverUsername = receiverUsername;
            Text = text;
            When = when;
            ChatId = null; // Private chat doesn't use ChatId
        }

        // Constructor for group chat messages
        public MessageModel(string username, string chatId, string text, DateTime when, bool isGroupChat = true)
        {
            Username = username;
            ChatId = chatId;
            Text = text;
            When = when;
            ReceiverUsername = null; // Group chat doesn't have single receiver
        }

        public string Username { get; }
        public string ReceiverUsername { get; } // For private 1-to-1 chats
        public string ChatId { get; } // For group chats
        public string Text { get; }
        public DateTime When { get; }
        
        // Helper properties
        public bool IsGroupChat => !string.IsNullOrEmpty(ChatId);
        public bool IsPrivateChat => !string.IsNullOrEmpty(ReceiverUsername);
        public bool IsPublicMessage => string.IsNullOrEmpty(ChatId) && string.IsNullOrEmpty(ReceiverUsername);
    }
}
