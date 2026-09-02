import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { DrugAlternative, Product, ProductRequest } from '../models/product.model';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/Product';
  private readonly drugAlternativeUrl = '/api/DrugAlternative';

  getAll(): Observable<Product[]> {
    return this.http.get<Product[]>(this.apiUrl);
  }

  getById(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/${id}`);
  }

  getByMedicationId(medicationId: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/by-medication/${medicationId}`);
  }

  search(keyword: string): Observable<Product[]> {
    const params = new HttpParams().set('keyword', keyword);
    return this.http.get<Product[]>(`${this.apiUrl}/search`, { params });
  }

  getAlternatives(productId: string): Observable<DrugAlternative[]> {
    return this.http.get<DrugAlternative[]>(`${this.drugAlternativeUrl}/product/${productId}`);
  }

  create(request: ProductRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(this.apiUrl, request);
  }

  update(id: string, request: ProductRequest): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: string): Observable<{ message: string }> {
    return this.http.delete<{ message: string }>(`${this.apiUrl}/${id}`);
  }
}

