import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ReceiptDetail } from '../../models/inventory-receipt.model';
import { Product } from '../../models/product.model';
import { FefoService } from '../../services/fefo.service';
import { ProductService } from '../../services/product.service';

type StockFilter = 'all' | 'in_stock' | 'low_stock' | 'out_of_stock';

@Component({
  selector: 'app-pharmacist-stock',
  standalone: true,
  imports: [CurrencyPipe, DatePipe, FormsModule, RouterLink],
  templateUrl: './pharmacist-stock.html',
  styleUrl: './pharmacist-stock.css',
})
export class PharmacistStock implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly fefoService = inject(FefoService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  products: Product[] = [];
  searchTerm = '';
  stockFilter: StockFilter = 'all';
  isLoading = true;
  errorMessage = '';

  selectedFefoProduct: Product | null = null;
  fefoDetails: ReceiptDetail[] = [];
  isLoadingFefo = false;
  fefoErrorMessage = '';

  ngOnInit(): void {
    this.loadProducts();
  }

  get filteredProducts(): Product[] {
    const keyword = this.searchTerm.trim().toLowerCase();

    return this.products.filter(product => {
      const matchesKeyword = !keyword ||
        product.name.toLowerCase().includes(keyword) ||
        product.medicationName?.toLowerCase().includes(keyword) ||
        product.medicationGenericName?.toLowerCase().includes(keyword) ||
        product.activeIngredient?.toLowerCase().includes(keyword);

      if (!matchesKeyword) return false;

      const stock = product.stockQuantity ?? 0;

      if (this.stockFilter === 'out_of_stock') return stock === 0;
      if (this.stockFilter === 'low_stock') return stock > 0 && stock <= 10;
      if (this.stockFilter === 'in_stock') return stock > 10;

      return true;
    });
  }

  get totalProducts(): number {
    return this.products.length;
  }

  get inStockCount(): number {
    return this.products.filter(product => (product.stockQuantity ?? 0) > 10).length;
  }

  get lowStockCount(): number {
    return this.products.filter(product => {
      const stock = product.stockQuantity ?? 0;
      return stock > 0 && stock <= 10;
    }).length;
  }

  get outOfStockCount(): number {
    return this.products.filter(product => (product.stockQuantity ?? 0) === 0).length;
  }

  setStockFilter(filter: StockFilter): void {
    this.stockFilter = filter;
  }

  clearSearch(): void {
    this.searchTerm = '';
  }

  viewFefo(product: Product): void {
    this.selectedFefoProduct = product;
    this.fefoDetails = [];
    this.fefoErrorMessage = '';
    this.isLoadingFefo = true;

    this.fefoService.getByProduct(product.id).subscribe({
      next: details => {
        this.fefoDetails = details;
        this.isLoadingFefo = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.fefoErrorMessage = this.getApiErrorMessage(error);
        this.isLoadingFefo = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  closeFefo(): void {
    this.selectedFefoProduct = null;
    this.fefoDetails = [];
    this.fefoErrorMessage = '';
    this.isLoadingFefo = false;
  }

  getStockText(stockQuantity: number | null): string {
    const stock = stockQuantity ?? 0;

    if (stock < 0) return 'Dữ liệu không hợp lệ';
    if (stock === 0) return 'Hết hàng';
    if (stock <= 10) return 'Sắp hết hàng';

    return 'Còn hàng';
  }

  getStockClass(stockQuantity: number | null): string {
    const stock = stockQuantity ?? 0;

    if (stock < 0) return 'status-invalid';
    if (stock === 0) return 'status-out';
    if (stock <= 10) return 'status-low';

    return 'status-ok';
  }

  getFefoPriority(index: number): string {
    if (index === 0) return 'Ưu tiên xuất trước';
    return `Ưu tiên ${index + 1}`;
  }

  private loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.productService.getAll().subscribe({
      next: products => {
        this.products = [...products].sort((a, b) => {
          const stockA = a.stockQuantity ?? 0;
          const stockB = b.stockQuantity ?? 0;

          if (stockA !== stockB) return stockA - stockB;
          return a.name.localeCompare(b.name);
        });

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

  private getApiErrorMessage(error: any): string {
    if (error?.error?.message) return error.error.message;
    return 'Không thể tải dữ liệu tồn kho. Vui lòng thử lại.';
  }
}
