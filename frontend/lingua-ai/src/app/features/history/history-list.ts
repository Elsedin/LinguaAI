import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { HistoryItem } from './history.models';
import { HistoryService } from './history.service';

@Component({
  selector: 'app-history-list',
  imports: [DatePipe, RouterLink],
  templateUrl: './history-list.html',
  styleUrl: './history-list.scss',
})
export class HistoryList implements OnInit {
  private readonly history = inject(HistoryService);
  private readonly auth = inject(AuthService);

  protected readonly currentUser = this.auth.currentUser;
  protected readonly items = signal<HistoryItem[]>([]);
  protected readonly page = signal(1);
  protected readonly pageSize = 10;
  protected readonly totalCount = signal(0);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalCount() / this.pageSize)),
  );

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

  protected load(page = this.page()): void {
    this.loading.set(true);
    this.error.set(null);
    this.page.set(page);

    this.history.list(page, this.pageSize).subscribe({
      next: (result) => {
        this.items.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.loading.set(false);
      },
    });
  }

  protected previousPage(): void {
    if (this.page() > 1) {
      this.load(this.page() - 1);
    }
  }

  protected nextPage(): void {
    if (this.page() < this.totalPages()) {
      this.load(this.page() + 1);
    }
  }
}
