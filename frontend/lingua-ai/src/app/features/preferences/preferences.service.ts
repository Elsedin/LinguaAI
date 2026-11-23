import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { API_BASE_URL } from '../../core/api.config';
import { UpdatePreferencesRequest, UserPreferences } from './preferences.models';

@Injectable({ providedIn: 'root' })
export class PreferencesService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/api/preferences`;
  private readonly credentials = { withCredentials: true };

  get(): Observable<UserPreferences> {
    return this.http.get<UserPreferences>(this.url, this.credentials).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error, 'Could not load preferences.'))),
      ),
    );
  }

  save(request: UpdatePreferencesRequest): Observable<UserPreferences> {
    return this.http.put<UserPreferences>(this.url, request, this.credentials).pipe(
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error, 'Could not save preferences.'))),
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
      return 'Please log in to manage your preferences.';
    }

    return error.message || fallback;
  }
}
