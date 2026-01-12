import { EntityAttachmentDto, EntitySubType } from "src/app/proxy/entity-attachments";
import { FileAttachmentDto } from "src/app/proxy/entity-attachments/file-attachments";
import { IssueStatus } from "src/app/proxy/issues";

export interface TabAttachments {
    tempFiles: FileAttachmentDto[];
    existingFiles: EntityAttachmentDto[];
}

export type IssueAttachmentsMap = Record<EntitySubType, TabAttachments>;

export function createEmptyAttachmentsMap(): IssueAttachmentsMap {
    return {
        [EntitySubType.Issue_Details]: { tempFiles: [], existingFiles: [] },
        [EntitySubType.Issue_RectificationAction]: { tempFiles: [], existingFiles: [] },
        [EntitySubType.Issue_QualityControl]: { tempFiles: [], existingFiles: [] },
        [EntitySubType.Issue_RepairerDetails]: { tempFiles: [], existingFiles: [] },
    };
}

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