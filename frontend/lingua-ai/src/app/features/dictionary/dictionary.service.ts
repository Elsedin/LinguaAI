import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { API_BASE_URL } from '../../core/api.config';
import {
  DictionaryEntry,
  DictionaryEntryRequest,
  DictionaryListQuery,
  PagedDictionary,
} from './dictionary.models';

@Injectable({ providedIn: 'root' })
export class DictionaryService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/api/dictionary`;
  private readonly credentials = { withCredentials: true };

  list(query: DictionaryListQuery = {}): Observable<PagedDictionary> {
    let params = new HttpParams()
      .set('page', query.page ?? 1)
      .set('pageSize', query.pageSize ?? 20);

    const search = query.search?.trim();
    if (search) {
      params = params.set('search', search);
    }

    const sourceLanguage = query.sourceLanguage?.trim();
    if (sourceLanguage) {
      params = params.set('sourceLanguage', sourceLanguage);
    }

    const targetLanguage = query.targetLanguage?.trim();
    if (targetLanguage) {
      params = params.set('targetLanguage', targetLanguage);
    }

    return this.http.get<PagedDictionary>(this.url, { ...this.credentials, params }).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error, 'Could not load dictionary.'))),
      ),
    );
  }

  create(request: DictionaryEntryRequest): Observable<DictionaryEntry> {
    return this.http.post<DictionaryEntry>(this.url, request, this.credentials).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error, 'Could not save dictionary entry.'))),
      ),
    );
  }

  update(id: string, request: DictionaryEntryRequest): Observable<DictionaryEntry> {
    return this.http.put<DictionaryEntry>(`${this.url}/${id}`, request, this.credentials).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error, 'Could not update dictionary entry.'))),
      ),
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`, this.credentials).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error, 'Could not delete dictionary entry.'))),
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
      return 'Please log in to manage your dictionary.';
    }

    if (error.status === 404) {
      return 'That dictionary entry was not found.';
    }

    return error.message || fallback;
  }
}
