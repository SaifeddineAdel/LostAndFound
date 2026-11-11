namespace LostAndFound.Api.Common.ExceptionHandling
{
    public sealed class ErrorResponse
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
    }
}