using Microsoft.AspNetCore.Http;

namespace E_Commerce.Core.Interfaces
{
    // Interface for handling file operations in the application
    // Primarily used for managing product images and other uploads
    public interface IFileHelper
    {
        // Uploads a file to the specified folder
        // Returns the URL/path of the uploaded file, or null if upload fails
        // folder parameter specifies the destination directory
        string? UploadFile(IFormFile file, string folder);

        // Deletes a file from the specified folder
        // Returns true if deletion successful, false otherwise
        // imageUrl: path to the file, Folfer: directory containing the file
        public bool DeleteFile(string imageUrl, string Folfer);
    }
}
