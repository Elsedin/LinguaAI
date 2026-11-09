import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { API_BASE_URL } from '../../core/api.config';
import { TranslateRequest, TranslateResponse } from './translation.models';

@Injectable({ providedIn: 'root' })
export class TranslationService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/api/translations`;

  translate(request: TranslateRequest): Observable<TranslateResponse> {
    return this.http
      .post<TranslateResponse>(this.url, request, { withCredentials: true })
      .pipe(
        catchError((error: HttpErrorResponse) =>
          throwError(() => new Error(this.readErrorMessage(error))),
        ),
      );
  }

  private readErrorMessage(error: HttpErrorResponse): string {
    const detail = error.error?.detail;
    if (typeof detail === 'string' && detail.trim().length > 0) {
      return detail;
    }

    if (error.status === 0) {
      return 'Cannot reach the API. Is LinguaAI.Api running?';
    }

    if (error.status === 429) {
      return 'Too many translation requests. Please wait a moment and try again.';
    }

    return error.message || 'Translation failed.';
  }
}
