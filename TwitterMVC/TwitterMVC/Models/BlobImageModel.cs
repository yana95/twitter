using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TwitterMVC.Models
{
    public class BlobImageModel
    {
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        
        public string MimeType { get; set; }
    }
}
