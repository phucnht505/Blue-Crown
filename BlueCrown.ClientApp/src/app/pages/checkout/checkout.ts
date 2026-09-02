import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { CartItem } from '../../models/cart-item.model';
import { CheckoutResponse, CreateCheckoutRequest } from '../../models/checkout.model';
import { Prescription } from '../../models/prescription.model';
import { AuthService } from '../../services/auth.service';
import { CartService } from '../../services/cart.service';
import { CheckoutService } from '../../services/checkout.service';
import { OrderService } from '../../services/order.service';
import { PrescriptionService } from '../../services/prescription.service';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [FormsModule, DecimalPipe, RouterLink],
  templateUrl: './checkout.html',
  styleUrl: './checkout.css',
})
export class Checkout implements OnInit {
  private readonly authService = inject(AuthService);
  private readonly cartService = inject(CartService);
  private readonly checkoutService = inject(CheckoutService);
  private readonly orderService = inject(OrderService);
  private readonly prescriptionService = inject(PrescriptionService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);
  private readonly router = inject(Router);

  items: CartItem[] = [];
  prescriptions: Prescription[] = [];
  usedPrescriptionIds = new Set<string>();
  guestPhone = '';
  shippingAddress = '';
  paymentMethod = 'cod';
  prescriptionId = '';
  isSubmitting = false;
  isLoadingPrescriptions = false;
  errorMessage = '';
  prescriptionMessage = '';
  order: CheckoutResponse | null = null;

  fieldErrors = {
    guestPhone: '',
    shippingAddress: '',
    paymentMethod: '',
    prescriptionId: '',
  };

  ngOnInit(): void {
    this.items = this.cartService.getItems();

    if (this.hasPrescriptionRequiredProduct)
      this.loadPrescriptions();
  }

  get totalAmount(): number {
    return this.items.reduce((total, item) => total + item.product.price * item.quantity, 0);
  }

  get prescriptionRequiredItems(): CartItem[] {
    return this.items.filter(item => item.product.isPrescriptionRequired === true);
  }

  get hasPrescriptionRequiredProduct(): boolean {
    return this.prescriptionRequiredItems.length > 0;
  }

  get eligiblePrescriptions(): Prescription[] {
    const requiredMedicationIds = this.prescriptionRequiredItems
      .map(item => item.product.medicationId)
      .filter((id): id is string => !!id);

    if (requiredMedicationIds.length !== this.prescriptionRequiredItems.length)
      return [];

    return this.prescriptions.filter(prescription => {
      if (prescription.status?.toLowerCase() !== 'approved')
        return false;

      if (this.usedPrescriptionIds.has(prescription.id))
        return false;

      const prescriptionMedicationIds = new Set(prescription.items.map(item => item.medicationId));

      return requiredMedicationIds.every(id => prescriptionMedicationIds.has(id));
    });
  }

  validateGuestPhone(): boolean {
    const phone = this.guestPhone.trim().replace(/[\s.-]/g, '');

    if (!phone) {
      this.fieldErrors.guestPhone = 'Vui lòng nhập số điện thoại.';
      return false;
    }

    const vietnamPhoneRegex = /^(?:\+84|0)(3|5|7|8|9)\d{8}$/;

    if (!vietnamPhoneRegex.test(phone)) {
      this.fieldErrors.guestPhone = 'Số điện thoại không hợp lệ. Ví dụ: 0901234567.';
      return false;
    }

    this.fieldErrors.guestPhone = '';
    return true;
  }

  validateShippingAddress(): boolean {
    const address = this.shippingAddress.trim();

    if (!address) {
      this.fieldErrors.shippingAddress = 'Vui lòng nhập địa chỉ giao hàng.';
      return false;
    }

    if (address.length < 10) {
      this.fieldErrors.shippingAddress = 'Địa chỉ giao hàng phải có ít nhất 10 ký tự.';
      return false;
    }

    if (address.length > 500) {
      this.fieldErrors.shippingAddress = 'Địa chỉ giao hàng không được vượt quá 500 ký tự.';
      return false;
    }

    if (!/[A-Za-zÀ-ỹ]/u.test(address)) {
      this.fieldErrors.shippingAddress = 'Địa chỉ giao hàng phải chứa thông tin chữ hợp lệ.';
      return false;
    }

    this.fieldErrors.shippingAddress = '';
    return true;
  }

  validatePaymentMethod(): boolean {
    if (this.paymentMethod !== 'cod') {
      this.fieldErrors.paymentMethod = 'Hiện tại hệ thống chỉ hỗ trợ thanh toán khi nhận hàng.';
      return false;
    }

    this.fieldErrors.paymentMethod = '';
    return true;
  }

  validatePrescription(): boolean {
    if (!this.hasPrescriptionRequiredProduct) {
      this.fieldErrors.prescriptionId = '';
      this.prescriptionId = '';
      return true;
    }

    const productWithoutMedication = this.prescriptionRequiredItems.find(item => !item.product.medicationId);

    if (productWithoutMedication) {
      this.fieldErrors.prescriptionId = `Product "${productWithoutMedication.product.name}" chưa được liên kết Medication.`;
      return false;
    }

    if (!this.authService.getCurrentUser()) {
      this.fieldErrors.prescriptionId = 'Product cần đơn thuốc yêu cầu bạn đăng nhập.';
      return false;
    }

    if (!this.prescriptionId) {
      this.fieldErrors.prescriptionId = 'Vui lòng chọn Prescription phù hợp.';
      return false;
    }

    const validPrescription = this.eligiblePrescriptions.some(item => item.id === this.prescriptionId);

    if (!validPrescription) {
      this.fieldErrors.prescriptionId = 'Prescription được chọn không phù hợp hoặc đã được sử dụng cho một đơn hàng.';
      return false;
    }

    this.fieldErrors.prescriptionId = '';
    return true;
  }

  clearFieldError(field: 'guestPhone' | 'shippingAddress' | 'paymentMethod' | 'prescriptionId'): void {
    this.fieldErrors[field] = '';
  }

  submitOrder(): void {
    this.errorMessage = '';

    if (this.items.length === 0) {
      this.errorMessage = 'Giỏ hàng đang trống.';
      return;
    }

    const invalidQuantityItem = this.items.find(item => item.quantity < 1 || item.quantity > 99);

    if (invalidQuantityItem) {
      this.errorMessage = `Số lượng Product "${invalidQuantityItem.product.name}" phải từ 1 đến 99.`;
      return;
    }

    const outOfStockItem = this.items.find(item => (item.product.stockQuantity ?? 0) <= 0);

    if (outOfStockItem) {
      this.errorMessage = `Product "${outOfStockItem.product.name}" đã hết hàng.`;
      return;
    }

    const overStockItem = this.items.find(item => item.quantity > (item.product.stockQuantity ?? 0));

    if (overStockItem) {
      this.errorMessage = `Product "${overStockItem.product.name}" chỉ còn ${overStockItem.product.stockQuantity ?? 0} sản phẩm.`;
      return;
    }

    const isPhoneValid = this.validateGuestPhone();
    const isAddressValid = this.validateShippingAddress();
    const isPaymentValid = this.validatePaymentMethod();
    const isPrescriptionValid = this.validatePrescription();

    if (!isPhoneValid || !isAddressValid || !isPaymentValid || !isPrescriptionValid)
      return;

    const request: CreateCheckoutRequest = {
      guestPhone: this.normalizePhone(),
      shippingAddress: this.shippingAddress.trim(),
      paymentMethod: 'cod',
      prescriptionId: this.hasPrescriptionRequiredProduct ? this.prescriptionId : null,
      items: this.items.map(item => ({
        productId: item.product.id,
        quantity: item.quantity,
      })),
    };

    this.isSubmitting = true;

    this.checkoutService.create(request).subscribe({
      next: response => {
        this.order = response;
        this.cartService.clearCart();
        this.items = [];
        this.isSubmitting = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        console.error('Lỗi tạo Order:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSubmitting = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  continueShopping(): void {
    this.router.navigate(['/products']);
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

  getPaymentStatusText(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'pending':
        return 'Chưa thanh toán';
      case 'paid':
        return 'Đã thanh toán';
      case 'cancelled':
        return 'Đã hủy';
      case 'failed':
        return 'Thanh toán thất bại';
      default:
        return 'Không xác định';
    }
  }

  getPaymentMethodText(method: string): string {
    return method.toLowerCase() === 'cod' ? 'Thanh toán khi nhận hàng' : method;
  }

  private loadPrescriptions(): void {
    const user = this.authService.getCurrentUser();

    if (!user) {
      this.prescriptionMessage = 'Đăng nhập để sử dụng Prescription cho Product cần đơn thuốc.';
      return;
    }

    if (user.role.toLowerCase() !== 'patient') {
      this.prescriptionMessage = 'Tài khoản hiện tại không có Patient Profile để chọn Prescription. Bạn vẫn có thể mua các Product không yêu cầu đơn thuốc.';
      return;
    }

    this.isLoadingPrescriptions = true;

    forkJoin({
      prescriptions: this.prescriptionService.getPatientPrescriptions(),
      orders: this.orderService.getMyOrders(),
    }).subscribe({
      next: result => {
        this.prescriptions = result.prescriptions;
        this.usedPrescriptionIds = new Set(
          result.orders
            .filter(order => order.prescriptionId && order.orderStatus?.toLowerCase() !== 'cancelled')
            .map(order => order.prescriptionId as string)
        );

        this.isLoadingPrescriptions = false;

        if (this.eligiblePrescriptions.length === 0)
          this.prescriptionMessage = 'Không có Prescription đã duyệt, chưa sử dụng và phù hợp với các Product cần kê đơn trong giỏ hàng.';
        else
          this.prescriptionMessage = '';

        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.prescriptionMessage = this.getApiErrorMessage(error);
        this.isLoadingPrescriptions = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private normalizePhone(): string {
    const phone = this.guestPhone.trim().replace(/[\s.-]/g, '');

    if (phone.startsWith('+84'))
      return `0${phone.substring(3)}`;

    return phone;
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string')
      return error.error;

    if (error?.error?.message)
      return error.error.message;

    if (error?.error?.errors) {
      const validationErrors = Object.values(error.error.errors).flat();

      if (validationErrors.length > 0)
        return String(validationErrors[0]);
    }

    return 'Không thể xử lý yêu cầu. Vui lòng thử lại.';
  }
}
