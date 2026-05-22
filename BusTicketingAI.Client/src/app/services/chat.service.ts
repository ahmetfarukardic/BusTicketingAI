import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private http = inject(HttpClient);
  private apiUrl = 'https://localhost:7202/api/chat';

  sendMessage(message: string): Observable<{reply: string}> {
    return this.http.post<{reply: string}>(this.apiUrl, { message });
  }
}
