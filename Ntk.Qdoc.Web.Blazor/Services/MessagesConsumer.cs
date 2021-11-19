using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Ntk.Qdoc.Web.Blazor.Interfaces;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Services
{


    public class MessagesConsumer : IMessagesConsumer
    {
        private readonly ChannelReader<MessageModel> _reader;

        public MessagesConsumer(ChannelReader<MessageModel> reader)
        {
            _reader = reader;
        }

        public async Task BeginConsumeAsync()
        {
            await foreach (var message in _reader.ReadAllAsync())
            {
                this.MessageReceived?.Invoke(this, message);
            }
        }

        public event EventHandler<MessageModel> MessageReceived;
    }
}
