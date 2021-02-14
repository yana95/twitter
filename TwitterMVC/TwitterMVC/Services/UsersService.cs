using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterMVC.Models;

namespace TwitterMVC.Services
{
    public class UsersService: IUsersService
    {
        private IHttpContextAccessor _httpContext;

        public UsersService(IHttpContextAccessor context)
        {
            _httpContext = context;
        }

        public async Task<User> addNewUser( User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                CloudTable table = await Storage.GetOrCreateTableAsync("Users");

                // Create the InsertOrReplace table operation
                TableOperation insertOrMergeOperation = TableOperation.Insert(user);

                // Execute the operation.
                TableResult result = await table.ExecuteAsync(insertOrMergeOperation);
                User insertedUser = result.Result as User;

                return insertedUser;
            }
            catch (StorageException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
                throw;
            }
        }

        public async Task<User> getUser(string email, string password)
        {
            CloudTable table = await Storage.GetOrCreateTableAsync("Users");

            string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, email);

            string psFilter = TableQuery.GenerateFilterCondition("Password", QueryComparisons.Equal, password);

            string combinedRowKeyFilter = TableQuery.CombineFilters(psFilter, TableOperators.And, pkFilter);
            TableQuery<User> query = new TableQuery<User>().Where(combinedRowKeyFilter);
            User user = table.ExecuteQuery(query).FirstOrDefault();

            if(user != null)
            {
                Dictionary<string, List<string>> relatedUsers = await getRelatedUsers(user.PartitionKey);

                user.Followers = relatedUsers["Followers"];
                user.Followings = relatedUsers["Followings"];
            }

            return user;
        }

        public async Task<User> getUserByEmail(string email)
        {
            CloudTable table = await Storage.GetOrCreateTableAsync("Users");

            var query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, email));
            User user = table.ExecuteQuery(query).First();
            Dictionary<string, List<string>> relatedUsers = await getRelatedUsers(user.PartitionKey);

            user.Followers = relatedUsers["Followers"];
            user.Followings = relatedUsers["Followings"];

            return user;
        }

        public async Task<User> getCurrentUser()
        {
            string userEmail = _httpContext.HttpContext.User.Identity.Name;

            CloudTable table = await Storage.GetOrCreateTableAsync("Users");
            var query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, userEmail));

            User user = table.ExecuteQuery(query).First();
            Dictionary<string, List<string>> relatedUsers = await getRelatedUsers(user.PartitionKey);

            user.Followers = relatedUsers["Followers"];
            user.Followings = relatedUsers["Followings"];

            return user;
        }

        public async Task<List<User>> getUsersExceptCurrrent()
        {
            string userEmail = _httpContext.HttpContext.User.Identity.Name;
            CloudTable table = await Storage.GetOrCreateTableAsync("Users");
            var query = new TableQuery<User>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, userEmail));

            List<User> users = table.ExecuteQuery(query).ToList();

            foreach (User user in users)
            {
                Dictionary<string, List<string>> relatedUsers = await getRelatedUsers(user.PartitionKey);

                user.Followers = relatedUsers["Followers"];
                user.Followings = relatedUsers["Followings"];
            }

            return users;
        }

        public async Task<TableResult> followTo(string id)
        {
            User currentUser = await getCurrentUser();
            User followToUser = await getUserByEmail(id);

            CloudTable table = await Storage.GetOrCreateTableAsync("Followers");
            FollowerModel follower = new FollowerModel(currentUser.PartitionKey, followToUser.PartitionKey);
                TableOperation insertOrMergeOperation = TableOperation.Insert(follower);

            var result = await table.ExecuteAsync(insertOrMergeOperation);

            return result;
        }

        public async Task<TableResult> unfollow(string id)
        {
            CloudTable table = await Storage.GetOrCreateTableAsync("Followers");

            TableOperation tableOperation = TableOperation.Retrieve<FollowerModel>(_httpContext.HttpContext.User.Identity.Name, id);
            FollowerModel follower = table.Execute(tableOperation).Result as FollowerModel;

            TableOperation deleteOperation = TableOperation.Delete(follower);

            var result = await table.ExecuteAsync(deleteOperation);

            return result;
        }

        public async Task<Dictionary<string, List<string>>> getRelatedUsers(string id)
        {
            CloudTable table = await Storage.GetOrCreateTableAsync("Followers");

            string pkFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id);

            string rkFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id);

            string combinedRowKeyFilter = TableQuery.CombineFilters(rkFilter, TableOperators.Or, pkFilter);
            TableQuery<FollowerModel> query = new TableQuery<FollowerModel>().Where(combinedRowKeyFilter);
            List<FollowerModel> relatedUsers = table.ExecuteQuery(query).ToList();

            List<string> followers = new List<string>();
            List<string> followings = new List<string>();

            relatedUsers.ForEach(relatedUser =>
            {
                if (relatedUser.PartitionKey == id)
                {
                    followings.Add(relatedUser.RowKey);
                }

                if (relatedUser.RowKey == id)
                {
                    followers.Add(relatedUser.PartitionKey);
                }
            });

            Dictionary<string, List<string>> relatedUsersDictionary = new Dictionary<string, List<string>>();
            relatedUsersDictionary.Add("Followers", followers);
            relatedUsersDictionary.Add("Followings", followings);

            return relatedUsersDictionary;
        }
    }
}
