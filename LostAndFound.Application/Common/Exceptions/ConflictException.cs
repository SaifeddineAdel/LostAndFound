namespace LostAndFound.Application.Common.Exceptions
{
    public class ConflictException : LostAndFoundException
    {
        public ConflictException(string message)
            : base(message, 409)
        {
        }
    }
}
