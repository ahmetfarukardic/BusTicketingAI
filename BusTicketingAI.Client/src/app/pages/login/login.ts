import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AlertService } from '../../services/alert.service';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, DialogModule, InputTextModule],
  templateUrl: './login.html',
  styleUrl: './login.scss',
})
export class Login {
  loginForm: FormGroup;
  errorMessage: string = '';
  
  isResetModalOpen: boolean = false;
  resetEmailControl = new FormControl('', [Validators.required, Validators.email]);

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private alertService = inject(AlertService);

  constructor() {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.authService.login(this.loginForm.value).subscribe({
        next: (response) =>{
          this.errorMessage = '';

          const returnUrl = this.route.snapshot.queryParams['returnUrl'];
          if (returnUrl) {
            this.router.navigateByUrl(returnUrl);
          }
          else {
            if (this.authService.hasRole('Admin')) {
              this.router.navigate(['/admin']);
            }
            else if (this.authService.hasRole('CompanyStaff')) {
              this.router.navigate(['/company']);
            }
            else {
              this.router.navigate(['/trips']);
            }
          }
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = 'Email veya şifre hatalı! Lütfen tekrar deneyin.';
        }
      });
    }
  }

  openResetModal() {
    this.resetEmailControl.reset();
    this.isResetModalOpen = true;
  }

  submitPasswordReset() {
    if (this.resetEmailControl.invalid) return;

    const email = this.resetEmailControl.value;

    this.authService.forgotPassword(email).subscribe({
      next: (res) => {
        this.isResetModalOpen = false;
        this.alertService.success('Başarılı', 'Şifre sıfırlama bağlantısı e-posta adresinize gönderildi.');
      },
      error: (err) => {
        this.alertService.error('Hata', 'Kayıtlı e-posta adresi bulunamadı.');
      }
    });
  }
}
