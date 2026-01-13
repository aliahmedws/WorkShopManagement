using System.Collections.Generic;

namespace WorkShopManagement.Common;

public class ListResult<T> where T : class
{
    public long TotalCount { get; set; }
    public List<T> Items { get; set; } = [];

    public ListResult()
    {
        
    }

    public ListResult(long totalCount)
    {
        TotalCount = totalCount;
    }

    public ListResult(List<T> items, long totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }
}