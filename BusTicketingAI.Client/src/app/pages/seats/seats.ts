import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, Signal, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertService } from '../../services/alert.service';
import { TripService } from '../../services/trip.service';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { ProfileService } from '../../services/profile.service';

interface OccupiedSeatDto {
  seatNumber: number;
  gender: 'E' | 'K';
}

interface Seat {
  number: number;
  status: 'available' | 'occupied' | 'selected';
  gender?: 'E' | 'K';
}

@Component({
  selector: 'app-seats',
  standalone: true,
  imports: [CommonModule, ButtonModule, DialogModule],
  templateUrl: './seats.html',
  styleUrl: './seats.scss',
})
export class Seats implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private alertService = inject(AlertService);
  private tripService = inject(TripService);
  private profileService = inject(ProfileService);

  tripId: string | null = null;
  seats: Seat[] = [];
  selectedSeats = signal<{ seatNumber: number, gender: 'E' | 'K' }[]>([]);
  seatPrice = 0;
  busLayout: string = '2+1';
  isLocking = signal<boolean>(false);

  restrictedGender = signal<'E' | 'K' | null>(null);
  isGenderModalOpen = signal<boolean>(false);
  pendingSeatSelection: Seat | null = null;

  ngOnInit() {
    this.tripId = this.route.snapshot.paramMap.get('id');
    if (this.tripId) {
      this.loadTripData(this.tripId);
    }
  }

  loadTripData(tripId: string) {
    this.tripService.getTripDetails(tripId).subscribe({
      next: (data: any) => {
        this.seatPrice = data.price;
        this.busLayout = data.seatCapacity === 30 ? '2+1' : '2+2'
        this.generateBusSeats(data.seatCapacity, data.occupiedSeats);
      },
      error: (err) => {
        this.alertService.error('Hata', 'Sefer detayları alınamadı.');
      }
    });
  }

  generateBusSeats(capacity: number, occupiedSeats: OccupiedSeatDto[]) {
    this.seats = [];
    for (let i = 1; i <= capacity; i++) {
      const occupiedSeat = occupiedSeats.find(s => s.seatNumber === i);
      this.seats.push({
        number: i,
        status: occupiedSeat ? 'occupied' : 'available',
        gender: occupiedSeat?.gender
      });
    }
  }

  toggleSeat(seat: Seat) {
    if (seat.status === 'occupied') {
      this.alertService.warning('Uyarı', 'Bu koltuk dolu.');
      return;
    }

    if (seat.status === 'selected') {
      seat.status = 'available';
      seat.gender = undefined;
      const currentSelected = this.selectedSeats().filter(s => s.seatNumber !== seat.number);
      this.selectedSeats.set(currentSelected);
    } else {
      if (this.selectedSeats().length >= 4) {
        this.alertService.warning('Limit Aşıldı', 'Tek seferde en fazla 4 koltuk seçebilirsiniz!');
        return;
      }

      const neighborNumber = this.getNeighborSeatNumber(seat.number, this.busLayout);
      const neighborSeat = neighborNumber ? this.seats.find(s => s.number === neighborNumber) : null;
      
      if (neighborSeat && neighborSeat.status === 'occupied' && neighborSeat.gender) {
        this.restrictedGender.set(neighborSeat.gender === 'E' ? 'K' : 'E');
      } else {
        this.restrictedGender.set(null); 
      }

      this.pendingSeatSelection = seat;
      this.isGenderModalOpen.set(true);
    }
  }

  confirmSeatGender(selectedGender: 'E' | 'K') {
    if (!this.pendingSeatSelection) return;
    if (this.restrictedGender() === selectedGender) {
      this.alertService.error('Kural İhlali', 'Bu koltuğa bu cinsiyet seçilemez!');
      return;
    }
    
    const seat = this.pendingSeatSelection;
    const neighborNumber = this.getNeighborSeatNumber(seat.number, this.busLayout);
    const neighborSeat = neighborNumber ? this.seats.find(s => s.number === neighborNumber) : null;

    if (neighborSeat && (neighborSeat.status === 'occupied')) {
      if (neighborSeat.gender && neighborSeat.gender !== selectedGender) {
        this.alertService.error('Kural İhlali', 'Başka bir yolcunun yanına farklı cinsiyette bilet alamazsınız.');
        this.isGenderModalOpen.set(false);
        this.pendingSeatSelection = null;
        return;
      }
    }

    seat.status = 'selected';
    seat.gender = selectedGender;

    this.selectedSeats.update(prev => [...prev, { seatNumber: seat.number, gender: selectedGender }]);
    this.isGenderModalOpen.set(false);
    this.pendingSeatSelection = null;
  }

  get totalPrice() {
    return this.selectedSeats().length * this.seatPrice;
  }

  proceedToCheckout() {
    if (this.selectedSeats().length === 0) {
      this.alertService.warning('Eksik Seçim', 'Lütfen devam etmek için en az bir koltuk seçin.');
      return;
    }

    if (this.isLocking()) return;
    this.isLocking.set(true);

    const payload = {
      tripId: this.tripId,
      seats: this.selectedSeats()
    };

    this.profileService.lockSeats(payload).subscribe({
      next: (res: any) => {
        this.isLocking.set(false);
        this.router.navigate(['/checkout'], {
          state: {
            tripId: this.tripId,
            seats: this.selectedSeats(),
            price: this.totalPrice
          }
        });
      },
      error: (err) => {
        this.isLocking.set(false);
        this.alertService.error('Üzgünüz', 'Seçtiğiniz koltuklar saniyeler önce başka bir kullanıcı tarafından rezerve edildi.');
        this.selectedSeats.set([]);
        if (this.tripId) this.loadTripData(this.tripId);
      }
    });
  }

  getNeighborSeatNumber(seatNumber: number, busLayout: string): number | null {
    if (busLayout === '2+2') {
      const isOdd = seatNumber % 2 !== 0;
      return isOdd ? seatNumber + 1 : seatNumber - 1;
    } 
    else if (busLayout === '2+1') {
      const mod = seatNumber % 3;

      if (mod === 1) return null;
      if (mod === 2) return seatNumber + 1;
      if (mod === 0) return seatNumber - 1;
    }

    return null;
  }
}