import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { DispensePrescriptionRequest, Prescription, PrescriptionItem } from '../../models/prescription.model';
import { Product } from '../../models/product.model';
import { PrescriptionService } from '../../services/prescription.service';
import { ProductService } from '../../services/product.service';

type PrescriptionStatusFilter = 'all' | 'pending' | 'approved' | 'dispensed' | 'cancelled';

@Component({
  selector: 'app-pharmacist-prescriptions',
  standalone: true,
  imports: [DatePipe, DecimalPipe, FormsModule, RouterLink],
  templateUrl: './pharmacist-prescriptions.html',
  styleUrl: './pharmacist-prescriptions.css',
})
export class PharmacistPrescriptions implements OnInit {
  private readonly prescriptionService = inject(PrescriptionService);
  private readonly productService = inject(ProductService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  prescriptions: Prescription[] = [];
  productOptionsByItem: Record<string, Product[]> = {};
  selectedProductIdByItem: Record<string, string> = {};
  quantityByItem: Record<string, number> = {};
  statusFilter: PrescriptionStatusFilter = 'all';
  openDispensePrescriptionId: string | null = null;
  loadingDispensePrescriptionId: string | null = null;
  processingPrescriptionId: string | null = null;
  isLoading = true;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadPrescriptions();
  }

  get filteredPrescriptions(): Prescription[] {
    if (this.statusFilter === 'all') return this.prescriptions;
    return this.prescriptions.filter(item => item.status?.toLowerCase() === this.statusFilter);
  }

  setStatusFilter(status: PrescriptionStatusFilter): void {
    this.statusFilter = status;
  }

  approve(prescription: Prescription): void {
    if (!window.confirm('Duyệt đơn thuốc này?')) return;
    this.updateStatus(prescription, 'approved');
  }

  cancelPrescription(prescription: Prescription): void {
    if (!window.confirm('Hủy đơn thuốc này?')) return;
    this.updateStatus(prescription, 'cancelled');
  }

  prepareDispense(prescription: Prescription): void {
    this.clearMessages();
    this.openDispensePrescriptionId = prescription.id;
    this.loadingDispensePrescriptionId = prescription.id;

    const requests = prescription.items.map(item => this.productService.getByMedicationId(item.medicationId));

    forkJoin(requests).subscribe({
      next: results => {
        prescription.items.forEach((item, index) => {
          this.productOptionsByItem[item.id] = results[index] ?? [];
          if (this.quantityByItem[item.id] == null) this.quantityByItem[item.id] = 1;
        });

        const missingItems = prescription.items.filter(item => (this.productOptionsByItem[item.id]?.length ?? 0) === 0);

        if (missingItems.length > 0) {
          this.errorMessage = `Chưa có Product được liên kết với Medication: ${missingItems.map(item => item.medicationName).join(', ')}.`;
        }

        this.loadingDispensePrescriptionId = null;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.loadingDispensePrescriptionId = null;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  closeDispense(): void {
    this.openDispensePrescriptionId = null;
    this.clearMessages();
  }

  dispense(prescription: Prescription): void {
    this.clearMessages();

    const requestItems = [];

    for (const item of prescription.items) {
      const productId = this.selectedProductIdByItem[item.id] ?? '';
      const quantityDispensed = Number(this.quantityByItem[item.id] ?? 0);

      if (!productId) {
        this.errorMessage = `Vui lòng chọn Product cho ${item.medicationName}.`;
        return;
      }

      if (!Number.isInteger(quantityDispensed) || quantityDispensed <= 0) {
        this.errorMessage = `Số lượng cấp cho ${item.medicationName} phải là số nguyên lớn hơn 0.`;
        return;
      }

      const selectedProduct = (this.productOptionsByItem[item.id] ?? []).find(product => product.id === productId);

      if (!selectedProduct) {
        this.errorMessage = `Product được chọn cho ${item.medicationName} không hợp lệ.`;
        return;
      }

      if (quantityDispensed > (selectedProduct.stockQuantity ?? 0)) {
        this.errorMessage = `${selectedProduct.name} chỉ còn ${selectedProduct.stockQuantity ?? 0} sản phẩm trong kho.`;
        return;
      }

      requestItems.push({
        prescriptionItemId: item.id,
        productId,
        quantityDispensed,
      });
    }

    const request: DispensePrescriptionRequest = { items: requestItems };

    if (!window.confirm('Xác nhận cấp thuốc và trừ tồn kho?')) return;

    this.processingPrescriptionId = prescription.id;

    this.prescriptionService.dispense(prescription.id, request).subscribe({
      next: updated => {
        this.replacePrescription(updated);
        this.openDispensePrescriptionId = null;
        this.processingPrescriptionId = null;
        this.successMessage = 'Cấp thuốc thành công. Tồn kho đã được cập nhật.';
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.processingPrescriptionId = null;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  getSelectedProduct(item: PrescriptionItem): Product | null {
    const productId = this.selectedProductIdByItem[item.id];
    if (!productId) return null;
    return (this.productOptionsByItem[item.id] ?? []).find(product => product.id === productId) ?? null;
  }

  getStatusText(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'pending':
        return 'Chờ xử lý';
      case 'approved':
        return 'Đã duyệt';
      case 'dispensed':
        return 'Đã cấp thuốc';
      case 'cancelled':
        return 'Đã hủy';
      default:
        return 'Không xác định';
    }
  }

  getStatusClass(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'pending':
        return 'status-pending';
      case 'approved':
        return 'status-approved';
      case 'dispensed':
        return 'status-dispensed';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return '';
    }
  }

  private loadPrescriptions(): void {
    this.isLoading = true;

    this.prescriptionService.getPharmacistPrescriptions().subscribe({
      next: prescriptions => {
        this.prescriptions = prescriptions;
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private updateStatus(prescription: Prescription, status: 'approved' | 'cancelled'): void {
    this.clearMessages();
    this.processingPrescriptionId = prescription.id;

    this.prescriptionService.updatePharmacistStatus(prescription.id, status).subscribe({
      next: updated => {
        this.replacePrescription(updated);
        this.processingPrescriptionId = null;
        this.successMessage = status === 'approved' ? 'Đã duyệt đơn thuốc.' : 'Đã hủy đơn thuốc.';
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.processingPrescriptionId = null;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private replacePrescription(updated: Prescription): void {
    this.prescriptions = this.prescriptions.map(item => item.id === updated.id ? updated : item);
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();
      if (errors.length > 0) return String(errors[0]);
    }

    return 'Không thể xử lý đơn thuốc. Vui lòng thử lại.';
  }
}
