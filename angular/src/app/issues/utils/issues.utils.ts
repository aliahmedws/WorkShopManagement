import { IssueStatus } from "src/app/proxy/issues";

export function mapIssueStatusBgColor(status: IssueStatus | null): string | '' {
    if (!status) return '';

    switch (status) {
        case IssueStatus.Pending:
            return 'bg-danger';

        case IssueStatus.Deferred:
            return 'bg-info';

        case IssueStatus.InProgress:
            return 'bg-warning';

        case IssueStatus.NoActionRequired:
            return 'bg-success';

        case IssueStatus.Resolved:
            return 'bg-success';

        default:
            return '';
    }
}