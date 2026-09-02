import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { EcommerceOrder, GuestOrderLookupRequest } from '../models/order.model';

export type OrderStatusUpdate = 'confirmed' | 'shipped' | 'delivered' | 'cancelled';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/EcommerceOrder';

  lookupGuestOrders(request: GuestOrderLookupRequest): Observable<EcommerceOrder[]> {
    return this.http.post<EcommerceOrder[]>(`${this.apiUrl}/lookup`, request);
  }

  getMyOrders(): Observable<EcommerceOrder[]> {
    return this.http.get<EcommerceOrder[]>(`${this.apiUrl}/my`);
  }

  getMyOrderById(id: string): Observable<EcommerceOrder> {
    return this.http.get<EcommerceOrder>(`${this.apiUrl}/my/${id}`);
  }

  cancelMyOrder(id: string): Observable<EcommerceOrder> {
    return this.http.put<EcommerceOrder>(`${this.apiUrl}/my/${id}/cancel`, {});
  }

  getManagementOrders(): Observable<EcommerceOrder[]> {
    return this.http.get<EcommerceOrder[]>(`${this.apiUrl}/manage`);
  }

  getManagementOrderById(id: string): Observable<EcommerceOrder> {
    return this.http.get<EcommerceOrder>(`${this.apiUrl}/manage/${id}`);
  }

  updateStatus(id: string, status: OrderStatusUpdate): Observable<EcommerceOrder> {
    return this.http.put<EcommerceOrder>(`${this.apiUrl}/manage/${id}/status`, { status });
  }
}

