import { Injectable } from '@angular/core';
import { FileAttachmentDto } from 'src/app/proxy/entity-attachments/file-attachments';

export type IssueRef = {
  id?: string | null;
  srNo: number;
};

@Injectable({ providedIn: 'root' })
export class IssueFilesState {
  private readonly filesByKey = new Map<string, FileAttachmentDto[]>();

  set(issue: IssueRef, files: FileAttachmentDto[]): void {
    this.filesByKey.set(this.keyOf(issue), files ?? []);
  }

  get(issue: IssueRef): FileAttachmentDto[] {
    return this.filesByKey.get(this.keyOf(issue)) ?? [];
  }

  clear(issue: IssueRef): void {
    this.filesByKey.delete(this.keyOf(issue));
  }

  clearAll(): void {
    this.filesByKey.clear();
  }

  private keyOf(issue: IssueRef): string {
    return issue.id ? `id:${issue.id}` : `new:${issue.srNo}`;
  }
}