import { Component, OnInit, inject } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { CartItem } from '../../models/cart-item.model';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [DecimalPipe, RouterLink],
  templateUrl: './cart.html',
  styleUrl: './cart.css',
})
export class Cart implements OnInit {
  private readonly cartService = inject(CartService);

  items: CartItem[] = [];
  message = '';

  ngOnInit(): void {
    this.loadCart();
  }

  get totalAmount(): number {
    return this.items.reduce((total, item) => total + item.product.price * item.quantity, 0);
  }

  increaseQuantity(item: CartItem): void {
    const newQuantity = item.quantity + 1;
    const updated = this.cartService.updateQuantity(item.product.id, newQuantity);

    if (!updated) {
      this.message = 'Số lượng đã đạt mức tồn kho hiện có.';
      return;
    }

    this.message = '';
    this.loadCart();
  }

  decreaseQuantity(item: CartItem): void {
    if (item.quantity <= 1) {
      return;
    }

    this.cartService.updateQuantity(item.product.id, item.quantity - 1);
    this.message = '';
    this.loadCart();
  }

  removeProduct(productId: string): void {
    this.cartService.removeProduct(productId);
    this.message = '';
    this.loadCart();
  }

  clearCart(): void {
    this.cartService.clearCart();
    this.message = '';
    this.loadCart();
  }

  private loadCart(): void {
    this.items = this.cartService.getItems();
  }
}
