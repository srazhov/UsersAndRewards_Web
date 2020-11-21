using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace UsersAndRewards.Csharp
{
    public class CommonActions
    {
        public static async Task<string> CreateImage(IWebHostEnvironment webHostEnvironment, IFormFile uploadFile, string path)
        {
            if (uploadFile != null)
            {
                var extension = Path.GetExtension(uploadFile.FileName);
                var nPath = GetRandomName(webHostEnvironment, path, extension);

                using var fs = new FileStream(webHostEnvironment.WebRootPath + nPath, FileMode.Create);
                await uploadFile.CopyToAsync(fs);
                return nPath;
            }

            return null;
        }

        private static string GetRandomName(IWebHostEnvironment webHostEnvironment, string path, string extension)
        {
            var neededPath = @webHostEnvironment.WebRootPath + @path;
            string currentName;
            do
            {
                currentName = $@"{DateTime.Now.Ticks}{extension}";
            }
            while (File.Exists(neededPath + currentName));

            return path + currentName;
        }
    }
}
