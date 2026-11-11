namespace LostAndFound.Application.Common.Exceptions
{
    public class NotFoundException : LostAndFoundException
    {
        public NotFoundException(string message)
            : base(message, 404)
        {
        }
    }
}
