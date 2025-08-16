using Microsoft.AspNetCore.Http;

namespace StaffManagementN.Interfaces;

public interface IFileService
{
    /// <summary>
    /// Uploads a file to the specified directory
    /// </summary>
    /// <param name="file">The file to upload</param>
    /// <param name="uploadDirectory">The directory to upload to (e.g., "books", "employees", "members")</param>
    /// <returns>The relative path to the uploaded file</returns>
    Task<string> UploadFileAsync(IFormFile file, string uploadDirectory);

    /// <summary>
    /// Deletes a file from the specified path
    /// </summary>
    /// <param name="filePath">The relative path to the file to delete</param>
    Task DeleteFileAsync(string filePath);
} 