namespace LostAndFound.Application.Common.Models;

public class PaginatedResponse<T>
{
    public IReadOnlyList<T> Items { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public bool HasPreviousPage { get; }

    public bool HasNextPage { get; }

    public PaginatedResponse(
        IReadOnlyList<T> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;

        if (pageSize <= 0 || totalCount <= 0)
        {
            TotalPages = 0;
        }
        else
        {
            TotalPages = totalCount / pageSize;

            if (totalCount % pageSize != 0)
            {
                TotalPages++;
            }
        }

        HasPreviousPage = pageNumber > 1;
        HasNextPage = pageNumber < TotalPages;
    }
}