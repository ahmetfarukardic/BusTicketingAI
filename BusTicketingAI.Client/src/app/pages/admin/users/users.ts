import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../../services/user.service';
import { BusCompanyService } from '../../../services/bus-company.service';
import { AlertService } from '../../../services/alert.service';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule, DialogModule, ButtonModule],
  templateUrl: './users.html',
  styleUrl: './users.scss',
})
export class Users implements OnInit{
  private userService = inject(UserService);
  private companyService = inject(BusCompanyService);
  private alertService = inject(AlertService);

  users: any[] = [];
  companies: any[] = [];
  searchText: string = '';
  showAddForm: boolean = false;

  newUser = { firstName: '', lastName: '', email: '', password: '', role: 'Member', companyId: null };
  editingUserId: string | null = null;
  editUser = { firstName: '', lastName: '', email: '', role: 'Member', companyId: null as number | null };

  isWalletModalOpen = false;
  selectedUserForWallet: any = null;
  currentUserBalance: number = 0;
  walletAmountInput: number | null = null;
  isAdjustingWallet = false;

  ngOnInit(): void {
    this.loadUsers();
    this.loadCompanies();
  }

  loadUsers() {
    this.userService.getUsers().subscribe({
      next: (data) => this.users = data,
      error: (err) => this.alertService.error('Hata!', 'Kullanıcılar yüklenemedi.')
    });
  }

  loadCompanies() {
    this.companyService.getAll().subscribe(data => this.companies = data);
  }

  get filteredUsers() {
    if (!this.searchText) return this.users;
    return this.users.filter(u => 
      u.firstName.toLowerCase().includes(this.searchText.toLowerCase()) || 
      u.email.toLowerCase().includes(this.searchText.toLowerCase()));
  }

  toggleAddForm() {
    this.showAddForm = !this.showAddForm;
    this.newUser = { firstName: '', lastName: '', email: '', password: '', role: 'Member', companyId: null };
  }

  addUser() {
    if (!this.newUser.firstName || !this.newUser.email || !this.newUser.password) return;

    if (this.newUser.role !== "CompanyStaff") {
      this.newUser.companyId = null;
    }

    this.userService.createUser(this.newUser).subscribe({
      next: () => {
        this.toggleAddForm();
        this.loadUsers();
        this.alertService.success('Eklendi!', 'Yeni kullanıcı başarıyla oluşturuldu.');
      },
      error: () => this.alertService.error('Hata!', 'Kullanıcı eklenirken bir sorun oluştu.')
    });
  }

  startEdit(user: any) {
    this.editingUserId = user.id;
    this.editUser = { ...user };
  }

  cancelEdit() {
    this.editingUserId = null;
  }

  saveEdit(id: string) {
    if (this.editUser.role !== 'CompanyStaff') this.editUser.companyId = null;

    const payload = { id: id, ...this.editUser };

    this.userService.updateUser(payload).subscribe({
      next: () => {
        this.editingUserId = null;
        this.loadUsers();
        this.alertService.success('Güncellendi!', 'Kullanıcı bilgileri güncellendi.');
      },
      error: () => this.alertService.error('Hata!', 'Güncelleme başarısız oldu.')
    });
  }

  deleteUser(user: any) {
    this.alertService.confirm(
      'Kullanıcı Silme',
      `"${user.firstName} ${user.lastName}" kullanıcısını silmek istediğinize emin misiniz?`,
      () => {
        this.userService.deleteUser(user.id).subscribe({
          next: () => {
            this.loadUsers();
            this.alertService.success('Silindi!', 'Kullanıcı sistemden kaldırıldı.');
          },
          error: () => this.alertService.error('Hata!', 'Silme işlemi başarısız.')
        });
      }
    );
  }

  openWalletModal(user: any) {
    this.selectedUserForWallet = user;
    this.walletAmountInput = null;
    this.currentUserBalance = 0;
    this.isWalletModalOpen = true;

    this.userService.getUserBalance(user.id).subscribe({
      next: (balance) => this.currentUserBalance = balance,
      error: () => this.alertService.error('Hata', 'Bakiye bilgisi çekilemedi.')
    });
  }

  submitWalletAdjustment(isAdding: boolean) {
    if (!this.walletAmountInput || this.walletAmountInput <= 0) {
      this.alertService.warning('Uyarı', 'Lütfen geçerli bir tutar girin.');
      return;
    }

    const finalAmount = isAdding ? this.walletAmountInput : -Math.abs(this.walletAmountInput);

    if (!isAdding && this.walletAmountInput > this.currentUserBalance) {
      this.alertService.warning('Uyarı', 'Kullanıcının bakiyesinden daha büyük bir kesinti yapamazsınız.');
      return;
    }

    this.isAdjustingWallet = true;

    const payload = {
      userId: this.selectedUserForWallet.id,
      amount: finalAmount
    };

    this.userService.adjustWalletBalance(payload).subscribe({
      next: () => {
        this.isAdjustingWallet = false;
        this.isWalletModalOpen = false;
        const actionText = isAdding ? 'yüklendi' : 'düşüldü';
        this.alertService.success('Başarılı', `Kullanıcı cüzdanına işlem uygulandı. Tutar başarıyla ${actionText}.`);
      },
      error: () => {
        this.isAdjustingWallet = false;
        this.alertService.error('Hata', 'Cüzdan işlemi gerçekleştirilemedi.');
      }
    });
  }
}