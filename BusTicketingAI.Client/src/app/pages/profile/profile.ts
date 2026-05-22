import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnInit, signal } from '@angular/core';
import { TabsModule } from 'primeng/tabs';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ButtonModule } from 'primeng/button';
import { ProfileService } from '../../services/profile.service';
import { AlertService } from '../../services/alert.service';
import { TicketDetailDto } from '../../core/models/ticket-model';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { first, identity } from 'rxjs';

@Component({
  selector: 'app-profile',
  imports: [CommonModule, DatePipe, CurrencyPipe, TabsModule, TableModule, TagModule, ButtonModule, ReactiveFormsModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss',
})
export class Profile implements OnInit{
  private profileService = inject(ProfileService);
  private alertService = inject(AlertService);
  private cdr = inject(ChangeDetectorRef);
  private formBuilder = inject(FormBuilder);

  profileForm!: FormGroup;
  isSaving = signal(false);

  activeTickets = signal<TicketDetailDto[]>([]);
  pastTickets = signal<TicketDetailDto[]>([]);

  walletBalance = signal<number>(0);
  walletTransactions = signal<any[]>([]);

  ngOnInit() {
    this.initForm();
    this.loadMyProfile();
    this.loadMyTickets();
    this.loadWalletData();
  }

  initForm() {
    this.profileForm = this.formBuilder.group({
      firstName: [{ value: '', disabled: true }], // İsim, soyisim ve mail değiştirilemez
      lastName: [{ value: '', disabled: true }],
      email: [{ value: '', disabled: true }],
      identityNumber: ['', [Validators.required, Validators.minLength(11), Validators.maxLength(11), Validators.pattern('^[0-9]*$')]],
      phoneNumber: ['', [Validators.required]],
      gender: ['', [Validators.required]]
    });
  }

  loadMyProfile() {
    this.profileService.getMyProfile().subscribe({
      next: (res) => {
        this.profileForm.patchValue({
          firstName: res.firstName,
          lastName: res.lastName,
          email: res.email,
          identityNumber: res.identityNumber,
          phoneNumber: res.phoneNumber,
          gender: res.gender
        });
      },
      error: (err) => {
        this.alertService.error('Hata', "Profil bilgileri alınamadı.");
      }
    });
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

  saveProfile() {
    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      this.alertService.error('Uyarı', 'Lütfen tüm alanları doğru formatta doldurun.');
      return;
    }

    this.isSaving.set(true);

    const payload = {
      identityNumber: this.profileForm.get('identityNumber')?.value,
      phoneNumber: this.profileForm.get('phoneNumber')?.value,
      gender: this.profileForm.get('gender')?.value
    };

    this.profileService.updateMyProfile(payload).subscribe({
      next: (res) => {
        this.alertService.success('Başarılı', 'Kişisel bilgileriniz kaydedildi.');
        this.isSaving.set(false);
      },
      error: (err) => {
        this.alertService.error('Hata', err.error?.message || 'Güncelleme başarısız oldu.');
        this.isSaving.set(false);
      }
    });
  }
}