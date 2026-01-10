import { FileAttachmentDto } from "src/app/proxy/entity-attachments/file-attachments";
import { IssueDto, IssueStatus, UpsertIssueDto } from "src/app/proxy/issues";

export function mapIssueStatusBgColor(status: IssueStatus | null): string | '' {
    if (!status) return '';

    switch (status) {
        case IssueStatus.Pending:
            return 'bg-danger';

        case IssueStatus.Deferred:
            return 'bg-warning';

        case IssueStatus.InProgress:
            return 'bg-info';

        case IssueStatus.NoActionRequired:
            return 'bg-success';

        case IssueStatus.Resolved:
            return 'bg-success';

        default:
            return 'bg-dark';
    }
}

export function mapToUpsertIssueDto(issue: IssueDto, tempFiles?: FileAttachmentDto[]): UpsertIssueDto {
    const upsert: UpsertIssueDto = {
        id: issue.id,
        srNo: issue.srNo,
        xPercent: issue.xPercent,
        yPercent: issue.yPercent,
        type: issue.type,
        status: issue.status,
        originStage: issue.originStage,
        deteriorationType: issue.deteriorationType,
        description: issue.description,
        rectificationAction: issue.rectificationAction,
        rectificationNotes: issue.rectificationNotes,
        qualityControlAction: issue.qualityControlAction,
        qualityControlNotes: issue.qualityControlNotes,
        repairerAction: issue.repairerAction,
        repairerNotes: issue.repairerNotes,
        tempFiles: tempFiles ?? [],
        entityAttachments: issue.entityAttachments ?? []
    };

    return upsert;
}