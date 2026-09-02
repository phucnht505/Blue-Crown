import { DecimalPipe } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { catchError, forkJoin, of } from 'rxjs';
import { DrugAlternative, Product } from '../../models/product.model';
import { CartService } from '../../services/cart.service';
import { ProductService } from '../../services/product.service';

interface ProductAlternativeView {
  relation: DrugAlternative;
  product: Product;
}

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [DecimalPipe, RouterLink],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.css',
})
export class ProductDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly productService = inject(ProductService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);
  private readonly cartService = inject(CartService);

  product: Product | null = null;
  alternatives: ProductAlternativeView[] = [];
  isLoading = false;
  isLoadingAlternatives = false;
  errorMessage = '';
  alternativeMessage = '';
  cartMessage = '';

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');

      if (!id) {
        this.product = null;
        this.errorMessage = 'Không tìm thấy mã sản phẩm.';
        return;
      }

      this.loadProduct(id);
    });
  }

  addToCart(): void {
    if (!this.product || (this.product.stockQuantity ?? 0) <= 0)
      return;

    const added = this.cartService.addProduct(this.product);

    this.cartMessage = added
      ? 'Đã thêm sản phẩm vào giỏ hàng.'
      : 'Không thể thêm sản phẩm vì đã đạt số lượng tồn kho.';
  }

  private loadProduct(id: string): void {
    this.isLoading = true;
    this.isLoadingAlternatives = false;
    this.errorMessage = '';
    this.alternativeMessage = '';
    this.cartMessage = '';
    this.alternatives = [];
    this.product = null;

    this.productService.getById(id).subscribe({
      next: product => {
        this.product = product;
        this.isLoading = false;

        if ((product.stockQuantity ?? 0) <= 0)
          this.loadAlternatives(product.id);

        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        console.error('Lỗi tải chi tiết sản phẩm:', error);
        this.errorMessage = 'Không thể tải thông tin sản phẩm.';
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private loadAlternatives(productId: string): void {
    this.isLoadingAlternatives = true;
    this.alternativeMessage = '';

    this.productService.getAlternatives(productId).subscribe({
      next: relations => {
        if (relations.length === 0) {
          this.alternatives = [];
          this.alternativeMessage = 'Hiện chưa có sản phẩm thay thế phù hợp.';
          this.isLoadingAlternatives = false;
          this.changeDetectorRef.detectChanges();
          return;
        }

        const requests = relations.map(relation =>
          this.productService.getById(relation.alternativeProductId).pipe(
            catchError(() => of(null))
          )
        );

        forkJoin(requests).subscribe({
          next: products => {
            this.alternatives = [];

            relations.forEach((relation, index) => {
              const product = products[index];

              // UC06: Chỉ gợi ý sản phẩm thay thế hiện còn hàng.
              if (product && (product.stockQuantity ?? 0) > 0)
                this.alternatives.push({ relation, product });
            });

            if (this.alternatives.length === 0)
              this.alternativeMessage = 'Hiện chưa có sản phẩm thay thế còn hàng.';

            this.isLoadingAlternatives = false;
            this.changeDetectorRef.detectChanges();
          },
          error: error => {
            console.error('Lỗi tải chi tiết sản phẩm thay thế:', error);
            this.alternativeMessage = 'Không thể tải danh sách sản phẩm thay thế.';
            this.isLoadingAlternatives = false;
            this.changeDetectorRef.detectChanges();
          },
        });
      },
      error: error => {
        console.error('Lỗi tải sản phẩm thay thế:', error);
        this.alternativeMessage = 'Không thể tải danh sách sản phẩm thay thế.';
        this.isLoadingAlternatives = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }
}
