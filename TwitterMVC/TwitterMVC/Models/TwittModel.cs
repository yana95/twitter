using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterMVC.Models
{
    using Microsoft.Azure.Cosmos.Table;
    public class TwittModel : TableEntity
    {
        public string Text { get; set; }
        public string ImageUrl { get; set; }

        public TwittModel()
        {
        }

        public TwittModel(string text, string imgUrl, string userEmail)
        {
            PartitionKey = userEmail;
            RowKey = Guid.NewGuid().ToString();
            Text = text;
            ImageUrl = imgUrl;
        }

    }
}
