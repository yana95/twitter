using System.Collections.Generic;
using TwitterMVC.Models;

namespace TwitterMVC.ViewModels
{
    public class HomePageModel
    {

        public string Text { get; set; }
        public string ImgUrl { get; set; }

        public string EditedTwittId { get; set; }

        public List<TwittModel> Twitts { get; set; }
    }
}
