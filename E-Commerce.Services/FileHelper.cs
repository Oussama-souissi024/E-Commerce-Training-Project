using E_Commerce.Core.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Services
{
    public class FileHelper : IFileHelper
    {
        private readonly IWebHostEnvironment _webHost;
        public FileHelper(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }
        public string? UploadFile(IFormFile file, string folder)
        {
            if (file != null)
            {
                var fileDir = Path.Combine(_webHost.WebRootPath, folder);

                // Create directory if it doesn't exist
                if (!Directory.Exists(fileDir))
                {
                    Directory.CreateDirectory(fileDir);
                }

                var fileName = Guid.NewGuid() + "-" + file.FileName;
                var filePath = Path.Combine(fileDir, fileName);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    try
                    {
                        file.CopyTo(fileStream);
                        return fileName;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Cannot upload this file");
                        return string.Empty;
                    }

                }
            }
            else
            {
                return string.Empty;
            }
        }

        public bool DeleteFile(string imageUrl, string folder)
        {
            try
            {
                // Construct the full file path
                var filePath = Path.Combine(_webHost.WebRootPath, folder, imageUrl);

                // Check if the file exists
                if (File.Exists(filePath))
                {
                    File.Delete(filePath); // Delete the file
                    return true; // Return true if deletion is successful
                }
                else
                {
                    Console.WriteLine("File not found."); // Log if the file does not exist
                    return false; // Return false if the file does not exist
                }
            }
            catch (Exception ex)
            {
                // Log the exception message
                Console.WriteLine($"An error occurred while deleting the file: {ex.Message}");
                return false; // Return false in case of an exception
            }
        }

    }
}

