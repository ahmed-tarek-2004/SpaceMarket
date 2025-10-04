import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-eye-password',
  imports: [CommonModule],
  templateUrl: './eye-password.component.html',
  styleUrls: ['./eye-password.component.scss']
})
export class EyePasswordComponent {
  @Input() showPassword = false;
  @Output() showPasswordChange = new EventEmitter<boolean>();

  constructor () {}

  toggle(): void {
    this.showPassword = !this.showPassword;
    this.showPasswordChange.emit(this.showPassword);
  }
}
