import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
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
export class Payment implements OnChanges {
  @Input() orderData: any;
  @Input() totalPrice!: number;
  @Input() originalPrice!: number;
  @Input() tripId!: string;
  @Input() useWalletBalance = false;

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

  ngOnChanges(changes: SimpleChanges) {
    if (changes['totalPrice']) {
      if (this.totalPrice === 0) {
        this.paymentForm.clearValidators();
        Object.keys(this.paymentForm.controls).forEach(key => {
          this.paymentForm.get(key)?.clearValidators();
          this.paymentForm.get(key)?.updateValueAndValidity();
        });
      } else {
        this.paymentForm.get('cardHolderName')?.setValidators(Validators.required);
        this.paymentForm.get('cardNumber')?.setValidators([Validators.required, Validators.minLength(16)]);
        this.paymentForm.get('expirationDate')?.setValidators(Validators.required);
        this.paymentForm.get('cvv')?.setValidators([Validators.required, Validators.minLength(3), Validators.maxLength(3)]);

        Object.keys(this.paymentForm.controls).forEach(key => {
          this.paymentForm.get(key)?.updateValueAndValidity();
        });
      }
    }
  }

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
    if (this.totalPrice > 0 && this.paymentForm.invalid) {
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
      passengers: this.orderData.passengers,
      cardHolderName: cardData.cardHolderName || 'WALLET',
      cardNumber: cardData.cardNumber || '000000000000',
      expirationDate: cardData.expirationDate || '12/99',
      cvv: cardData.cvv || '000',
      totalAmount: this.originalPrice,
      useWalletBalance: this.useWalletBalance
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
