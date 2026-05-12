import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7202/api/auth';

  currentUser = signal<any>(null);

  constructor() {
    this.checkToken();
  }

  login(credentials: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, credentials).pipe(
      tap((response: any) => {
        if (response && response.token) {
          localStorage.setItem('token', response.token);
          this.checkToken();
        }
      })
    );
  }

  register(userData: any) : Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, userData);
  }

  checkToken() {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decoded: any = jwtDecode(token);

        const userRole = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || decoded.role;

        this.currentUser.set({
          email: decoded.email || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
          role: userRole,
          name: decoded.name || decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
        });
      } catch (error) {
        console.error('Token Cozulemedi: ', error);
        this.logout();
      }
    }
  }

  hasRole(role: string): boolean {
    const user = this.currentUser();
    return user ? user.role === role : false;
  }

  logout() {
    localStorage.removeItem('token');
    this.currentUser.set(null);
  }

  forgotPassword(email: string | null) {
    return this.http.post(`${this.apiUrl}/forgot-password`, { email });
  }

  resetPassword(payload: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/reset-password`, payload);
  }
}