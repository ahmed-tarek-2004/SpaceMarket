import { Component } from '@angular/core';
import { ClientSignUpFormComponent } from '../../components/sign-up/client-sign-up-form/client-sign-up-form.component';
import { ProviderSignUpFormComponent } from '../../components/sign-up/provider-sign-up-form/provider-sign-up-form.component';

@Component({
  selector: 'app-sign-up-page',
  imports: [ClientSignUpFormComponent, ProviderSignUpFormComponent],
  templateUrl: './sign-up-page.component.html',
  styleUrls: ['./sign-up-page.component.scss'],
})
export class SignUpPageComponent {
  selectedRegisterTabIndex = 0;
  selectedRegisterTab: 'client' | 'provider' = 'client';

  selectTab(index: number) {
    this.selectedRegisterTabIndex = index;
    this.selectedRegisterTab = index === 0 ? 'client' : 'provider';
  }

  getTabClass(index: number): string {
    const baseClasses = 'whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm lg:text-base';

    if (this.selectedRegisterTabIndex === index) {
      return `${baseClasses} border-[#00A8FF] text-[#00A8FF]`;
    } else {
      return `${baseClasses} border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300`;
    }
  }
}
