namespace Api.Features.Abstractions;

public class PagedList<T>(List<T> items, int page, int pageSize, int totalCount)
{
    public List<T> Items { get; private set; } = items;

    public int Page { get; private set; } = page;

    public int PageSize { get; private set; } = pageSize;

    public int TotalCount { get; private set; } = totalCount;

    public bool HasNextPage => Page * PageSize < TotalCount;

    public bool HasPreviousPage => Page > 1;
}
