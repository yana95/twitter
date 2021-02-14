using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitterMVC.Models;

namespace TwitterMVC.Services
{
    public interface IUsersService
    {
        public Task<User> addNewUser(User user);
        public Task<User> getUser(string email, string password);
        public Task<User> getCurrentUser();

        public Task<List<User>> getUsersExceptCurrrent();

        public Task<TableResult> followTo(string id);

        public Task<TableResult> unfollow(string id);

        public Task<Dictionary<string, List<string>>> getRelatedUsers(string id);
    }
}
