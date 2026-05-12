import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './reset-password.html',
  styleUrl: './reset-password.scss',
})
export class ResetPassword implements OnInit {
  resetForm: FormGroup;
  token: string = '';
  email: string = '';
  isLoading = false;

  private formBuilder = inject(FormBuilder);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);
  private alertService = inject(AlertService);

  constructor() {
    this.resetForm = this.formBuilder.group({
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.token = params['token'];
      this.email = params['email'];

      if (!this.token || !this.email) {
        this.alertService.error('Hata', 'Geçersiz veya süresi dolmuş bir bağlantıya tıkladınız.');
        this.router.navigate(['/login']);
      }
    });
  }

  passwordMatchValidator(control: AbstractControl) {
    const password = control.get('newPassword')?.value;
    const confirmPassword = control.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true};
  }

  onSubmit() {
    if (this.resetForm.invalid) return;

    this.isLoading = true;
    const payload = {
      email: this.email,
      token: this.token,
      newPassword: this.resetForm.value.newPassword
    };

    this.authService.resetPassword(payload).subscribe({
      next: (res) => {
        this.isLoading = false;
        this.alertService.success('Başarılı', res.message || 'Şifreniz başarıyla değiştirildi!');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.isLoading = false;
        this.alertService.error('Hata', err.error?.message || 'Şifre sıfırlanırken bir hata oluştu.');
      }
    });
  }
}