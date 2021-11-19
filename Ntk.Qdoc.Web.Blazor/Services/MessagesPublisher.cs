using System.Threading.Channels;
using System.Threading.Tasks;
using Ntk.Qdoc.Web.Blazor.Interfaces;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Services
{
 
    public class MessagesPublisher : IMessagesPublisher
    {
        private readonly ChannelWriter<MessageModel> _writer;

        public MessagesPublisher(ChannelWriter<MessageModel> writer)
        {
            _writer = writer;
        }

        public async Task PublishAsync(MessageModel message)
        {
            await _writer.WriteAsync(message);
        }
    }
}
