import { EntityAttachmentDto, EntitySubType } from "src/app/proxy/entity-attachments";
import { FileAttachmentDto } from "src/app/proxy/entity-attachments/file-attachments";
import { IssueStatus } from "src/app/proxy/issues";
import { mapIssueStatusColor } from "src/app/shared/utils/stage-colors.utils";

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
    const color = mapIssueStatusColor(status);
    return color ? `bg-${color}` : '';
}