import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CreateInventoryReceiptRequest, InventoryReceipt } from '../../models/inventory-receipt.model';
import { Product } from '../../models/product.model';
import { Supplier } from '../../models/supplier.model';
import { InventoryReceiptService } from '../../services/inventory-receipt.service';
import { ProductService } from '../../services/product.service';
import { SupplierService } from '../../services/supplier.service';

@Component({
  selector: 'app-pharmacist-inventory',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, ReactiveFormsModule, RouterLink],
  templateUrl: './pharmacist-inventory.html',
  styleUrl: './pharmacist-inventory.css',
})
export class PharmacistInventory implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly receiptService = inject(InventoryReceiptService);
  private readonly supplierService = inject(SupplierService);
  private readonly productService = inject(ProductService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  suppliers: Supplier[] = [];
  products: Product[] = [];
  receipts: InventoryReceipt[] = [];
  isLoading = true;
  isSaving = false;
  submitted = false;
  errorMessage = '';
  successMessage = '';
  readonly minExpirationDate = this.getTomorrowDate();

  receiptForm = this.formBuilder.group({
    supplierId: ['', Validators.required],
    details: this.formBuilder.array([this.createDetailGroup()]),
  });

  get details(): FormArray {
    return this.receiptForm.controls.details;
  }

  get gdpSuppliers(): Supplier[] {
    return this.suppliers.filter(item => item.gdpCertified === true);
  }

  ngOnInit(): void {
    this.loadData();
  }

  addDetail(): void {
    this.details.push(this.createDetailGroup());
  }

  removeDetail(index: number): void {
    if (this.details.length === 1) return;
    this.details.removeAt(index);
  }

  isInvalid(controlName: string, index?: number): boolean {
    const control = index == null ? this.receiptForm.get(controlName) : this.details.at(index).get(controlName);
    return !!control && control.invalid && (control.touched || this.submitted);
  }

  save(): void {
    this.clearMessages();
    this.submitted = true;
    this.receiptForm.markAllAsTouched();

    if (this.receiptForm.invalid) {
      this.errorMessage = 'Vui lòng nhập đầy đủ và đúng thông tin phiếu nhập.';
      return;
    }

    const rawValue = this.receiptForm.getRawValue();
    const detailValues = rawValue.details;

    for (const item of detailValues) {
      if (!item['batchNumber']?.trim()) {
        this.errorMessage = 'Số lô không được để trống.';
        return;
      }

      if (item['expirationDate'] && item['expirationDate'] < this.minExpirationDate) {
        this.errorMessage = 'Hạn sử dụng phải sau ngày hiện tại.';
        return;
      }
    }

    const duplicated = detailValues.some((item, index) => detailValues.findIndex(other => other['productId'] === item['productId'] && other['batchNumber']?.trim().toLowerCase() === item['batchNumber']?.trim().toLowerCase()) !== index);

    if (duplicated) {
      this.errorMessage = 'Không được nhập trùng cùng Product và số lô trong một phiếu.';
      return;
    }

    const request: CreateInventoryReceiptRequest = {
      supplierId: rawValue.supplierId ?? '',
      details: detailValues.map(item => ({
        productId: item['productId'] ?? '',
        batchNumber: item['batchNumber']?.trim() ?? '',
        expirationDate: item['expirationDate'] ?? '',
        quantityImported: Number(item['quantityImported'] ?? 0),
        importPrice: Number(item['importPrice'] ?? 0),
      })),
    };

    this.isSaving = true;

    this.receiptService.create(request).subscribe({
      next: () => {
        this.successMessage = 'Tạo phiếu nhập thành công. Phiếu đang chờ Admin duyệt.';
        this.resetForm();
        this.loadReceipts();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  getStatusText(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'pending_approval':
        return 'Chờ duyệt';
      case 'approved':
        return 'Đã duyệt';
      case 'rejected':
        return 'Đã từ chối';
      default:
        return 'Không xác định';
    }
  }

  getStatusClass(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'pending_approval':
        return 'status-pending';
      case 'approved':
        return 'status-approved';
      case 'rejected':
        return 'status-rejected';
      default:
        return '';
    }
  }

  private loadData(): void {
    forkJoin({
      suppliers: this.supplierService.getAll(),
      products: this.productService.getAll(),
      receipts: this.receiptService.getAll(),
    }).subscribe({
      next: result => {
        this.suppliers = result.suppliers;
        this.products = result.products;
        this.receipts = result.receipts;
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

  private loadReceipts(): void {
    this.receiptService.getAll().subscribe({
      next: receipts => {
        this.receipts = receipts;
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

  private createDetailGroup(): FormGroup {
    return this.formBuilder.group({
      productId: ['', Validators.required],
      batchNumber: ['', [Validators.required, Validators.maxLength(100)]],
      expirationDate: ['', Validators.required],
      quantityImported: [1, [Validators.required, Validators.min(1)]],
      importPrice: [1, [Validators.required, Validators.min(0.01)]],
    });
  }

  private resetForm(): void {
    this.receiptForm.controls.supplierId.setValue('');
    this.details.clear();
    this.details.push(this.createDetailGroup());
    this.submitted = false;
  }

  private getTomorrowDate(): string {
    const date = new Date();
    date.setDate(date.getDate() + 1);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
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

    return 'Không thể xử lý phiếu nhập. Vui lòng thử lại.';
  }
}
