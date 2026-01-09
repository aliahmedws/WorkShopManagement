using System;

namespace WorkShopManagement.Issues;

public class IssueBuilder
{
    public static Issue Create(Guid carId, Guid issueId, UpsertIssueDto item)
    {
        var newIssue = new Issue(
                issueId,
                carId,
                item.XPercent,
                item.YPercent,
                item.Type,
                item.Status,
                item.OriginStage,
                item.DeteriorationType,
                item.Description,
                item.RectificationAction
            );

        return newIssue;
    }

    public static Issue Update(Issue issue, UpsertIssueDto item)
    {
        if (issue.XPercent != item.XPercent || issue.YPercent != item.YPercent)
        {
            issue.SetCoordinates(item.XPercent, item.YPercent);
        }

        if (issue.Type != item.Type)
        {
            issue.SetType(item.Type);
        }

        if (issue.Status != item.Status)
        {
            issue.SetStatus(item.Status);
        }

        if (issue.OriginStage != item.OriginStage)
        {
            issue.SetOriginStage(item.OriginStage);
        }

        if (issue.DeteriorationType != item.DeteriorationType)
        {
            issue.SetDeteriorationType(item.DeteriorationType);
        }

        if (!string.Equals(issue.Description, item.Description.Trim(), StringComparison.Ordinal))
        {
            issue.SetDescription(item.Description);
        }

        if (!string.Equals(issue.RectificationAction, item.RectificationAction?.Trim(), StringComparison.Ordinal))
        {
            issue.SetRectificationAction(item.RectificationAction);
        }

        return issue;
    }
}