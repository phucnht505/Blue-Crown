import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectorRef, Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EcommerceOrder } from '../../models/order.model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-guest-order-lookup',
  standalone: true,
  imports: [ReactiveFormsModule, DecimalPipe, DatePipe],
  templateUrl: './guest-order-lookup.html',
  styleUrl: './guest-order-lookup.css',
})
export class GuestOrderLookup {
  private readonly formBuilder = inject(FormBuilder);
  private readonly orderService = inject(OrderService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  orders: EcommerceOrder[] = [];
  selectedOrder: EcommerceOrder | null = null;
  isLoading = false;
  errorMessage = '';

  lookupForm = this.formBuilder.nonNullable.group({
    orderId: ['', Validators.pattern(/^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$/)],
    guestPhone: ['', [Validators.required, Validators.pattern(/^(0[35789]\d{8}|\+84[35789]\d{8})$/)]],
  });

  lookup(): void {
    this.errorMessage = '';
    this.orders = [];
    this.selectedOrder = null;
    this.lookupForm.markAllAsTouched();

    if (this.lookupForm.invalid)
      return;

    const value = this.lookupForm.getRawValue();

    this.isLoading = true;

    this.orderService.lookupGuestOrders({
      orderId: value.orderId.trim() || null,
      guestPhone: value.guestPhone.trim(),
    }).subscribe({
      next: orders => {
        this.orders = orders;

        if (orders.length === 1)
          this.selectedOrder = orders[0];

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

  selectOrder(order: EcommerceOrder): void {
    this.selectedOrder = order;
    this.changeDetectorRef.detectChanges();
  }

  reset(): void {
    this.lookupForm.reset({
      orderId: '',
      guestPhone: '',
    });

    this.orders = [];
    this.selectedOrder = null;
    this.errorMessage = '';
  }

  getOrderStatusLabel(status: string): string {
    switch (status) {
      case 'processing':
        return 'Đang xử lý';
      case 'confirmed':
        return 'Đã xác nhận';
      case 'shipped':
        return 'Đang giao';
      case 'delivered':
        return 'Đã giao';
      case 'cancelled':
        return 'Đã hủy';
      default:
        return status || '-';
    }
  }

  getPaymentStatusLabel(status: string): string {
    switch (status) {
      case 'pending':
        return 'Chưa thanh toán';
      case 'paid':
        return 'Đã thanh toán';
      case 'cancelled':
        return 'Đã hủy';
      default:
        return status || '-';
    }
  }

  getPaymentMethodLabel(method: string): string {
    return method?.toLowerCase() === 'cod'
      ? 'Thanh toán khi nhận hàng (COD)'
      : method || '-';
  }

  getOrderTotal(item: { quantity: number; unitPrice: number }): number {
    return item.quantity * item.unitPrice;
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string')
      return error.error;

    if (error?.error?.message)
      return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();

      if (errors.length > 0)
        return String(errors[0]);
    }

    return 'Không thể tra cứu đơn hàng. Vui lòng thử lại.';
  }
}
