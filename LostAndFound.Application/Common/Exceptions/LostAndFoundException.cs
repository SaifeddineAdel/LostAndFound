namespace LostAndFound.Application.Common.Exceptions
{
    public abstract class LostAndFoundException : Exception
    {
        public int StatusCode { get; }

        protected LostAndFoundException(
            string message,
            int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
