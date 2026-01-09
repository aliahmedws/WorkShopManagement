using System;

namespace WorkShopManagement.Issues;

public class IssueBuilder
{
    public static Issue Create(Guid carId, Guid issueId, UpsertIssueDto item)
    {
        var newIssue = new Issue(
            issueId,
            item.SrNo,
            carId,
            item.XPercent,
            item.YPercent,
            item.Type,
            item.Status,
            item.OriginStage,
            item.DeteriorationType,
            item.Description,
            item.RectificationAction,
            item.RectificationNotes,
            item.QualityControlAction,
            item.QualityControlNotes,
            item.RepairerAction,
            item.RepairerNotes
        );

        return newIssue;
    }

    public static Issue Update(Issue issue, UpsertIssueDto item)
    {
        if (issue.SrNo != item.SrNo) issue.SetSrNo(item.SrNo);
        if (issue.XPercent != item.XPercent || issue.YPercent != item.YPercent) issue.SetCoordinates(item.XPercent, item.YPercent);
        if (issue.Type != item.Type) issue.SetType(item.Type);
        if (issue.Status != item.Status) issue.SetStatus(item.Status);
        if (issue.OriginStage != item.OriginStage) issue.SetOriginStage(item.OriginStage);
        if (issue.DeteriorationType != item.DeteriorationType) issue.SetDeteriorationType(item.DeteriorationType);
        if (!string.Equals(issue.Description, item.Description.Trim(), StringComparison.Ordinal)) issue.SetDescription(item.Description);

        if (!string.Equals(issue.RectificationAction, item.RectificationAction?.Trim(), StringComparison.Ordinal) || !string.Equals(issue.RectificationNotes, item.RectificationNotes?.Trim(), StringComparison.Ordinal)) issue.SetRectificationAction(item.RectificationAction, item.RectificationNotes);
        if (!string.Equals(issue.QualityControlAction, item.QualityControlAction?.Trim(), StringComparison.Ordinal) || !string.Equals(issue.QualityControlNotes, item.QualityControlNotes?.Trim(), StringComparison.Ordinal)) issue.SetQualityControlAction(item.QualityControlAction, item.QualityControlNotes);
        if (!string.Equals(issue.RepairerAction, item.RepairerAction?.Trim(), StringComparison.Ordinal) || !string.Equals(issue.RepairerNotes, item.RepairerNotes?.Trim(), StringComparison.Ordinal)) issue.SetRepairerAction(item.RepairerAction, item.RepairerNotes);

        return issue;
    }
}