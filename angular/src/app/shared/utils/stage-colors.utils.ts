import { AvvStatus } from "src/app/proxy/car-bays";
import { CreStatus } from "src/app/proxy/cars";
import { CheckListProgressStatus } from "src/app/proxy/check-lists";
import { IssueStatus } from "src/app/proxy/issues";
import { QualityGateStatus } from "src/app/proxy/quality-gates/quality-gate-status.enum";
import { RecallStatus } from "src/app/proxy/recalls";

const DEFAULT_COLOR = 'secondary';

export function mapIssueStatusColor(status: IssueStatus | null): string | '' {
    if (!status) return DEFAULT_COLOR;

    switch (status) {
        case IssueStatus.Pending:
            return 'danger';

        case IssueStatus.Deferred:
            return 'warning';

        case IssueStatus.InProgress:
            return 'info';

        case IssueStatus.NoActionRequired:
            return 'success';

        case IssueStatus.Resolved:
            return 'success';

        default:
            return DEFAULT_COLOR;
    }
}

export function mapRecallStatusColor(status: RecallStatus | null): string | '' {
    if (!status) return DEFAULT_COLOR;

    switch (status) {
        case RecallStatus.Pending:
            return 'danger';

        case RecallStatus.Completed:
            return 'success';

        default:
            return DEFAULT_COLOR;
    }
}

export function mapNoteStatusColor(note: string | null): string {
    if(!note) return DEFAULT_COLOR;

    return 'success';
}

export function mapAvvStatus(avvStatus: AvvStatus | null): string {
    if(!avvStatus) return DEFAULT_COLOR;

    return 'success';
}

export function mapEstReleaseStatusColor(estReleased: string | null): string {
    if(!estReleased) return DEFAULT_COLOR;

    return 'success';
}

export function mapCreStatusColor(status: CreStatus | null): string {
    if(!status) return DEFAULT_COLOR;

    switch (status) {
        case CreStatus.Pending:
            return 'danger';

        case CreStatus.Submitted:
            return 'success';

        default:
            return DEFAULT_COLOR;
    }
}

export function mapAvvStatusColor(status: AvvStatus | null): string | '' {
    if (!status) return DEFAULT_COLOR;

    return 'success';
}

export function mapCheckListProgressStatusColor(progressStatus: CheckListProgressStatus): string | '' {
    if (!progressStatus) return DEFAULT_COLOR;

    switch (progressStatus) {
        case CheckListProgressStatus.InProgress:
            return 'warning';

        case CheckListProgressStatus.Completed:
            return 'success';

        default:
            return DEFAULT_COLOR;
    }
}


