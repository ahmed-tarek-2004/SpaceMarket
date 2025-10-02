import { Component, Input } from '@angular/core';
import { Service } from '../../interfaces/service.interface';

@Component({
  selector: 'app-service-header',
  standalone: true,
  templateUrl: './service-header.component.html',
  styleUrls: ['./service-header.component.scss']
})
export class ServiceHeaderComponent {
  @Input() service!: Service; 
}