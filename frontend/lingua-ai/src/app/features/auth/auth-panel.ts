import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from './auth.service';

@Component({
  selector: 'app-auth-panel',
  imports: [FormsModule, RouterLink],
  templateUrl: './auth-panel.html',
  styleUrl: './auth-panel.scss',
})
export class AuthPanel implements OnInit {
  private readonly auth = inject(AuthService);

  protected readonly currentUser = this.auth.currentUser;
  protected readonly mode = signal<'login' | 'register'>('login');
  protected readonly email = signal('');
  protected readonly password = signal('');
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  ngOnInit(): void {
    this.auth.loadCurrentUser().subscribe();
  }

  protected setMode(next: 'login' | 'register'): void {
    this.mode.set(next);
    this.error.set(null);
  }

  protected submit(): void {
    if (this.loading() || this.email().trim().length === 0 || this.password().length === 0) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const request = { email: this.email().trim(), password: this.password() };
    const call =
      this.mode() === 'login' ? this.auth.login(request) : this.auth.register(request);

    call.subscribe({
      next: () => {
        this.password.set('');
        this.loading.set(false);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.loading.set(false);
      },
    });
  }

  protected logout(): void {
    this.loading.set(true);
    this.error.set(null);
    this.auth.logout().subscribe({
      next: () => this.loading.set(false),
      error: (err: Error) => {
        this.error.set(err.message);
        this.loading.set(false);
      },
    });
  }
}
