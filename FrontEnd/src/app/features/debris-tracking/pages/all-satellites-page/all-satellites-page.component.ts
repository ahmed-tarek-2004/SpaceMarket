import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AllSatellitesComponent } from '../../components/all-satellites/all-satellites.component';
import { SatelliteFormComponent } from '../../components/satellite-form/satellite-form.component';

@Component({
  selector: 'app-all-satellites-page',
  standalone: true,
  imports: [CommonModule, AllSatellitesComponent, SatelliteFormComponent],
  templateUrl: './all-satellites-page.component.html',
  styleUrls: ['./all-satellites-page.component.scss'],
})
export class AllSatellitesPageComponent {
  showForm = signal(false);
  editMode = signal(false);
  editData = signal<{ id: string; name: string; noradId: string } | null>(null);

  onSatelliteSelected(data: { id: string; name: string; noradId: string }): void {
    this.editData.set(data);
    this.editMode.set(true);
    this.showForm.set(true);
  }

  onFormSubmit(): void {
    this.showForm.set(false);
    this.editMode.set(false);
    this.editData.set(null);
  }

  onFormCancel(): void {
    this.showForm.set(false);
    this.editMode.set(false);
    this.editData.set(null);
  }
}
