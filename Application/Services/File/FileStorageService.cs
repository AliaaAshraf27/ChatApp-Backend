using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.File
{
    public class FileStorageService(IWebHostEnvironment _environment) : IFileStorageService
    {
        public async Task<FileUploadResult> UploadAsync(IFormFile file, string folder)
        {
            var webRootPath = _environment.WebRootPath;
            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath,"wwwroot");
            }

            var uploadFolder = Path.Combine(webRootPath,"Uploads",folder);
            Directory.CreateDirectory(uploadFolder);

            var extension = Path.GetExtension(file.FileName);
            var uniqueFileName =$"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(uploadFolder,uniqueFileName);
            await using var stream = new FileStream(filePath,FileMode.Create);
            await file.CopyToAsync(stream);

            return new FileUploadResult
            {
                Url = $"/Uploads/{folder}/{uniqueFileName}",
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length
            };
        }

        public Task DeleteAsync(string url)
        {
            var webRootPath = _environment.WebRootPath;

            if (string.IsNullOrEmpty(webRootPath))
            {
                webRootPath = Path.Combine(_environment.ContentRootPath,"wwwroot");
            }

            var filePath = Path.Combine(webRootPath,url.TrimStart('/'));

            //if (File.Exists(filePath))
            //{
            //    File.Delete(filePath);
            //}

            return Task.CompletedTask;
        }
    }
}
