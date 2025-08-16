using Microsoft.AspNetCore.Http;
using StaffManagementN.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace StaffManagementN.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileService> _logger;
    private const string UploadsBaseDirectory = "uploads";
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".heic", ".heif" };

    public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string uploadDirectory)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("No file was provided", nameof(file));
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new ArgumentException($"File type {extension} is not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}");
        }

        // Create the uploads directory if it doesn't exist
        var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsBaseDirectory, uploadDirectory);
        Directory.CreateDirectory(uploadsPath);

        // Generate a unique filename
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsPath, fileName);

        try
        {
            // For HEIC/HEIF files, convert to JPEG
            if (extension == ".heic" || extension == ".heif")
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var image = await Image.LoadAsync(stream))
                    {
                        // Change the extension to .jpg for the output file
                        fileName = Path.ChangeExtension(fileName, ".jpg");
                        filePath = Path.Combine(uploadsPath, fileName);

                        // Save as JPEG
                        await image.SaveAsJpegAsync(filePath);
                    }
                }
            }
            else
            {
                // For other image types, save directly
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }

            _logger.LogInformation("File uploaded successfully: {FilePath}", filePath);
            return Path.Combine(UploadsBaseDirectory, uploadDirectory, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file: {FileName}", file.FileName);
            throw;
        }
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            return Task.CompletedTask;
        }

        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath, filePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                _logger.LogInformation("File deleted successfully: {FilePath}", fullPath);
            }
            else
            {
                _logger.LogWarning("File not found for deletion: {FilePath}", fullPath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
            throw;
        }

        return Task.CompletedTask;
    }
} 