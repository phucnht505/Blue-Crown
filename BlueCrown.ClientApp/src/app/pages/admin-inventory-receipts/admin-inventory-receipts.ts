import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { InventoryReceipt } from '../../models/inventory-receipt.model';
import { InventoryReceiptService } from '../../services/inventory-receipt.service';

type ReceiptStatusFilter = 'all' | 'pending_approval' | 'approved' | 'rejected';

@Component({
  selector: 'app-admin-inventory-receipts',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './admin-inventory-receipts.html',
  styleUrl: './admin-inventory-receipts.css',
})
export class AdminInventoryReceipts implements OnInit {
  private readonly receiptService = inject(InventoryReceiptService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  receipts: InventoryReceipt[] = [];
  statusFilter: ReceiptStatusFilter = 'all';
  processingReceiptId: string | null = null;
  isLoading = true;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadReceipts();
  }

  get filteredReceipts(): InventoryReceipt[] {
    if (this.statusFilter === 'all') return this.receipts;
    return this.receipts.filter(receipt => receipt.status?.toLowerCase() === this.statusFilter);
  }

  setStatusFilter(status: ReceiptStatusFilter): void {
    this.statusFilter = status;
  }

  approve(receipt: InventoryReceipt): void {
    this.clearMessages();

    if (!window.confirm(`Duyệt phiếu nhập của "${receipt.supplierName || 'nhà cung cấp này'}"? Tồn kho sẽ được cộng sau khi duyệt.`)) return;

    this.processingReceiptId = receipt.id;

    this.receiptService.approve(receipt.id).subscribe({
      next: response => {
        this.successMessage = response.message;
        this.processingReceiptId = null;
        this.loadReceipts(false);
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.processingReceiptId = null;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  reject(receipt: InventoryReceipt): void {
    this.clearMessages();

    if (!window.confirm(`Từ chối phiếu nhập của "${receipt.supplierName || 'nhà cung cấp này'}"?`)) return;

    this.processingReceiptId = receipt.id;

    this.receiptService.reject(receipt.id).subscribe({
      next: response => {
        this.successMessage = response.message;
        this.processingReceiptId = null;
        this.loadReceipts(false);
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.processingReceiptId = null;
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

  private loadReceipts(showLoading = true): void {
    if (showLoading) this.isLoading = true;

    this.receiptService.getAll().subscribe({
      next: receipts => {
        this.receipts = receipts;
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
