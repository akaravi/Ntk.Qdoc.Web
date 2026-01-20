using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Ntk.Qdoc.Web.Blazor.Interfaces;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Services
{
    public class MessagesConsumerWorker : BackgroundService
    {
        private readonly IMessagesConsumer _consumer;
        private readonly IMessageRepository _messageRepository;

        public MessagesConsumerWorker(IMessagesConsumer consumer, IMessageRepository messageRepository)
        {
            _consumer = consumer;
            _messageRepository = messageRepository;
            _consumer.MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, Models.MessageModel message)
        {
            // ذخیره پیام در repository
            _messageRepository.AddMessage(message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.BeginConsumeAsync();
        }
    }
}
