import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ReceiptDetail } from '../models/inventory-receipt.model';

@Injectable({
  providedIn: 'root',
})
export class FefoService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/Fefo';

  getByProduct(productId: string): Observable<ReceiptDetail[]> {
    return this.http.get<ReceiptDetail[]>(`${this.apiUrl}/${productId}`);
  }
}

