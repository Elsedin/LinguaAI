import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { ActivityDay, DashboardSummary } from './dashboard.models';
import { DashboardService } from './dashboard.service';

@Component({
  selector: 'app-dashboard-page',
  imports: [DatePipe, RouterLink],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.scss',
})
export class DashboardPage implements OnInit {
  private readonly dashboard = inject(DashboardService);
  private readonly auth = inject(AuthService);

  protected readonly currentUser = this.auth.currentUser;
  protected readonly summary = signal<DashboardSummary | null>(null);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly maxActivity = computed(() => {
    const days = this.summary()?.activityByDay ?? [];
    return Math.max(1, ...days.map((day) => day.count));
  });

  ngOnInit(): void {
    if (this.currentUser()) {
      this.load();
      return;
    }

    this.auth.loadCurrentUser().subscribe({
      next: (user) => {
        if (user) {
          this.load();
        }
      },
      error: (err: Error) => this.error.set(err.message),
    });
  }

  protected load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.dashboard.getSummary().subscribe({
      next: (data) => {
        this.summary.set(data);
        this.loading.set(false);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.loading.set(false);
      },
    });
  }

  protected barHeight(day: ActivityDay): number {
    return Math.round((day.count / this.maxActivity()) * 100);
  }
}
