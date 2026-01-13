using System.Collections.Generic;
using System.Linq;
using WorkShopManagement.Issues;
using WorkShopManagement.Recalls;

namespace WorkShopManagement.Stages;

public static class StageColorHelper
{
    public static RecallStatus? MapRecallStatus(IEnumerable<RecallStatus>? values)
    {
        if (values == null)
            return null;

        var list = values as IList<RecallStatus> ?? [.. values];
        return list.Count == 0 ? null : list.Min();
    }

    public static IssueStatus? MapIssueStatus(IEnumerable<IssueStatus>? values)
    {
        if (values == null)
            return null;

        var list = values as IList<IssueStatus> ?? [.. values];
        return list.Count == 0 ? null : list.Min();
    }
}
