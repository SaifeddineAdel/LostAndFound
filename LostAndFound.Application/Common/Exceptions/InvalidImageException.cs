using LostAndFound.Application.Common.Exceptions;

namespace LostAndFound.Infrastructure.Common.Exceptions;

public class InvalidImageException : LostAndFoundException
{
    public InvalidImageException(string message)
        : base(message, 400)
    {
    }
}
