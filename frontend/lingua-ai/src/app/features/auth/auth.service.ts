import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject, signal } from '@angular/core';
import { Observable, catchError, of, tap, throwError } from 'rxjs';
import { API_BASE_URL } from '../../core/api.config';
import { AuthUser, LoginRequest, RegisterRequest } from './auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly url = `${API_BASE_URL}/api/auth`;
  private readonly credentials = { withCredentials: true };

  readonly currentUser = signal<AuthUser | null>(null);

  register(request: RegisterRequest): Observable<AuthUser> {
    return this.http.post<AuthUser>(`${this.url}/register`, request, this.credentials).pipe(
      tap((user) => this.currentUser.set(user)),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error))),
      ),
    );
  }

  login(request: LoginRequest): Observable<AuthUser> {
    return this.http.post<AuthUser>(`${this.url}/login`, request, this.credentials).pipe(
      tap((user) => this.currentUser.set(user)),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error))),
      ),
    );
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.url}/logout`, {}, this.credentials).pipe(
      tap(() => this.currentUser.set(null)),
      catchError((error: HttpErrorResponse) =>
        throwError(() => new Error(this.readError(error))),
      ),
    );
  }

  loadCurrentUser(): Observable<AuthUser | null> {
    return this.http.get<AuthUser>(`${this.url}/me`, this.credentials).pipe(
      tap((user) => this.currentUser.set(user)),
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          this.currentUser.set(null);
          return of(null);
        }

        return throwError(() => new Error(this.readError(error)));
      }),
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

    return error.message || 'Authentication failed.';
  }
}
