import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Supplier } from '../../models/supplier.model';
import { SupplierService } from '../../services/supplier.service';

@Component({
  selector: 'app-pharmacist-suppliers',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './pharmacist-suppliers.html',
  styleUrl: './pharmacist-suppliers.css',
})
export class PharmacistSuppliers implements OnInit {
  private readonly supplierService = inject(SupplierService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  suppliers: Supplier[] = [];
  editingId: string | null = null;
  supplierName = '';
  contactPhone = '';
  gdpCertified = true;
  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadSuppliers();
  }

  save(): void {
    this.clearMessages();

    const supplierName = this.supplierName.trim();
    const contactPhone = this.contactPhone.trim();

    if (!supplierName) {
      this.errorMessage = 'Tên nhà cung cấp không được để trống.';
      return;
    }

    if (supplierName.length < 2 || supplierName.length > 255) {
      this.errorMessage = 'Tên nhà cung cấp phải từ 2 đến 255 ký tự.';
      return;
    }

    if (!/[A-Za-zÀ-ỹ]/u.test(supplierName)) {
      this.errorMessage = 'Tên nhà cung cấp không được chỉ chứa số hoặc ký tự đặc biệt.';
      return;
    }

    if (!contactPhone) {
      this.errorMessage = 'Số điện thoại không được để trống.';
      return;
    }

    if (!/^(0[35789]\d{8}|\+84[35789]\d{8})$/.test(contactPhone)) {
      this.errorMessage = 'Số điện thoại không hợp lệ.';
      return;
    }

    const request = { supplierName, contactPhone, gdpCertified: this.gdpCertified };
    this.isSaving = true;

    if (this.editingId) {
      this.supplierService.update(this.editingId, request).subscribe({
        next: () => {
          this.successMessage = 'Cập nhật nhà cung cấp thành công.';
          this.resetForm();
          this.loadSuppliers(false);
        },
        error: error => {
          this.errorMessage = this.getApiErrorMessage(error);
          this.isSaving = false;
          this.changeDetectorRef.detectChanges();
        },
      });
      return;
    }

    this.supplierService.create(request).subscribe({
      next: supplier => {
        this.suppliers = [...this.suppliers, supplier].sort((a, b) => a.supplierName.localeCompare(b.supplierName));
        this.successMessage = 'Thêm nhà cung cấp thành công.';
        this.resetForm();
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  edit(supplier: Supplier): void {
    this.clearMessages();
    this.editingId = supplier.id;
    this.supplierName = supplier.supplierName;
    this.contactPhone = supplier.contactPhone ?? '';
    this.gdpCertified = supplier.gdpCertified ?? false;
  }

  cancelEdit(): void {
    this.resetForm();
    this.clearMessages();
  }

  deleteSupplier(supplier: Supplier): void {
    this.clearMessages();

    if (!window.confirm(`Xóa nhà cung cấp "${supplier.supplierName}"?`))
      return;

    this.supplierService.delete(supplier.id).subscribe({
      next: () => {
        this.suppliers = this.suppliers.filter(item => item.id !== supplier.id);
        this.successMessage = 'Xóa nhà cung cấp thành công.';
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private loadSuppliers(showLoading = true): void {
    if (showLoading)
      this.isLoading = true;

    this.supplierService.getAll().subscribe({
      next: suppliers => {
        this.suppliers = suppliers;
        this.isLoading = false;
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private resetForm(): void {
    this.editingId = null;
    this.supplierName = '';
    this.contactPhone = '';
    this.gdpCertified = true;
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private getApiErrorMessage(error: any): string {
    if (error?.error?.message)
      return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();

      if (errors.length > 0)
        return String(errors[0]);
    }

    return 'Không thể xử lý nhà cung cấp. Vui lòng thử lại.';
  }
}
