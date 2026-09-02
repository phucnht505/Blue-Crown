import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EcommerceOrder } from '../../models/order.model';
import { OrderService, OrderStatusUpdate } from '../../services/order.service';

type OrderFilter = 'all' | 'processing' | 'confirmed' | 'shipped' | 'delivered' | 'cancelled';

@Component({
  selector: 'app-order-management',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, FormsModule, RouterLink],
  templateUrl: './order-management.html',
  styleUrl: './order-management.css',
})
export class OrderManagement implements OnInit {
  private readonly orderService = inject(OrderService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  orders: EcommerceOrder[] = [];
  filter: OrderFilter = 'all';
  searchTerm = '';
  isLoading = true;
  processingOrderId: string | null = null;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadOrders();
  }

  get filteredOrders(): EcommerceOrder[] {
    const keyword = this.searchTerm.trim().toLowerCase();

    return this.orders.filter(order => {
      const matchesStatus = this.filter === 'all' || order.orderStatus?.toLowerCase() === this.filter;

      if (!matchesStatus) return false;
      if (!keyword) return true;

      return order.id.toLowerCase().includes(keyword) ||
        order.userName?.toLowerCase().includes(keyword) ||
        order.guestPhone?.toLowerCase().includes(keyword) ||
        order.shippingAddress.toLowerCase().includes(keyword) ||
        order.items.some(item => item.productName.toLowerCase().includes(keyword));
    });
  }

  getCount(status: OrderFilter): number {
    if (status === 'all') return this.orders.length;
    return this.orders.filter(order => order.orderStatus?.toLowerCase() === status).length;
  }

  setFilter(filter: OrderFilter): void {
    this.filter = filter;
  }

  canConfirm(order: EcommerceOrder): boolean {
    return order.orderStatus?.toLowerCase() === 'processing';
  }

  canShip(order: EcommerceOrder): boolean {
    return order.orderStatus?.toLowerCase() === 'confirmed';
  }

  canDeliver(order: EcommerceOrder): boolean {
    return order.orderStatus?.toLowerCase() === 'shipped';
  }

  canCancel(order: EcommerceOrder): boolean {
    const status = order.orderStatus?.toLowerCase();
    return status === 'processing' || status === 'confirmed';
  }

  updateStatus(order: EcommerceOrder, status: OrderStatusUpdate): void {
    this.clearMessages();

    if (!this.isValidTransition(order, status)) {
      this.errorMessage = 'Không thể chuyển đơn hàng sang trạng thái này.';
      return;
    }

    const action = this.getActionText(status);
    const stockWarning = status === 'cancelled' ? ' Số lượng Product của đơn sẽ được hoàn lại kho.' : '';

    if (!window.confirm(`${action} đơn hàng ${order.id}?${stockWarning}`)) return;

    this.processingOrderId = order.id;

    this.orderService.updateStatus(order.id, status).subscribe({
      next: updatedOrder => {
        this.orders = this.orders.map(item => item.id === updatedOrder.id ? updatedOrder : item);
        this.successMessage = `${action} đơn hàng thành công.`;
        this.processingOrderId = null;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.processingOrderId = null;
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
    return method?.toLowerCase() === 'cod' ? 'COD' : method;
  }

  private isValidTransition(order: EcommerceOrder, target: OrderStatusUpdate): boolean {
    const current = order.orderStatus?.toLowerCase();

    if (current === 'processing') return target === 'confirmed' || target === 'cancelled';
    if (current === 'confirmed') return target === 'shipped' || target === 'cancelled';
    if (current === 'shipped') return target === 'delivered';

    return false;
  }

  private getActionText(status: OrderStatusUpdate): string {
    switch (status) {
      case 'confirmed':
        return 'Xác nhận';
      case 'shipped':
        return 'Chuyển sang giao hàng';
      case 'delivered':
        return 'Xác nhận đã giao';
      case 'cancelled':
        return 'Hủy';
    }
  }

  private loadOrders(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.orderService.getManagementOrders().subscribe({
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
      const errors = Object.values(error.error.errors).flat();
      if (errors.length > 0) return String(errors[0]);
    }

    return 'Không thể xử lý đơn hàng. Vui lòng thử lại.';
  }
}
