import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { API_BASE_URL } from '../../core/api.config';
import { HistoryItem, HistoryListQuery, PagedHistory } from './history.models';

@Injectable({ providedIn: 'root' })
export class HistoryService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/api/history`;
  private readonly credentials = { withCredentials: true };

  list(query: HistoryListQuery = {}): Observable<PagedHistory> {
    let params = new HttpParams()
      .set('page', query.page ?? 1)
      .set('pageSize', query.pageSize ?? 20);

    const search = query.search?.trim();
    if (search) {
      params = params.set('search', search);
    }

    if (query.favoritesOnly) {
      params = params.set('favoritesOnly', 'true');
    }

    return this.http.get<PagedHistory>(this.url, { ...this.credentials, params }).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error, 'Could not load history.'))),
      ),
    );
  }

  toggleFavorite(id: string): Observable<HistoryItem> {
    return this.http
      .post<HistoryItem>(`${this.url}/${id}/favorite`, {}, this.credentials)
      .pipe(
        catchError((error: HttpErrorResponse) =>
          throwError(() => new Error(this.readError(error, 'Could not update favorite.'))),
        ),
      );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`, this.credentials).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error, 'Could not delete history item.'))),
      ),
    );
  }

  private readError(error: HttpErrorResponse, fallback: string): string {
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

    if (error.status === 404) {
      return 'That history item was not found.';
    }

    return error.message || fallback;
  }
}
