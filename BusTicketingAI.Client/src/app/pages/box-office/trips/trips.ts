import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BoxOfficeService } from '../../../services/box-office.service';
import { AlertService } from '../../../services/alert.service';
import { TripManagementService } from '../../../services/trip-management.service';
import { CompanyActiveTrip, Terminal } from '../../../core/models/box-office.model';
import { Bus, CreateTripRequest } from '../../../core/models/bus';
import { Passenger } from '../../../core/models/passenger';
import { FlatpickrDirective } from '../../../directives/flatpickr.directive';

@Component({
  selector: 'app-trips',
  standalone: true,
  imports: [CommonModule, FormsModule, FlatpickrDirective],
  templateUrl: './trips.html',
  styleUrl: './trips.scss',
})
export class Trips implements OnInit {
  private boxOfficeService = inject(BoxOfficeService);
  private alertService = inject(AlertService);
  private tripManagementService = inject(TripManagementService);

  terminals = signal<Terminal[]>([]);
  buses = signal<Bus[]>([]);
  trips = signal<CompanyActiveTrip[]>([]);

  newTripForm = signal<Partial<CreateTripRequest>>({
    busId: undefined,
    originTerminalId: undefined,
    destinationTerminalId: undefined,
    departureTime: '',
    basePrice: undefined,
    estimatedDuration: undefined
  });

  selectedTripForUpdate = signal<CompanyActiveTrip | null>(null);
  newTimeForUpdate = signal<string>('');

  selectedTripForManifesto = signal<CompanyActiveTrip | null>(null);
  tripPassengers = signal<Passenger[]>([]);

  ngOnInit(): void {
    this.loadInitialData();
  }

  loadInitialData() {
    this.boxOfficeService.getTerminals().subscribe({
      next: (data) => this.terminals.set(data)
    });

    this.tripManagementService.getCompanyBuses().subscribe({
      next: (data) => this.buses.set(data)
    });

    this.loadTrips();
  }

  loadTrips() {
    this.boxOfficeService.getActiveTrips(null, null, null).subscribe({
      next: (data) => this.trips.set(data)
    });
  }

  submitNewTrip() {
    const payload = this.newTripForm() as CreateTripRequest;

    if (!payload.busId || !payload.originTerminalId || !payload.destinationTerminalId || !payload.departureTime || !payload.basePrice || !payload.estimatedDuration) {
      this.alertService.error('Hata', 'Lütfen tüm alanları doldurun.');
      return;
    }

    if (payload.originTerminalId === payload.destinationTerminalId) {
      this.alertService.error('Hata', 'Kalkış ve Varış noktası aynı olamaz.');
      return;
    }

    this.tripManagementService.createTrip(payload).subscribe({
      next: (res) => {
        this.alertService.success('Başarılı', res.message);
        this.loadTrips();
        this.resetForm();
      },
      error: (err) => {
        this.alertService.error('Hata', err.error.message || 'Sefer oluşturulamadı.');
      }
    });
  }

  resetForm() {
    this.newTripForm.set({
      busId: undefined,
      originTerminalId: undefined,
      destinationTerminalId: undefined,
      departureTime: '',
      basePrice: undefined,
      estimatedDuration: undefined
    });
  }

  updateNewTripForm(key: string, value: any) {
    this.newTripForm.update(prev => ({ ...prev, [key]: value }));
  }

  openUpdateModal(trip: CompanyActiveTrip) {
    this.selectedTripForUpdate.set(trip);
    const formattedDate = new Date(trip.departureTime).toISOString().slice(0,16);
    this.newTimeForUpdate.set(formattedDate);
  }

  closeUpdateModal() {
    this.selectedTripForUpdate.set(null);
    this.newTimeForUpdate.set('');
  }

  submitTimeUpdate() {
    const trip = this.selectedTripForUpdate();
    if (!trip || !this.newTimeForUpdate()) return;

    this.tripManagementService.updateTripTime(trip.tripId, { newDepartureTime: this.newTimeForUpdate() })
      .subscribe({
        next: (res) => {
          this.alertService.success('Başarılı', res.message);
          this.loadTrips();
          this.closeUpdateModal();
        },
        error: (err) => {
          this.alertService.error('Uyarı', err.error?.message || 'Saat güncellenemedi.');
        }
      })
  }

  openManifestoModal(trip: CompanyActiveTrip) {
    this.selectedTripForManifesto.set(trip);
    this.tripManagementService.getTripPassengers(trip.tripId).subscribe({
      next: (data) => this.tripPassengers.set(data),
      error: (err) => this.alertService.error('Hata', 'Yolcu listesi alınamadı.')
    });
  }

  closeManifestoModal() {
    this.selectedTripForManifesto.set(null);
    this.tripPassengers.set([]);
  }

  cancelPassengerTicket(passenger: Passenger) {
    this.alertService.confirm(
      'Bilet İptali', 
      `${passenger.seatNumber} numaralı koltuğun biletini iptal etmek istediğinize emin misiniz?`, 
      () => {
        this.tripManagementService.cancelTicket(passenger.ticketId).subscribe({
          next: (res) => {
            this.alertService.success('İptal Edildi', res.message);
            
            if (this.selectedTripForManifesto()) {
              this.openManifestoModal(this.selectedTripForManifesto()!);
            }
          },
          error: (err) => {
            this.alertService.error('Hata', err.error?.message || 'Bilet iptal edilemedi.');
          }
        });
      }
    );
  }
}