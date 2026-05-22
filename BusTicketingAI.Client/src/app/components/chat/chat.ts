import { CommonModule } from '@angular/common';
import { AfterViewChecked, Component, ElementRef, inject, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ChatService } from '../../services/chat.service';
import { Router } from '@angular/router';
import { trigger } from '@angular/animations';
import { generate } from 'rxjs';

interface ChatMessage {
  text: string;
  isBot: boolean;
}

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  styleUrl: './chat.scss',
})
export class Chat implements AfterViewChecked{
  private chatService = inject(ChatService);
  private router = inject(Router);

  @ViewChild('scrollMe') private myScrollContainer!: ElementRef;

  isOpen = signal(false);
  isLoading = signal(false);
  userInput = signal('');
  messages = signal<ChatMessage[]>([
    { text: 'Merhaba! Ben BusTicketingAI asistanınızım. Size nasıl yardımcı olabilirim? (Örn: "Bugün Ankara\'dan Bursa\'ya bilet var mı?")', isBot: true }
  ]);

  toggleChat() {
    this.isOpen.set(!this.isOpen());
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  scrollToBottom(): void {
    try {
      this.myScrollContainer.nativeElement.scrollTop = this.myScrollContainer.nativeElement.scrollHeight;
    }
    catch(err) { }
  }

  sendMessage() {
    const text = this.userInput().trim();
    if (!text) return;

    this.messages.update(m => [...m, { text: text, isBot: false }]);
    this.userInput.set('');
    this.isLoading.set(true);

    this.chatService.sendMessage(text).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        this.handleBotReply(res.reply);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.messages.update(m => [...m, { text: 'Bağlantı hatası oluştu. Lütfen tekrar deneyin.', isBot: true }]);
      }
    });
  }

  private handleBotReply(reply: string) {
    const cleanReply = reply.replace(/```json/g, '').replace(/```/g, '').trim();

    try {
      const parsedData = JSON.parse(cleanReply);

      if (parsedData.action === 'NAVIGATE_TO_CHECKOUT') {
        this.messages.update(m => [...m, {
          text: "Harika! Seçiminizi aldım. Sizi ödeme ekranına yönlendiriyorum...",
          isBot: true
        }]);

        setTimeout(() => {
          this.router.navigate(['/trip', parsedData.tripId, 'seats']);
          this.isOpen.set(false);
        }, 1500);
        return; 
      }

      if (parsedData.action === 'NAVIGATE_TO_PAYMENT') {
        this.messages.update(m => [...m, {
          text: 'Harika haber! Profil bilgileriniz eksiksiz olduğu için koltuğunuzu doğrudan ayırdım. Güvenli ödeme sayfasına geçiyoruz...',
          isBot: true
        }]);

        setTimeout(() => {
          this.router.navigate(['/checkout'], {
            state: {
              tripId: parsedData.tripId,
              seats: [{ seatNumber: parsedData.seatNumber, gender: parsedData.gender === 'Erkek' ? 'E' : 'K' }],
              price: parsedData.price
            }
          });
          this.isOpen.set(false);
        }, 1500);
        return;
      }
    } catch (e) { 

    }
    this.messages.update(m => [...m, { text: reply, isBot: true}]);
  }
}