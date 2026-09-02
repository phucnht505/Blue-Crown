import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateInventoryReceiptRequest, InventoryReceipt } from '../models/inventory-receipt.model';

@Injectable({
  providedIn: 'root',
})
export class InventoryReceiptService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/InventoryReceipt';

  getAll(): Observable<InventoryReceipt[]> {
    return this.http.get<InventoryReceipt[]>(this.apiUrl);
  }

  getById(id: string): Observable<InventoryReceipt> {
    return this.http.get<InventoryReceipt>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateInventoryReceiptRequest): Observable<{ message: string }> {
    return this.http.post<{ message: string }>(this.apiUrl, request);
  }

  approve(id: string): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}/approve`, {});
  }

  reject(id: string): Observable<{ message: string }> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${id}/reject`, {});
  }
}

