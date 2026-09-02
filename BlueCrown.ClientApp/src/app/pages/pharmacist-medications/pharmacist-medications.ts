import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Medication } from '../../models/medication.model';
import { MedicationService } from '../../services/medication.service';

@Component({
  selector: 'app-pharmacist-medications',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './pharmacist-medications.html',
  styleUrl: './pharmacist-medications.css',
})
export class PharmacistMedications implements OnInit {
  private readonly medicationService = inject(MedicationService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  medications: Medication[] = [];
  editingId: string | null = null;
  name = '';
  genericName = '';
  category = '';
  searchTerm = '';
  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';
  fieldErrors: Record<string, string> = {};

  ngOnInit(): void {
    this.loadMedications();
  }

  get filteredMedications(): Medication[] {
    const keyword = this.searchTerm.trim().toLowerCase();
    if (!keyword) return this.medications;

    return this.medications.filter(item =>
      item.name.toLowerCase().includes(keyword) ||
      item.genericName?.toLowerCase().includes(keyword) ||
      item.category?.toLowerCase().includes(keyword)
    );
  }

  save(): void {
    this.clearMessages();
    this.fieldErrors = {};

    if (!this.validateForm()) {
      this.errorMessage = 'Vui lòng kiểm tra lại thông tin Medication.';
      return;
    }

    const request = {
      name: this.name.trim(),
      genericName: this.genericName.trim() || null,
      category: this.category.trim() || null,
    };

    this.isSaving = true;

    if (this.editingId) {
      this.medicationService.update(this.editingId, request).subscribe({
        next: response => {
          this.successMessage = response.message;
          this.resetForm();
          this.loadMedications(false);
        },
        error: error => {
          this.errorMessage = this.getApiErrorMessage(error);
          this.isSaving = false;
          this.changeDetectorRef.detectChanges();
        },
      });
      return;
    }

    this.medicationService.create(request).subscribe({
      next: medication => {
        this.medications = [...this.medications, medication].sort((a, b) => a.name.localeCompare(b.name));
        this.successMessage = 'Thêm Medication thành công.';
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

  edit(medication: Medication): void {
    this.clearMessages();
    this.fieldErrors = {};
    this.editingId = medication.id;
    this.name = medication.name;
    this.genericName = medication.genericName ?? '';
    this.category = medication.category ?? '';
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit(): void {
    this.resetForm();
    this.clearMessages();
  }

  deleteMedication(medication: Medication): void {
    this.clearMessages();

    if (!window.confirm(`Xóa Medication "${medication.name}"?`)) return;

    this.medicationService.delete(medication.id).subscribe({
      next: response => {
        this.medications = this.medications.filter(item => item.id !== medication.id);
        this.successMessage = response.message;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private validateForm(): boolean {
    const name = this.name.trim();
    const genericName = this.genericName.trim();
    const category = this.category.trim();

    if (!name) this.fieldErrors['name'] = 'Tên Medication không được để trống.';
    else if (name.length < 2 || name.length > 150) this.fieldErrors['name'] = 'Tên Medication phải từ 2 đến 150 ký tự.';
    else if (!/[A-Za-zÀ-ỹ]/u.test(name)) this.fieldErrors['name'] = 'Tên Medication phải chứa ít nhất một chữ cái.';

    if (genericName.length > 150) this.fieldErrors['genericName'] = 'Tên generic tối đa 150 ký tự.';
    else if (genericName && !/[A-Za-zÀ-ỹ]/u.test(genericName)) this.fieldErrors['genericName'] = 'Tên generic phải chứa ít nhất một chữ cái.';

    if (category.length > 100) this.fieldErrors['category'] = 'Nhóm thuốc tối đa 100 ký tự.';
    else if (category && !/[A-Za-zÀ-ỹ]/u.test(category)) this.fieldErrors['category'] = 'Nhóm thuốc phải chứa ít nhất một chữ cái.';

    return Object.keys(this.fieldErrors).length === 0;
  }

  private loadMedications(showLoading = true): void {
    if (showLoading) this.isLoading = true;

    this.medicationService.getAll().subscribe({
      next: medications => {
        this.medications = medications;
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
    this.name = '';
    this.genericName = '';
    this.category = '';
    this.fieldErrors = {};
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private getApiErrorMessage(error: any): string {
    if (error?.error?.message) return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();
      if (errors.length > 0) return String(errors[0]);
    }

    return 'Không thể xử lý Medication. Vui lòng thử lại.';
  }
}
