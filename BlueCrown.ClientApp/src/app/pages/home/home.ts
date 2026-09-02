import {
  ChangeDetectorRef,
  Component,
  inject,
  OnInit,
} from '@angular/core';
import {
  RouterLink,
} from '@angular/router';

import {
  ProductCard,
} from '../../shared/product-card/product-card';
import {
  Product,
} from '../../models/product.model';
import {
  ProductService,
} from '../../services/product.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    RouterLink,
    ProductCard,
  ],
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home implements OnInit {
  private readonly productService =
    inject(ProductService);
  private readonly changeDetectorRef =
    inject(ChangeDetectorRef);

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
          console.log(
            'Product từ Backend:',
            products,
          );

          this.products =
            products.slice(0, 4);

          this.isLoading = false;

          this.changeDetectorRef.detectChanges();
        },

        error: (error) => {
          console.error(
            'Lỗi tải sản phẩm:',
            error,
          );

          this.errorMessage =
            'Không thể tải danh sách sản phẩm.';

          this.isLoading = false;

          this.changeDetectorRef.detectChanges();
        },
      });
  }
}
