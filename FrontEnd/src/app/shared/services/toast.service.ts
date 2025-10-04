import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ToastMessage } from '../interfaces/toaster-message';

@Injectable({
  providedIn: 'root',
})
export class ToastService {
  private _messages = new BehaviorSubject<ToastMessage | null>(null);
  messages$ = this._messages.asObservable();

  success(message: string, duration = 6000) {
    this.show(message, 'success', duration);
  }

  error(message: string, duration = 8000) {
    this.show(message, 'error', duration);
  }

  info(message: string, duration = 6000) {
    this.show(message, 'info', duration);
  }

  warning(message: string, duration = 6000) {
    this.show(message, 'warning', duration);
  }

  private show(message: string, type: ToastMessage['type'], duration: number) {
    this._messages.next({ type, message, duration });
  }
}
