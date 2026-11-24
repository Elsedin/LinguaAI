import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { API_BASE_URL } from '../../core/api.config';
import { DashboardSummary } from './dashboard.models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/api/dashboard`;
  private readonly credentials = { withCredentials: true };

  getSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(this.url, this.credentials).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error))),
      ),
    );
  }

  private readError(error: HttpErrorResponse): string {
    const detail = error.error?.detail;
    if (typeof detail === 'string' && detail.trim().length > 0) {
      return detail;
    }

    if (error.status === 0) {
      return 'Cannot reach the API. Is LinguaAI.Api running?';
    }

    if (error.status === 401) {
      return 'Please log in to view your dashboard.';
    }

    return error.message || 'Could not load dashboard.';
  }
}
