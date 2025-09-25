import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { VerifyOtpFormComponent } from '../../components/verify-otp-form/verify-otp-form.component';

@Component({
  selector: 'app-verify-otp-page',
  standalone: true,
  imports: [
    CommonModule,
    VerifyOtpFormComponent
    ],
  templateUrl: './verify-otp-page.component.html',
  styleUrls: ['./verify-otp-page.component.scss']
})
export class VerifyOtpPageComponent {}