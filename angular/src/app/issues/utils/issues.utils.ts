import { EntityAttachmentDto, EntitySubType } from "src/app/proxy/entity-attachments";
import { FileAttachmentDto } from "src/app/proxy/entity-attachments/file-attachments";
import { IssueStatus } from "src/app/proxy/issues";
import { mapIssueStatusColor } from "src/app/shared/utils/stage-colors.utils";

export interface TabAttachments {
    tempFiles: FileAttachmentDto[];
    existingFiles: EntityAttachmentDto[];
}

export type IssueEntitySubType =
  | EntitySubType.Issue_Details
  | EntitySubType.Issue_RectificationAction
  | EntitySubType.Issue_QualityControl
  | EntitySubType.Issue_RepairerDetails;

export type IssueAttachmentsMap = Record<IssueEntitySubType, TabAttachments>;

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