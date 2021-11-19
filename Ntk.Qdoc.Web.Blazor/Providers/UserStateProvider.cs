using System.Collections.Concurrent;
using System.Collections.Generic;
using Ntk.Qdoc.Web.Blazor.Models;

namespace Ntk.Qdoc.Web.Blazor.Providers
{
    public interface IUserStateProvider
    {
        void AddOrUpdate(UserModel state);
        IEnumerable<UserModel> GetAll();
        void Remove(string username);
        UserModel GetByClient(ConnectedClientModel client);
        UserModel GetByUsername(string username);
    }

    public class UserStateProvider : IUserStateProvider
    {
        private ConcurrentDictionary<string, UserModel> _users;
        private ConcurrentDictionary<string, UserModel> _usersByClientId;
        public UserStateProvider()
        {
            _users = new ConcurrentDictionary<string, UserModel>();
            _usersByClientId = new ConcurrentDictionary<string, UserModel>();
        }

        public void AddOrUpdate(UserModel state)
        {
            _users.AddOrUpdate(state.Username, k => state, (k, s) => state);
            if (state.Client == null)
            {
                Remove(state.Username);
                return;
            }
            _usersByClientId.AddOrUpdate(state.Client.Id, k => state, (k, s) => state);
        }

        public UserModel GetByUsername(string username)
        {
            return _users.ContainsKey(username) ? _users[username] : null;
        }

        public UserModel GetByClient(ConnectedClientModel client)
        {
            return _usersByClientId.ContainsKey(client.Id) ? _usersByClientId[client.Id] : null;
        }

        public IEnumerable<UserModel> GetAll()
        {
            return _users.Values;
        }

        public void Remove(string username)
        {
            _users.TryRemove(username, out var _);
        }
    }
}
