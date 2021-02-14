using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterMVC.Models
{
    using Microsoft.Azure.Cosmos.Table;
    public class User: TableEntity
    {
        public string Password { get; set; }
        public List<TwittModel> Twitts { get; set; }
        public  List<string> Followings { get; set; }
        public  List<string> Followers { get; set; }

        public User()
        {
        }

        public User(string email, string password)
        {
            PartitionKey = email;
            RowKey = Guid.NewGuid().ToString();
            Password = password;
            Twitts = new List<TwittModel>();
            Followings = new List<string>();
            Followers = new List<string>();
        }

    }
}
