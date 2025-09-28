import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ForgetPasswordFormComponent } from '../../components/sign-in/forget-password-form/forget-password-form.component';

@Component({
  selector: 'app-forget-password-page',
  standalone: true,
  imports: [CommonModule, ForgetPasswordFormComponent],
  templateUrl: './forget-password-page.component.html',
  styleUrls: ['./forget-password-page.component.scss']
})
export class ForgetPasswordPageComponent {}