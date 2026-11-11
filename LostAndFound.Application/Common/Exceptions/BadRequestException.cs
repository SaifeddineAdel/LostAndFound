namespace LostAndFound.Application.Common.Exceptions
{
    public class BadRequestException : LostAndFoundException
    {
        public BadRequestException(string message)
            : base(message, 400)
        {
        }
    }
}
