using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.File
{
    public interface IFileStorageService
    {
        Task<FileUploadResult> UploadAsync(IFormFile file, string folder);
        Task DeleteAsync(string url);
    }
}
