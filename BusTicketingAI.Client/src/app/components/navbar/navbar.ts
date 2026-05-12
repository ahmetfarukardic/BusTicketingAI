import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonModule],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss',
})
export class Navbar {
  authService = inject(AuthService);
  private router = inject(Router);

  isLoggedIn() : boolean {
    return typeof window !== 'undefined' && localStorage.getItem('token') !== null;
  }

  getUserName(): string {
    if (typeof window === 'undefined') return ''; 
    
    const token = localStorage.getItem('token');
    if (!token) return '';

    try {
      const payload = token.split('.')[1];
      
      const decodedJson = decodeURIComponent(atob(payload).split('').map(function(c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
      }).join(''));
      
      const decoded = JSON.parse(decodedJson);
      
      return decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname'] 
          || decoded.given_name 
          || decoded.name 
          || 'Yolcu';
    } catch (e) {
      return 'Yolcu';
    }
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['/trips']);
  }
}