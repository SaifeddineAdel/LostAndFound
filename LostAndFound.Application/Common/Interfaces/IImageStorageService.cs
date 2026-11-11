using LostAndFound.Application.Common.Models;

namespace LostAndFound.Application.Common.Interfaces;

public interface IImageStorageService
{
    Task<string> SaveAsync(ImageUpload image);

    Task DeleteAsync(string imageUrl);
}