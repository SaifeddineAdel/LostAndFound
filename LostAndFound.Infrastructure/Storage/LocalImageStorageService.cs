
using LostAndFound.Application.Common.Interfaces;
using LostAndFound.Application.Common.Models;
using LostAndFound.Infrastructure.Common.Exceptions;
using Microsoft.AspNetCore.Hosting;

namespace LostAndFound.Infrastructure.Storage;

public sealed class LocalImageStorageService : IImageStorageService
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private readonly string _imagesDirectory;

    public LocalImageStorageService(IWebHostEnvironment environment)
    {
        var webRoot = environment.WebRootPath
            ?? Path.Combine(environment.ContentRootPath, "wwwroot");

        _imagesDirectory = Path.Combine(
            webRoot,
            "uploads",
            "reports");
    }

    public async Task<string> SaveAsync(ImageUpload image)
    {
        if (image.Length <= 0)
            throw new InvalidImageException("The image is empty.");

        if (image.Length > MaxFileSize)
            throw new InvalidImageException(
                "Each image must be 5 MB or smaller.");

        if (!image.Content.CanSeek)
            throw new InvalidImageException(
                "The image stream must support seeking.");

        var header = new byte[12];

        image.Content.Position = 0;

        var bytesRead = await image.Content.ReadAsync(
            header.AsMemory(0, header.Length));

        var extension = GetImageExtension(header, bytesRead);

        if (extension is null)
            throw new InvalidImageException(
                "Only JPEG, PNG, and WebP images are allowed.");

        Directory.CreateDirectory(_imagesDirectory);

        var fileName = $"{Guid.NewGuid():N}{extension}";
        var filePath = Path.Combine(_imagesDirectory, fileName);

        image.Content.Position = 0;

        await using (var output = new FileStream(
            filePath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            useAsync: true))
        {
            await image.Content.CopyToAsync(output);
        }

        return $"/uploads/reports/{fileName}";
    }

    public Task DeleteAsync(string imageUrl)
    {
        const string prefix = "/uploads/reports/";

        if (!imageUrl.StartsWith(prefix, StringComparison.Ordinal))
            return Task.CompletedTask;

        var fileName = imageUrl[prefix.Length..];

        if (string.IsNullOrWhiteSpace(fileName) ||
            fileName != Path.GetFileName(fileName))
        {
            return Task.CompletedTask;
        }

        var filePath = Path.Combine(_imagesDirectory, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }

    private static string? GetImageExtension(byte[] header, int length)
    {
        // JPEG signature
        if (length >= 3 &&
            header[0] == 0xFF &&
            header[1] == 0xD8 &&
            header[2] == 0xFF)
        {
            return ".jpg";
        }

        // PNG signature
        if (length >= 8 &&
            header[0] == 0x89 &&
            header[1] == 0x50 &&
            header[2] == 0x4E &&
            header[3] == 0x47 &&
            header[4] == 0x0D &&
            header[5] == 0x0A &&
            header[6] == 0x1A &&
            header[7] == 0x0A)
        {
            return ".png";
        }

        // WebP signature
        if (length >= 12 &&
            header[0] == (byte)'R' &&
            header[1] == (byte)'I' &&
            header[2] == (byte)'F' &&
            header[3] == (byte)'F' &&
            header[8] == (byte)'W' &&
            header[9] == (byte)'E' &&
            header[10] == (byte)'B' &&
            header[11] == (byte)'P')
        {
            return ".webp";
        }

        return null;
    }
}
