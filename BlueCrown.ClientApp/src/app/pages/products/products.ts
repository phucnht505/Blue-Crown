import {
  ChangeDetectorRef,
  Component,
  inject,
  OnInit,
} from '@angular/core';
import { FormsModule } from '@angular/forms';

import { Product } from '../../models/product.model';
import { ProductService } from '../../services/product.service';
import { ProductCard } from '../../shared/product-card/product-card';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    FormsModule,
    ProductCard,
  ],
  templateUrl: './products.html',
  styleUrl: './products.css',
})
export class Products implements OnInit {
  private readonly productService =
    inject(ProductService);

  private readonly changeDetectorRef =
    inject(ChangeDetectorRef);

  searchText = '';

  products: Product[] = [];

  isLoading = false;

  errorMessage = '';

  ngOnInit(): void {
    this.loadProducts();
  }

  private loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.productService
      .getAll()
      .subscribe({
        next: (products) => {
          this.products = products;

          this.isLoading = false;

          this.changeDetectorRef.detectChanges();
        },

        error: (error) => {
          console.error(
            'Lỗi tải danh sách sản phẩm:',
            error,
          );

          this.errorMessage =
            'Không thể tải danh sách sản phẩm.';

          this.isLoading = false;

          this.changeDetectorRef.detectChanges();
        },
      });
  }

  onSearch(): void {
    const keyword = this.searchText.trim();

    if (!keyword) {
      this.loadProducts();
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    this.productService
      .search(keyword)
      .subscribe({
        next: (products) => {
          this.products = products;

          this.isLoading = false;

          this.changeDetectorRef.detectChanges();
        },

        error: (error) => {
          console.error(
            'Lỗi tìm kiếm sản phẩm:',
            error,
          );

          this.errorMessage =
            'Không thể tìm kiếm sản phẩm.';

          this.isLoading = false;

          this.changeDetectorRef.detectChanges();
        },
      });
  }
}
