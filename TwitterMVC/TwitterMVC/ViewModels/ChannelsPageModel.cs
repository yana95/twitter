using System.Collections.Generic;
using TwitterMVC.Models;

namespace TwitterMVC.ViewModels
{
    public class ChannelsPageModel
    {
        public User CurrentUser { get; set; }
        public List<User> Channels { get; set; }

    }
}
