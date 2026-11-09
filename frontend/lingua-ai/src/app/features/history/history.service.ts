import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { API_BASE_URL } from '../../core/api.config';
import { PagedHistory } from './history.models';

@Injectable({ providedIn: 'root' })
export class HistoryService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/api/history`;
  private readonly credentials = { withCredentials: true };

  list(page = 1, pageSize = 20): Observable<PagedHistory> {
    const params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);

    return this.http.get<PagedHistory>(this.url, { ...this.credentials, params }).pipe(
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
      return 'Please log in to view your translation history.';
    }

    return error.message || 'Could not load history.';
  }
}
