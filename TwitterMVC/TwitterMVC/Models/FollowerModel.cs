using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterMVC.Models
{
    public class FollowerModel : TableEntity
    {
        public FollowerModel()
        {
        }
        public FollowerModel(string followerId, string followingId)
        {
            PartitionKey = followerId;
            RowKey = followingId;
        }
    }
}
