import { Component, Input } from '@angular/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  standalone: true,
  selector: 'c-loading-bar',
  imports: [MatProgressBarModule],
  template: `@if (visivel) {<mat-progress-bar color="primary" mode="indeterminate"> </mat-progress-bar>}  `,
  styles: ``
})
export class LoadingBarComponent {
  @Input() visivel: boolean = true;
}
