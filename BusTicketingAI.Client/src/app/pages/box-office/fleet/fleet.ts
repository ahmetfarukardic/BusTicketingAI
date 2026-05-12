import { CommonModule } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TripManagementService } from '../../../services/trip-management.service';
import { AlertService } from '../../../services/alert.service';
import { Bus } from '../../../core/models/bus';
import { CreateBusRequest } from '../../../core/models/create-bus-request';
import { dateTimestampProvider } from 'rxjs/internal/scheduler/dateTimestampProvider';

@Component({
  selector: 'app-fleet',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './fleet.html',
  styleUrl: './fleet.scss',
})
export class Fleet implements OnInit {
  private fleetService = inject(TripManagementService);
  private alertService = inject(AlertService);

  buses = signal<Bus[]>([]);
  
  newBusForm = signal<Partial<CreateBusRequest>>({
    plateNumber: '',
    seatCapacity: undefined
  });

  ngOnInit(): void {
    this.loadBuses();
  }

  loadBuses() {
    return this.fleetService.getCompanyBuses().subscribe({
      next: (data) => this.buses.set(data)
    });
  }

  updateNewBusForm(key: string, value: any) {
    this.newBusForm.update(prev => ({ ...prev, [key]: value}));
  }

  submitNewBus() {
    const payload = this.newBusForm() as CreateBusRequest;

    if (!payload.plateNumber || !payload.seatCapacity) {
      this.alertService.error('Hata', 'Lütfen plaka ve koltuk kapasitesini girin.');
      return;
    }

    this.fleetService.createBus(payload).subscribe({
      next: (res) => {
        this.alertService.success('Başarılı', res.message);
        this.loadBuses();
        this.newBusForm.set({ plateNumber: '', seatCapacity: undefined });
      },
      error: (err) => {
        this.alertService.error('Hata', err.error?.message || 'Otobüs eklenemedi.');
      }
    });
  }

  deleteBus(busId: number, plateNumber: string) {
    this.alertService.confirm(
      'Otobüs Silme',
      `${plateNumber} plakalı otobüsü silmek istediğinize emin misiniz?`,
      () => {
        this.fleetService.deleteBus(busId).subscribe({
          next: (res) => {
            this.alertService.success('Başarılı', res.message);
            this.loadBuses();
          },
          error: (err) => {
            this.alertService.error('Hata', err.error?.message || 'Otobüs silinemedi.');
          }
        });
      }
    );
  }
}
