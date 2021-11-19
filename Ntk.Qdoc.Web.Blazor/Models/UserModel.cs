using System;

namespace Ntk.Qdoc.Web.Blazor.Models
{
    public class UserModel
    {
        public UserModel(string username)
        {
            Username = username;
        }

        public string Username { get; }
        public ConnectedClientModel Client { get; private set; }
        

        public void Connect(ConnectedClientModel client)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void Disconnect()
        {
            this.Client = null;
        }
    }
}
