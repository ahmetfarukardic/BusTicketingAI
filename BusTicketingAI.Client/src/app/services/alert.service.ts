import { inject, Injectable } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
//import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private messageService = inject(MessageService);
  private confirmationService = inject(ConfirmationService);

  success(title: string, message: string) {
    this.messageService.add({ severity: 'success', summary: title, detail: message, life: 3000 });
  }

  error(title: string, message: string) {
    this.messageService.add({ severity: 'error', summary: title, detail: message, life: 4000 });
  }

  warning(title: string, message: string) {
    this.messageService.add({ severity: 'warn', summary: title, detail: message, life: 3500 });
  }

  info(title: string, message: string) {
    this.messageService.add({ severity: 'info', summary: title, detail: message, life: 3000 });
  }

  confirm(header: string, message: string, acceptCallback: () => void) {
    this.confirmationService.confirm({
      header: header,
      message: message,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Evet',
      rejectLabel: 'İptal',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => {
        acceptCallback();
      }
    });
  }
}