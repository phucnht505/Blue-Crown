import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, timeout } from 'rxjs';
import { AdminStatistics, StatisticsPeriod } from '../models/admin-statistics.model';

@Injectable({
    providedIn: 'root',
})
export class AdminStatisticsService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = '/api/admin-statistics';

    getStatistics(
        period: StatisticsPeriod,
        date?: string,
        month?: number,
        year?: number,
    ): Observable<AdminStatistics> {
        let params = new HttpParams().set('period', period);

        if (period === 'day' && date) {
            params = params.set('date', date);
        }

        if (period === 'month') {
            if (month !== undefined) {
                params = params.set('month', month);
            }

            if (year !== undefined) {
                params = params.set('year', year);
            }
        }

        if (period === 'year' && year !== undefined) {
            params = params.set('year', year);
        }

        return this.http
            .get<AdminStatistics>(this.apiUrl, { params })
            .pipe(timeout(5000));
    }
}