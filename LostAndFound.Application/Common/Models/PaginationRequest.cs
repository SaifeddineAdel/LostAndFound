
namespace LostAndFound.Application.Common.Models;

public class PaginationRequest
{
    private const int MaxPageSize = 50;
    private int _pageSize = 10;

    public int PageNumber { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1
            ? 10
            : Math.Min(value, MaxPageSize);
    }
}
