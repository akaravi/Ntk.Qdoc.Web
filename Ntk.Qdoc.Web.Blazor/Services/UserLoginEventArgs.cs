using System;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Services
{
    public class UserLoginEventArgs : EventArgs
    {
        public UserModel User { get; }

        public UserLoginEventArgs(UserModel user)
        {
            User = user;
        }
    }

}
