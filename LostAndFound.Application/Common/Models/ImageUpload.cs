
namespace LostAndFound.Application.Common.Models;

public sealed class ImageUpload
{
    public Stream Content { get; }
    public long Length { get; }
    public string ContentType { get; }

    public ImageUpload(
        Stream content,
        long length,
        string contentType)
    {
        Content = content;
        Length = length;
        ContentType = contentType;
    }
}
