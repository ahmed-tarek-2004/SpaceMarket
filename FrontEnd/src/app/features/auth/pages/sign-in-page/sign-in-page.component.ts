import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SignInFormComponent } from '../../components/sign-in/sign-in-form/sign-in-form.component';

@Component({
  selector: 'app-sign-in-page',
  standalone: true,
  imports: [CommonModule, SignInFormComponent],
  templateUrl: './sign-in-page.component.html',
  styleUrls: ['./sign-in-page.component.scss'],
})
export class SignInPageComponent {}
