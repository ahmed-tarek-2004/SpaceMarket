import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  imports: [CommonModule],
  templateUrl: './button.component.html',
  styleUrl: './button.component.scss'
})
export class ButtonComponent {
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() label: string = 'Button';
  @Input() color: string = 'bg-blue-500';
  @Input() hoverColor: string = 'hover:bg-blue-600';
  @Input() textColor: string = 'text-white';
  @Input() disabled: boolean = false;
  @Input() fullWidth: boolean = true;
  @Input() loading: boolean = false;
  @Input() loadingText: string = 'Loading...';
  @Input() additionalClasses: string = '';

  get buttonClasses(): string {
    const baseClasses =
      'inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-colors duration-200 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50';

    const widthClass = this.fullWidth ? 'w-full' : '';
    const sizeClass = 'h-10 px-4';

    const stateClasses =
      this.disabled || this.loading
        ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
        : `${this.color} ${this.hoverColor} ${this.textColor} cursor-pointer`;

    const allClasses = [baseClasses, widthClass, sizeClass, stateClasses, this.additionalClasses]
      .filter(Boolean)
      .join(' ')
      .replace(/\s+/g, ' ');

    return allClasses;
  }
}
