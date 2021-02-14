using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterMVC.Models;

namespace TwitterMVC.Services
{
    public interface IBlobStorageService
    {
        public string UploadFileToBlob(BlobImageModel BlobData);
        public  void DeleteBlobData(string fileUrl);
    }
}
