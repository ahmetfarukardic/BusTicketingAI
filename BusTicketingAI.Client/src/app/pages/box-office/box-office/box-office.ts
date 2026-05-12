import { CommonModule } from '@angular/common';
import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BoxOfficeService } from '../../../services/box-office.service';
import { AlertService } from '../../../services/alert.service';
import { CompanyActiveTrip, SellTicketRequest, Terminal } from '../../../core/models/box-office.model';
import { FlatpickrDirective } from '../../../directives/flatpickr.directive';
import { DialogModule } from 'primeng/dialog';

interface Seat {
  number: number;
  status: 'available' | 'occupied' | 'selected';
  gender?: 'E' | 'K';
}

@Component({
  selector: 'app-box-office',
  standalone: true,
  imports: [CommonModule, FormsModule, FlatpickrDirective, DialogModule],
  templateUrl: './box-office.html',
  styleUrl: './box-office.scss',
})

export class BoxOffice implements OnInit {
  private boxOfficeService = inject(BoxOfficeService);
  private alertService = inject(AlertService);

  terminals = signal<Terminal[]>([]);
  trips = signal<CompanyActiveTrip[]>([]);
  selectedTrip = signal<CompanyActiveTrip | null>(null);

  occupiedSeats = signal<{ seatNumber: number, gender: 'E' | 'K' }[]>([]);
  selectedSeat = signal<{ seatNumber: number, gender: 'E' | 'K' } | null>(null);

  busLayout = signal<string>('2+1');
  seats = signal<Seat[]>([]);

  filterOrigin = signal<number | null>(null);
  filterDest = signal<number | null>(null);
  filterDate = signal<string>('');
  
  sellForm = signal({ passengerName: '', passengerTC: '' });
  searchText = signal<string>('');

  isGenderModalOpen = signal<boolean>(false);
  restrictedGender = signal<'E'| 'K' | null>(null);
  pendingSeatSelection = signal<Seat | null>(null);

  ngOnInit(): void {
    this.loadTrips();
    this.loadTerminals();
  }

  loadTerminals() {
    this.boxOfficeService.getTerminals().subscribe({
      next: (data) => this.terminals.set(data),
      error: () => this.alertService.error('Hata', 'Terminaller yüklenemedi.')
    });
  }

  loadTrips() {
    this.boxOfficeService.getActiveTrips(this.filterOrigin(), this.filterDest(), this.filterDate()).subscribe({
      next: (data) => {
        this.trips.set(data);
        this.selectedTrip.set(null);
      },
      error: () => this.alertService.error('Hata!', 'Seferler yüklenemedi.')
    });
  }

  clearFilters() {
    this.filterOrigin.set(null);
    this.filterDest.set(null);
    this.filterDate.set('');
    this.loadTrips();
  }

  swapCities() {
    const origin = this.filterOrigin();
    const dest = this.filterDest();
    this.filterOrigin.set(dest);
    this.filterDest.set(origin);
  }

  selectTrip(trip: CompanyActiveTrip) {
    this.selectedTrip.set(trip);
    this.selectedSeat.set(null);
    this.sellForm.set({ passengerName: '', passengerTC: ''});
    this.busLayout.set(trip.totalSeats <= 30 ? '2+1' : '2+2');
    this.loadOccupiedSeats(trip.tripId);
  }

  loadOccupiedSeats(tripId: string) {
    this.boxOfficeService.getOccupiedSeats(tripId).subscribe({
      next: (data) => {
        this.occupiedSeats.set(data);
        this.generateBusSeats(this.selectedTrip()!.totalSeats);
      },
      error: () => this.alertService.error('Hata!', 'Koltuk durumu yüklenemedi.')
    });
  }

  generateBusSeats(capacity: number) {
    const currentOccupied = this.occupiedSeats();
    const generatedSeats: Seat[] = [];

    for (let i = 1; i <= capacity; i++) {
      const occupied = currentOccupied.find(s => s.seatNumber === i);
      generatedSeats.push({
        number: i,
        status: occupied ? 'occupied' : 'available',
        gender: occupied?.gender
      });
    }
    this.seats.set(generatedSeats);
  }

  toggleSeat(seat: Seat) {
    if (seat.status === 'occupied') {
      this.alertService.warning('Uyarı', 'Bu koltuk zaten dolu.');
      return;
    }

    if (this.selectedSeat()?.seatNumber === seat.number){
      this.selectedSeat.set(null);
      this.sellForm.set({ passengerName: '', passengerTC: '' });
      this.generateBusSeats(this.selectedTrip()!.totalSeats);
      return;
    }

    const neighborNumber = this.getNeighborSeatNumber(seat.number, this.busLayout());
    const neighborSeat = neighborNumber ? this.seats().find(s => s.number === neighborNumber) : null;

    if (neighborSeat && neighborSeat.status === 'occupied' && neighborSeat.gender) {
      this.restrictedGender.set(neighborSeat.gender === 'E' ? 'K' : 'E');
    } else {
      this.restrictedGender.set(null);
    }

    this.pendingSeatSelection.set(seat);
    this.isGenderModalOpen.set(true);
  }

  getNeighborSeatNumber(seatNumber: number, busLayout: string): number | null {
    if (busLayout === '2+2') {
      return seatNumber % 2 !== 0 ? seatNumber + 1 : seatNumber - 1;
    } else if (busLayout === '2+1') {
      const mod = seatNumber % 3;
      if (mod === 1) return null; // Tekli koltuk
      if (mod === 2) return seatNumber + 1;
      if (mod === 0) return seatNumber - 1;
    }
    return null;
  }

  confirmSeatGender(selectedGender: 'E' | 'K') {
    const seat = this.pendingSeatSelection();
    if (!seat) return;

    if (this.restrictedGender() === selectedGender) {
      this.alertService.error('Kural İhlali', 'Yan koltuktaki yolcu ile aynı cinsiyette olmalısınız!');
      return;
    }

    this.selectedSeat.set({ seatNumber: seat.number, gender: selectedGender });
    this.isGenderModalOpen.set(false);
    this.pendingSeatSelection.set(null);
    this.generateBusSeats(this.selectedTrip()!.totalSeats);
  }

  updateForm(field: 'passengerName' | 'passengerTC', value: string) {
    this.sellForm.update(currentForm => ({
      ...currentForm,
      [field]: value
    }));
  }

  sellTicket() {
    const trip = this.selectedTrip();
    const seat = this.selectedSeat();
    const form = this.sellForm();

    if (!trip || !seat || !form.passengerName || !form.passengerTC) {
      this.alertService.error('Uyarı', 'Lütfen tüm alanları doldurun.');
      return;
    }

    const payload: SellTicketRequest = {
      tripId: trip.tripId,
      seatNumber: seat.seatNumber,
      passengerName: form.passengerName,
      passengerTC: form.passengerTC,
      price: trip.price,
      gender: seat.gender
    };

    this.boxOfficeService.sellTicket(payload).subscribe({
      next: (res) => {
        this.alertService.success('Başarılı!', res.message);
        this.selectedSeat.set(null);
        this.sellForm.set({ passengerName: '', passengerTC: '' });
        this.loadOccupiedSeats(trip.tripId);
      },
      error: () => this.alertService.error('Hata!', 'Bilet satışı gerçekleştirilemedi.')
    });
  }
}
