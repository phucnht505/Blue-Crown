import { Injectable, inject } from '@angular/core';
import { CartItem } from '../models/cart-item.model';
import { Product } from '../models/product.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private readonly authService = inject(AuthService);
  private readonly baseStorageKey = 'blue-crown-cart';
  private readonly maxQuantityPerProduct = 99;

  getItems(): CartItem[] {
    const data = localStorage.getItem(this.getStorageKey());

    if (!data) return [];

    try {
      return JSON.parse(data) as CartItem[];
    } catch {
      return [];
    }
  }

  addProduct(product: Product): boolean {
    const items = this.getItems();
    const existingItem = items.find(item => item.product.id === product.id);
    const stock = product.stockQuantity ?? 0;

    if (stock <= 0) return false;

    if (existingItem) {
      if (existingItem.quantity >= stock || existingItem.quantity >= this.maxQuantityPerProduct) return false;
      existingItem.quantity++;
    } else {
      items.push({ product, quantity: 1 });
    }

    this.saveItems(items);
    return true;
  }

  updateQuantity(productId: string, quantity: number): boolean {
    const items = this.getItems();
    const item = items.find(cartItem => cartItem.product.id === productId);

    if (!item || quantity < 1 || quantity > this.maxQuantityPerProduct) return false;

    const stock = item.product.stockQuantity ?? 0;

    if (quantity > stock) return false;

    item.quantity = quantity;
    this.saveItems(items);

    return true;
  }

  removeProduct(productId: string): void {
    const items = this.getItems().filter(item => item.product.id !== productId);
    this.saveItems(items);
  }

  clearCart(): void {
    localStorage.removeItem(this.getStorageKey());
  }

  getTotal(): number {
    return this.getItems().reduce((total, item) => total + item.product.price * item.quantity, 0);
  }

  getItemCount(): number {
    return this.getItems().reduce((total, item) => total + item.quantity, 0);
  }

  private saveItems(items: CartItem[]): void {
    localStorage.setItem(this.getStorageKey(), JSON.stringify(items));
  }

  private getStorageKey(): string {
    const user = this.authService.getCurrentUser();

    if (!user) return `${this.baseStorageKey}-guest`;

    return `${this.baseStorageKey}-${user.userId}`;
  }
}
