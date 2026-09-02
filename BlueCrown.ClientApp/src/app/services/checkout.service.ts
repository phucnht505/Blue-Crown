import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CheckoutResponse, CreateCheckoutRequest } from '../models/checkout.model';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/EcommerceOrder';

  create(request: CreateCheckoutRequest): Observable<CheckoutResponse> {
    return this.http.post<CheckoutResponse>(this.apiUrl, request);
  }
}

