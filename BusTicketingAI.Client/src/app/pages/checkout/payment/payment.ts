import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ProfileService } from '../../../services/profile.service';
import { AlertService } from '../../../services/alert.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-payment',
  standalone: true,
  imports: [CommonModule, ButtonModule, InputTextModule, ReactiveFormsModule],
  templateUrl: './payment.html',
  styleUrl: './payment.scss',
})
export class Payment {
  @Input() orderData: any;
  @Input() totalPrice!: number;
  @Input() tripId!: string;

  @Output() paymentSuccess = new EventEmitter<void>();

  private formBuilder = inject(FormBuilder);
  private profileService = inject(ProfileService);
  private alertService = inject(AlertService);
  private router = inject(Router);

  isLoading = false;

  paymentForm: FormGroup = this.formBuilder.group({
    cardHolderName: ['', Validators.required],
    cardNumber: ['', [Validators.required, Validators.minLength(16)]],
    expirationDate: ['', Validators.required],
    cvv: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(3)]]
  });

  formatExpirationDate(event: any) {
    let inputChar = String.fromCharCode(event.keyCode);
    let value = event.target.value;

    value = value.replace(/\D/g, '');

    if (value.length > 2) {
      value = value.substring(0, 2) + '/' + value.substring(2, 4);
    }

    event.target.value = value;
    this.paymentForm.patchValue({ expirationDate: value }, { emitEvent: false });
  }

  submitOrder() {
    if (this.paymentForm.invalid) {
      this.paymentForm.markAllAsTouched();
      this.alertService.warning('Uyarı', 'Lütfen kart bilgilerini eksiksiz giriniz.');
      return;
    }

    this.isLoading = true;
    const cardData = this.paymentForm.value;

    const checkoutPayload = {
      tripId: this.tripId,
      contactEmail: this.orderData.contactEmail,
      contactPhone: this.orderData.contactPhone,
      passengers: this.orderData.passengers, // { seatNumber, fullName, tcIdentity } olarak gelecek, backend ile birebir aynı!
      cardHolderName: cardData.cardHolderName,
      cardNumber: cardData.cardNumber,
      expirationDate: cardData.expirationDate,
      cvv: cardData.cvv,
      totalAmount: this.totalPrice
    };

    this.profileService.checkoutTicket(checkoutPayload).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.paymentSuccess.emit();
        this.alertService.success('Mutlu Yolculuklar!', 'Ödemeniz alındı. Bilet detayları e-posta adresinize gönderildi.');
        this.router.navigate(['/profile']);
      },
      error: (err) => {
        this.isLoading = false;
        const errorMessage = err.error?.message || err.error?.detail || 'Ödeme işlemi reddedildi.';
        this.alertService.error('Ödeme Başarısız', errorMessage);
      }
    })
  }
}
