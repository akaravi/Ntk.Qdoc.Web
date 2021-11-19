using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Interfaces
{
    public interface IUserStateProvider
    {
        void AddOrUpdate(UserModel state);
        IEnumerable<UserModel> GetAll();
        void Remove(string username);
        UserModel GetByClient(ConnectedClientModel client);
        UserModel GetByUsername(string username);
    }
}
