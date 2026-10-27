using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.File
{
    public class FileUploadResult
    {
        public string Url { get; set; }
        public string FileName { get; set; } 
        public string ContentType { get; set; } 
        public long FileSizeBytes { get; set; }
    }
}
