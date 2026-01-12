import { BehaviorSubject, Subject } from "rxjs";
import { CarDto } from "src/app/proxy/cars";
import { IssueDto } from "src/app/proxy/issues";

export class IssueStateService {
    private carSubject = new BehaviorSubject<CarDto | null>(null);
    readonly car$ = this.carSubject.asObservable();

    private issuesSubject = new BehaviorSubject<IssueDto[]>([]);
    readonly issues$ = this.issuesSubject.asObservable();

    private issueSubject = new BehaviorSubject<IssueDto | null>(null);
    readonly issue$ = this.issueSubject.asObservable();

    private refreshRequestedSubject = new Subject<void>();
    readonly refreshRequested$ = this.refreshRequestedSubject.asObservable();

    setCar(car: CarDto | null) { this.carSubject.next(car); }
    setIssues(issues: IssueDto[] | null) { this.issuesSubject.next(issues ?? []); }
    setIssue(issue: IssueDto | null) { this.issueSubject.next(issue); }

    requestRefresh() { this.refreshRequestedSubject.next(); }
}