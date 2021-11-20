﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ntk.Qdoc.Web.Blazor.Interfaces;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]

    public class ChatController : ControllerBase
    {
        private readonly IUserStateProvider _usersProvider;
        private readonly IMessagesPublisher _publisher;
        private readonly IMessagesConsumer _consumer;

        public ChatController(IUserStateProvider usersProvider, IMessagesPublisher publisher, IMessagesConsumer consumer)
        {
            _usersProvider = usersProvider;
            _publisher = publisher;
            _consumer = consumer;
            //_consumer.MessageReceived += OnMessage;
        }



        [HttpPost]
        public IActionResult PostMessageAsync(SendMessageDtoModel model)
        {
            _publisher.PublishAsync(new MessageModel(model.Username, model.Message, DateTime.UtcNow));
            return new OkResult();
        }
    }
}
