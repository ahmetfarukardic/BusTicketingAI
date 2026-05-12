import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BusCompanyService } from '../../../services/bus-company.service';
import { AlertService } from '../../../services/alert.service';

@Component({
  selector: 'app-bus-companies',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bus-companies.html',
  styleUrl: './bus-companies.scss',
})
export class BusCompanies implements OnInit{
  private companyService = inject(BusCompanyService);
  private alertService = inject(AlertService);

  companies: any[] = [];
  newCompanyName: string = '';
  editingCompanyId: number | null = null;
  editCompanyName: string = '';
  showAddForm: boolean = false;
  searchText: string = '';

  ngOnInit() {
    this.loadCompanies();
  }

  loadCompanies() {
    this.companyService.getAll().subscribe({
      next: (data) => {
        this.companies = data;
      },
      error: (err) => console.error('Firmalar yüklenirken hata:', err)
    });
  }

  get filteredCompanies() {
    if (!this.searchText) {
      return this.companies;
    }
    return this.companies.filter(c => c.name.toLowerCase().includes(this.searchText.toLocaleLowerCase()));
  }

  toggleAddForm() {
    this.showAddForm = !this.showAddForm;
    this.newCompanyName = '';
  }

  addCompany() {
    if (!this.newCompanyName.trim()) return;

    const payload = { name: this.newCompanyName };

    this.companyService.create(payload).subscribe({
      next: () => {
        this.newCompanyName = '';
        this.showAddForm = false;
        this.loadCompanies();
        this.alertService.success('Eklendi!', 'Yeni firma başarıyla oluşturuldu.');
      },
      error: (err) => this.alertService.error('Hata!', 'Firma eklenirken bir sorun oluştu.')
    });
  }

  deleteCompany(company: any) {
    this.alertService.confirm(
      'Firma Silme',
      `"${company.name}" firmasını silmek istediğinize emin misiniz?`,
      () => {
        this.companyService.delete(company.id).subscribe({
          next: () => {
            this.loadCompanies();
            this.alertService.success('Silindi!', 'Firma başarıyla kaldırıldı.');
          },
          error: (err) => {
            this.alertService.error('Hata!', 'Silme işlemi başarısız oldu.');
          }
        });
      }
    );
  }

  startEdit(company: any) {
    this.editingCompanyId = company.id;
    this.editCompanyName = company.name;
  }

  cancelEdit() {
    this.editingCompanyId = null;
    this.editCompanyName = '';
  }

  saveEdit(id: number) {
    if (!this.editCompanyName.trim()) return;

    const payload = { id: id, name: this.editCompanyName };
    this.companyService.update(id, payload).subscribe({
      next: () => {
        this.editingCompanyId = null;
        this.loadCompanies();
      },
      error: (err) => alert('Güncelleme başarısız oldu!')
    });
  }
}
