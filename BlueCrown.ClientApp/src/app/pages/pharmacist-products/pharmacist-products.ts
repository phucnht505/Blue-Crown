import { CurrencyPipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Medication } from '../../models/medication.model';
import { Product, ProductRequest } from '../../models/product.model';
import { MedicationService } from '../../services/medication.service';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-pharmacist-products',
  standalone: true,
  imports: [CurrencyPipe, FormsModule, RouterLink],
  templateUrl: './pharmacist-products.html',
  styleUrl: './pharmacist-products.css',
})
export class PharmacistProducts implements OnInit {
  private readonly productService = inject(ProductService);
  private readonly medicationService = inject(MedicationService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  products: Product[] = [];
  medications: Medication[] = [];
  editingId: string | null = null;
  editingStock = 0;
  medicationId = '';
  name = '';
  description = '';
  price: number | null = null;
  isPrescriptionRequired = false;
  activeIngredient = '';
  therapeuticGroup = '';
  dosageForm = '';
  strength = '';
  imageUrl = '';
  searchTerm = '';
  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';
  fieldErrors: Record<string, string> = {};

  ngOnInit(): void {
    this.loadData();
  }

  get filteredProducts(): Product[] {
    const keyword = this.searchTerm.trim().toLowerCase();
    if (!keyword) return this.products;

    return this.products.filter(product =>
      product.name.toLowerCase().includes(keyword) ||
      product.medicationName?.toLowerCase().includes(keyword) ||
      product.activeIngredient?.toLowerCase().includes(keyword)
    );
  }

  save(): void {
    this.clearMessages();
    this.fieldErrors = {};

    if (!this.validateForm()) {
      this.errorMessage = 'Vui lòng kiểm tra lại thông tin Product.';
      return;
    }

    const request: ProductRequest = {
      medicationId: this.medicationId || null,
      name: this.name.trim(),
      description: this.description.trim() || null,
      price: Number(this.price),
      isPrescriptionRequired: this.isPrescriptionRequired,
      activeIngredient: this.activeIngredient.trim() || null,
      therapeuticGroup: this.therapeuticGroup.trim() || null,
      dosageForm: this.dosageForm.trim() || null,
      strength: this.strength.trim() || null,
      imageUrl: this.imageUrl.trim() || null,
    };

    this.isSaving = true;

    if (this.editingId) {
      this.productService.update(this.editingId, request).subscribe({
        next: response => {
          this.successMessage = response.message;
          this.resetForm();
          this.loadProducts(false);
        },
        error: error => {
          this.errorMessage = this.getApiErrorMessage(error);
          this.isSaving = false;
          this.changeDetectorRef.detectChanges();
        },
      });
      return;
    }

    this.productService.create(request).subscribe({
      next: response => {
        this.successMessage = `${response.message} Tồn kho ban đầu = 0.`;
        this.resetForm();
        this.loadProducts(false);
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  edit(product: Product): void {
    this.clearMessages();
    this.fieldErrors = {};
    this.editingId = product.id;
    this.editingStock = product.stockQuantity ?? 0;
    this.medicationId = product.medicationId ?? '';
    this.name = product.name;
    this.description = product.description ?? '';
    this.price = product.price;
    this.isPrescriptionRequired = product.isPrescriptionRequired ?? false;
    this.activeIngredient = product.activeIngredient ?? '';
    this.therapeuticGroup = product.therapeuticGroup ?? '';
    this.dosageForm = product.dosageForm ?? '';
    this.strength = product.strength ?? '';
    this.imageUrl = product.imageUrl ?? '';
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit(): void {
    this.resetForm();
    this.clearMessages();
  }

  deleteProduct(product: Product): void {
    this.clearMessages();

    if (!window.confirm(`Xóa Product "${product.name}"?`)) return;

    this.productService.delete(product.id).subscribe({
      next: response => {
        this.products = this.products.filter(item => item.id !== product.id);
        this.successMessage = response.message;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private validateForm(): boolean {
    const name = this.name.trim();

    if (!name) this.fieldErrors['name'] = 'Tên Product không được để trống.';
    else if (name.length < 2 || name.length > 100) this.fieldErrors['name'] = 'Tên Product phải từ 2 đến 100 ký tự.';
    else if (!/[A-Za-zÀ-ỹ]/u.test(name)) this.fieldErrors['name'] = 'Tên Product phải chứa ít nhất một chữ cái.';

    if (this.price == null || !Number.isFinite(Number(this.price)) || Number(this.price) <= 0)
      this.fieldErrors['price'] = 'Giá bán phải lớn hơn 0.';

    if (this.description.trim().length > 500) this.fieldErrors['description'] = 'Mô tả tối đa 500 ký tự.';
    if (this.activeIngredient.trim().length > 100) this.fieldErrors['activeIngredient'] = 'Hoạt chất tối đa 100 ký tự.';
    if (this.therapeuticGroup.trim().length > 100) this.fieldErrors['therapeuticGroup'] = 'Nhóm điều trị tối đa 100 ký tự.';
    if (this.dosageForm.trim().length > 50) this.fieldErrors['dosageForm'] = 'Dạng bào chế tối đa 50 ký tự.';
    if (this.strength.trim().length > 50) this.fieldErrors['strength'] = 'Hàm lượng tối đa 50 ký tự.';
    if (this.imageUrl.trim().length > 500) this.fieldErrors['imageUrl'] = 'Đường dẫn hình ảnh tối đa 500 ký tự.';

    return Object.keys(this.fieldErrors).length === 0;
  }

  private loadData(): void {
    forkJoin({
      products: this.productService.getAll(),
      medications: this.medicationService.getAll(),
    }).subscribe({
      next: result => {
        this.products = result.products;
        this.medications = result.medications;
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

  private loadProducts(showLoading = true): void {
    if (showLoading) this.isLoading = true;

    this.productService.getAll().subscribe({
      next: products => {
        this.products = products;
        this.isLoading = false;
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private resetForm(): void {
    this.editingId = null;
    this.editingStock = 0;
    this.medicationId = '';
    this.name = '';
    this.description = '';
    this.price = null;
    this.isPrescriptionRequired = false;
    this.activeIngredient = '';
    this.therapeuticGroup = '';
    this.dosageForm = '';
    this.strength = '';
    this.imageUrl = '';
    this.fieldErrors = {};
  }

  private clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private getApiErrorMessage(error: any): string {
    if (error?.error?.message) return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();
      if (errors.length > 0) return String(errors[0]);
    }

    return 'Không thể xử lý Product. Vui lòng thử lại.';
  }
}
