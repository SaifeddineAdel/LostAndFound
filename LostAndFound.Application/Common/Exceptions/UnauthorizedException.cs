namespace LostAndFound.Application.Common.Exceptions;

public sealed class UnauthorizedException : LostAndFoundException
{
    public UnauthorizedException(string message)
        : base(message, 401)
    {
    }
}