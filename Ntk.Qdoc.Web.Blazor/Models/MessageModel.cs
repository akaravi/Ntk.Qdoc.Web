using System;

namespace Ntk.Qdoc.Web.Blazor.Models
{
    public class MessageModel
    {
        public MessageModel(string username, string text, DateTime when)
        {
            Username = username;
            Text = text;
            When = when;
        }
        public MessageModel(string username,string receiverUsername, string text, DateTime when)
        {
            Username = username;
            ReceiverUsername = receiverUsername;
            Text = text;
            When = when;
        }
        public string Username { get; }
        public string ReceiverUsername { get; }
        public string Text { get; }
        public DateTime When { get; }
    }
}
