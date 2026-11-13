import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { API_BASE_URL } from '../../core/api.config';
import { WritingRequest, WritingResponse } from './writing.models';

@Injectable({ providedIn: 'root' })
export class WritingService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/api/writing`;

  transform(request: WritingRequest): Observable<WritingResponse> {
    return this.http.post<WritingResponse>(this.url, request).pipe(
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

    if (error.status === 429) {
      return 'Too many writing requests. Please wait a moment and try again.';
    }

    return error.message || 'Writing request failed.';
  }
}
