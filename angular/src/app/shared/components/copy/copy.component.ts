import { CommonModule } from '@angular/common';
import { booleanAttribute, Component, inject, Input } from '@angular/core';
import { CopyService } from '../../services/copy.service';

@Component({
  selector: 'app-copy',
  imports: [CommonModule],
  templateUrl: './copy.component.html',
  styleUrl: './copy.component.scss'
})
export class CopyComponent {
  @Input() text: string | undefined;
  @Input({ required: true }) value: string;
  @Input({ transform: booleanAttribute }) hideIcon: boolean = false;
  @Input() textPosition: 'start' | 'end' = 'end';
  @Input() textClass: string | undefined;

  private copyService = inject(CopyService);

  get _textClass() {
    return `${this.textClass || ''} mx-1`
  }

  copy() {
    if (!this.value) return;
    this.copyService.copy(this.value);
  }
}
