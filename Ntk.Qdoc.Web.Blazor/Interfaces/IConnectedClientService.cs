﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Interfaces
{
    public interface IConnectedClientService
    {
        ConnectedClientModel Client { get; }

        void Connect(string id);
        void Disconnect();
    }

}
