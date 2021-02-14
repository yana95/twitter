using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TwitterMVC.Models;
using TwitterMVC.ViewModels;

namespace TwitterMVC.Services
{
    public class TwittsService: ITwittsService
    {
        private IHttpContextAccessor _httpContext;
        private IUsersService _usersService;
        private IBlobStorageService _blobStorageService;

        public TwittsService(IHttpContextAccessor context,  IUsersService usersService, IBlobStorageService blobStorageService)
        {
            _httpContext = context;
            _usersService = usersService;
            _blobStorageService = blobStorageService;
        }
        public async Task<List<TwittModel>> getUserTwitts() {
            User user = await _usersService.getCurrentUser();

            CloudTable table = await Storage.GetOrCreateTableAsync("Twitts");
            var query = new TableQuery<TwittModel>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, user.PartitionKey));

            List<TwittModel> twitts = table.ExecuteQuery(query).ToList();

            return twitts;
        }
        public async void deleteTwitt(string id) {
            TwittModel twitt = await getTwitt(id);

            _blobStorageService.DeleteBlobData(twitt.ImageUrl);

            CloudTable table = await Storage.GetOrCreateTableAsync("Twitts");
            TableOperation deleteOperation = TableOperation.Delete(twitt);

            await table.ExecuteAsync(deleteOperation);

        }
        public async void addTwitt(IFormFile file, string text) {
            string userEmail = _httpContext.HttpContext.User.Identity.Name;

            BlobImageModel blobData = fileToBlobData(file);

            string imageUrl = _blobStorageService.UploadFileToBlob(blobData);

            TwittModel twitt = new TwittModel(text, imageUrl, userEmail);

            CloudTable table = await Storage.GetOrCreateTableAsync("Twitts");
            TableOperation insertOrMergeOperation = TableOperation.Insert(twitt);

           await table.ExecuteAsync(insertOrMergeOperation);
        }

        public async Task<List<TwittModel>> getCurrentUserNews()
        {
            Dictionary<string, List<string>> relatedUsers = await _usersService.getRelatedUsers(_httpContext.HttpContext.User.Identity.Name);
            List<string> followings = relatedUsers["Followings"];
            List<TwittModel> twitts = new List<TwittModel>();
            CloudTable table = await Storage.GetOrCreateTableAsync("Twitts");

            followings.ForEach( f =>
            {
                var query = new TableQuery<TwittModel>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, f));
                var result = table.ExecuteQuery(query).ToList();
                twitts = twitts.Concat(result).ToList();
            });

           twitts.Sort((TwittModel x, TwittModel y) => y.Timestamp.CompareTo(x.Timestamp ));

            return twitts;
        }

        private BlobImageModel fileToBlobData (IFormFile file)
        {
            BlobImageModel blobImage = new BlobImageModel();
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                blobImage.FileData = target.ToArray();
            }

            blobImage.MimeType = file.ContentType;
            blobImage.FileName = file.Name;

            return blobImage;
        }

        public async void saveTwitt(IFormFile file, string text, string imgUrl, string twittId)
        {
            string newImgUrl = imgUrl;

            if(file != null)
            {
                BlobImageModel blobData = fileToBlobData(file);

                _blobStorageService.DeleteBlobData(imgUrl);
                newImgUrl = _blobStorageService.UploadFileToBlob(blobData);
            }

            TwittModel twitt = await getTwitt(twittId);

            twitt.ImageUrl = newImgUrl;
            twitt.Text = text;

            CloudTable table = await Storage.GetOrCreateTableAsync("Twitts");
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(twitt);

            await table.ExecuteAsync(insertOrMergeOperation);
        }

        private async Task<TwittModel> getTwitt(string twittId)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<TwittModel>(_httpContext.HttpContext.User.Identity.Name, twittId);
            CloudTable table = await Storage.GetOrCreateTableAsync("Twitts");
            TableResult result = await table.ExecuteAsync(retrieveOperation);
            TwittModel twitt = result.Result as TwittModel;

            return twitt;
        }
    }
}
