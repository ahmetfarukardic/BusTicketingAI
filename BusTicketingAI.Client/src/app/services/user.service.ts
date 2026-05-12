import { HttpClient, HttpEventType } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7202/api/users';

  getUsers(): Observable<any[]> {
    return this.http.get<any[]>(this.apiUrl);
  } 

  createUser(user: any): Observable<any> {
    return this.http.post(this.apiUrl, user);
  } 

  updateUser(user: any): Observable<any> {
    return this.http.put(this.apiUrl, user);
  }

  deleteUser(id: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }
}
