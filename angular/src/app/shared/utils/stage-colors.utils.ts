import { AvvStatus } from "src/app/proxy/car-bays";
import { IssueStatus } from "src/app/proxy/issues";
import { RecallStatus } from "src/app/proxy/recalls";

const DEFAULT_COLOR = 'dark';

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

export function mapAvvStatusColor(status: AvvStatus | null): string | '' {
    if (!status) return DEFAULT_COLOR;

    return 'success';
}