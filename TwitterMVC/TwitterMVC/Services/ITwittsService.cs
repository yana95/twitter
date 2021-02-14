using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterMVC.Models;

namespace TwitterMVC.Services
{
    public interface ITwittsService
    {
        public Task<List<TwittModel>> getUserTwitts();
        public void deleteTwitt(string id);
        public void addTwitt(IFormFile file, string text);

        public void saveTwitt(IFormFile file, string text, string imgUrl, string twittId);

        public Task<List<TwittModel>> getCurrentUserNews();
    }
}
