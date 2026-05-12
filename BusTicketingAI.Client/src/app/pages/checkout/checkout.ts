import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { AuthService } from '../../services/auth.service';
import { ProfileService } from '../../services/profile.service';
import { Payment } from './payment/payment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonModule, InputTextModule, CheckboxModule, DialogModule, Payment, FormsModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss',
})
export class Checkout implements OnInit, OnDestroy {
  private formBuilder = inject(FormBuilder);
  private authService = inject(AuthService);
  private profileService = inject(ProfileService);
  private router = inject(Router);

  tripId = signal<string>('');
  selectedSeats = signal<{ seatNumber: number, gender: 'E' | 'K' }[]>([]);
  totalPrice = signal<number>(0);
  isPaymentSuccessful = false;

  //page statement(1: passengers info, 2: payment)
  currentStep = signal<number>(1);

  isTermsModalOpen = signal<boolean>(false);
  termsConfirmed = signal<boolean>(false);

  checkoutForm!: FormGroup;

  constructor() {
    const navigation = this.router.getCurrentNavigation();
    const state = navigation?.extras.state as { tripId: string, seats: {seatNumber: number, gender: 'E' | 'K' }[], price: number };

    if (state) {
      this.tripId.set(state.tripId);
      this.selectedSeats.set(state.seats);
      this.totalPrice.set(state.price);
    } else {
      this.router.navigate(['/trips']);
    }
  }

  ngOnDestroy() {
    if (!this.isPaymentSuccessful && this.tripId() && this.selectedSeats().length > 0) {
      const payload = {
        tripId: this.tripId(),
        seats: this.selectedSeats()
      };

      this.profileService.unlockSeats(payload).subscribe();
    }
  }

  ngOnInit() {
    this.initForm();
    this.loadUserDataAndBuildForms();    
  }

  initForm() {
    this.checkoutForm = this.formBuilder.group({
      contactEmail: ['', [Validators.required, Validators.email]],
      contactPhone: ['', Validators.required],
      passengers: this.formBuilder.array([])
    });

    const user = this.authService.currentUser();
    if (user && user.email) {
      this.checkoutForm.patchValue({ contactEmail: user.email });
    }
  }

  get passengersFormArray() {
    return this.checkoutForm.get('passengers') as FormArray;
  }

  loadUserDataAndBuildForms() {
    this.profileService.getMyProfile().subscribe({
      next: (profile) => {
        this.checkoutForm.patchValue({
          contactEmail: profile.email,
          contactPhone: profile.phoneNumber || ''
        });
        this.buildPassengerForms(profile);
      },
      error: () => {
        this.buildPassengerForms(null);
      }
    });
  }

  buildPassengerForms(userProfile: any) {
    const seats = this.selectedSeats();

    seats.forEach((seat, index) => {
      const passengerGroup = this.formBuilder.group({
        seatNumber: [seat.seatNumber],
        gender: [seat.gender],
        fullName: ['', Validators.required],
        tcIdentity: ['', [Validators.required, Validators.minLength(11), Validators.maxLength(11)]]
      });

      if (index === 0 && userProfile){
        passengerGroup.patchValue({
          fullName: `${userProfile.firstName} ${userProfile.lastName}`.trim()
        });
      }
      
      this.passengersFormArray.push(passengerGroup);
    });
  }

  openTermsModal() {
    this.isTermsModalOpen.set(true);
  }

  acceptTerms() {
    this.termsConfirmed.set(true);
    this.isTermsModalOpen.set(false);
  }

  goToPayment() {
    if (this.checkoutForm.invalid || !this.termsConfirmed()) {
      this.checkoutForm.markAllAsTouched();
      return;
    }
    this.currentStep.set(2);
  }
}