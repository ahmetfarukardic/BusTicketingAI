import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnInit, signal } from '@angular/core';
import { TabsModule } from 'primeng/tabs';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { ProfileService } from '../../services/profile.service';
import { AlertService } from '../../services/alert.service';
import { TicketDetailDto } from '../../core/models/ticket-model';

@Component({
  selector: 'app-profile',
  imports: [CommonModule, DatePipe, CurrencyPipe, TabsModule, TableModule, TagModule, ButtonModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class Profile implements OnInit{
  private profileService = inject(ProfileService);
  private alertService = inject(AlertService);
  private cdr = inject(ChangeDetectorRef);

  activeTickets = signal<TicketDetailDto[]>([]);
  pastTickets = signal<TicketDetailDto[]>([]);

  walletBalance = signal<number>(0);
  walletTransactions = signal<any[]>([]);

  ngOnInit() {
    this.loadMyTickets();
    this.loadWalletData();
  }

  loadWalletData() {
    this.profileService.getWalletBalance().subscribe(res => this.walletBalance.set(res));
    this.profileService.getWalletTransactions().subscribe(res => this.walletTransactions.set(res));
  }

  loadMyTickets() {
    this.profileService.getMyTickets().subscribe({
      next: (res) => {
        this.activeTickets.set([...res.activeTickets]);
        this.pastTickets.set([...res.pastTickets]);
        setTimeout(() => {
          this.cdr.detectChanges();
        }, 50);
      },
      error: (err) => {
        this.alertService.error('Hata', 'Biletleriniz yüklenirken bir sorun oluştu.');
      }
    });
  }

  cancelTicket(ticket: TicketDetailDto) {
    this.alertService.confirm(
      'Bilet İptali',
      `${ticket.departureTerminal} - ${ticket.arrivalTerminal} seferine ait ${ticket.seatNumber} numaralı koltuğunuzu iptal etmek istediğinize emin misiniz?`,
      () => {
        this.profileService.cancelMyTicket(ticket.ticketId).subscribe({
          next: (res) => {
            this.alertService.success('Başarılı', 'Biletiniz başarıyla iptal edildi.');
            this.loadMyTickets();
            this.loadWalletData();
          },
          error: (err) => {
            const errorMessage = err.error?.message || err.error?.detail || 'Bilet iptal edilirken bir hata oluştu.';
            this.alertService.error('İptal Başarısız', errorMessage);
          }
        });
      }
    );
  }

  getSeverity(status: number): "success" | "secondary" | "info" | "warn" | "danger" | "contrast" {
    switch (status) {
      case 1: return 'success';
      case 0: return 'danger';
      default: return 'info';
    }
  }

  getStatusText(status: number): string {
    switch (status) {
      case 1: return 'Aktif';
      case 0: return 'İptal Edildi';
      default: return 'Geçmiş Sefer';
    }
  }
}