import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { EcommerceOrder } from '../../models/order.model';
import { OrderService } from '../../services/order.service';

@Component({
  selector: 'app-my-orders',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './my-orders.html',
  styleUrl: './my-orders.css',
})
export class MyOrders implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  orders: EcommerceOrder[] = [];
  isLoading = true;
  cancellingOrderId: string | null = null;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadOrders();
  }

  canCancel(order: EcommerceOrder): boolean {
    const status = order.orderStatus?.toLowerCase();
    return status === 'processing' || status === 'confirmed';
  }

  cancelOrder(order: EcommerceOrder): void {
    this.clearMessages();

    if (!this.canCancel(order)) {
      this.errorMessage = 'Đơn hàng ở trạng thái hiện tại không thể hủy.';
      return;
    }

    if (!window.confirm(`Bạn có chắc muốn hủy đơn hàng ${order.id}?`)) return;

    this.cancellingOrderId = order.id;

    this.orderService.cancelMyOrder(order.id).subscribe({
      next: updatedOrder => {
        this.orders = this.orders.map(item => item.id === updatedOrder.id ? updatedOrder : item);
        this.successMessage = 'Hủy đơn hàng thành công. Số lượng sản phẩm đã được hoàn lại kho.';
        this.cancellingOrderId = null;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.cancellingOrderId = null;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  getOrderStatusText(status: string | null): string {
    switch (status?.toLowerCase()) {
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
        return 'Không xác định';
    }
  }

  getOrderStatusClass(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'processing':
        return 'status-processing';
      case 'confirmed':
        return 'status-confirmed';
      case 'shipped':
        return 'status-shipped';
      case 'delivered':
        return 'status-delivered';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return '';
    }
  }

  getPaymentStatusText(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'pending':
        return 'Chưa thanh toán';
      case 'paid':
        return 'Đã thanh toán';
      case 'cancelled':
        return 'Đã hủy';
      case 'failed':
        return 'Thất bại';
      default:
        return 'Không xác định';
    }
  }

  getPaymentMethodText(method: string): string {
    return method?.toLowerCase() === 'cod' ? 'Thanh toán khi nhận hàng' : method;
  }

  private loadOrders(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.orderService.getMyOrders().subscribe({
      next: orders => {
        this.orders = orders;
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
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;

    if (error?.error?.errors) {
      const validationErrors = Object.values(error.error.errors).flat();
      if (validationErrors.length > 0) return String(validationErrors[0]);
    }

    return 'Không thể xử lý đơn hàng. Vui lòng thử lại.';
  }
}
