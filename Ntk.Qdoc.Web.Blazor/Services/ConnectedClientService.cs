using Ntk.Qdoc.Web.Blazor.Interfaces;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Services
{

    public class InMemoryConnectedClientService : IConnectedClientService
    {
        public ConnectedClientModel Client { get; private set; }

        public void Connect(string id)
        {
            this.Client = new ConnectedClientModel(id);
        }

        public void Disconnect()
        {
            this.Client = null;
        }
    }

}
