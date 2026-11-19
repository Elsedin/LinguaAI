import { DatePipe } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import { DICTIONARY_LANGUAGES, DictionaryEntry } from './dictionary.models';
import { DictionaryService } from './dictionary.service';

@Component({
  selector: 'app-dictionary-page',
  imports: [DatePipe, FormsModule, RouterLink],
  templateUrl: './dictionary-page.html',
  styleUrl: './dictionary-page.scss',
})
export class DictionaryPage implements OnInit {
  private readonly dictionary = inject(DictionaryService);
  private readonly auth = inject(AuthService);

  protected readonly languages = DICTIONARY_LANGUAGES;
  protected readonly currentUser = this.auth.currentUser;
  protected readonly items = signal<DictionaryEntry[]>([]);
  protected readonly page = signal(1);
  protected readonly pageSize = 10;
  protected readonly totalCount = signal(0);
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly busyId = signal<string | null>(null);
  protected readonly editingId = signal<string | null>(null);
  protected readonly error = signal<string | null>(null);

  protected readonly term = signal('');
  protected readonly translation = signal('');
  protected readonly sourceLanguage = signal('en');
  protected readonly targetLanguage = signal('bs');
  protected readonly exampleSentence = signal('');
  protected readonly notes = signal('');

  protected readonly searchDraft = signal('');
  protected readonly activeSearch = signal('');
  protected readonly filterSourceLanguage = signal('');
  protected readonly filterTargetLanguage = signal('');

  protected readonly totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalCount() / this.pageSize)),
  );
  protected readonly isEditing = computed(() => this.editingId() !== null);
  protected readonly hasActiveFilters = computed(
    () =>
      this.activeSearch().length > 0 ||
      this.filterSourceLanguage().length > 0 ||
      this.filterTargetLanguage().length > 0,
  );
  protected readonly canSave = computed(
    () =>
      this.term().trim().length > 0 &&
      this.translation().trim().length > 0 &&
      !this.saving(),
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

  protected applyFilters(): void {
    this.activeSearch.set(this.searchDraft().trim());
    this.load(1);
  }

  protected clearFilters(): void {
    this.searchDraft.set('');
    this.activeSearch.set('');
    this.filterSourceLanguage.set('');
    this.filterTargetLanguage.set('');
    this.load(1);
  }

  protected load(page = this.page()): void {
    this.loading.set(true);
    this.error.set(null);
    this.page.set(page);

    this.dictionary
      .list({
        page,
        pageSize: this.pageSize,
        search: this.activeSearch(),
        sourceLanguage: this.filterSourceLanguage(),
        targetLanguage: this.filterTargetLanguage(),
      })
      .subscribe({
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

  protected startEdit(item: DictionaryEntry): void {
    this.editingId.set(item.id);
    this.term.set(item.term);
    this.translation.set(item.translation);
    this.sourceLanguage.set(item.sourceLanguage);
    this.targetLanguage.set(item.targetLanguage);
    this.exampleSentence.set(item.exampleSentence ?? '');
    this.notes.set(item.notes ?? '');
    this.error.set(null);
  }

  protected cancelEdit(): void {
    this.clearForm();
  }

  protected save(): void {
    if (!this.canSave()) {
      return;
    }

    this.saving.set(true);
    this.error.set(null);

    const example = this.exampleSentence().trim();
    const notes = this.notes().trim();
    const request = {
      term: this.term().trim(),
      translation: this.translation().trim(),
      sourceLanguage: this.sourceLanguage(),
      targetLanguage: this.targetLanguage(),
      exampleSentence: example.length > 0 ? example : null,
      notes: notes.length > 0 ? notes : null,
    };

    const editingId = this.editingId();
    const call = editingId
      ? this.dictionary.update(editingId, request)
      : this.dictionary.create(request);

    call.subscribe({
      next: (saved) => {
        if (editingId) {
          this.items.update((rows) =>
            rows.map((row) => (row.id === saved.id ? saved : row)),
          );
        } else {
          this.load(1);
        }
        this.clearForm();
        this.saving.set(false);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.saving.set(false);
      },
    });
  }

  protected deleteItem(item: DictionaryEntry): void {
    if (this.busyId() !== null) {
      return;
    }

    if (!window.confirm(`Delete “${item.term}” from your dictionary?`)) {
      return;
    }

    this.busyId.set(item.id);
    this.error.set(null);

    this.dictionary.delete(item.id).subscribe({
      next: () => {
        if (this.editingId() === item.id) {
          this.clearForm();
        }
        this.items.update((rows) => rows.filter((row) => row.id !== item.id));
        this.totalCount.update((count) => Math.max(0, count - 1));
        this.busyId.set(null);

        if (this.items().length === 0 && this.page() > 1) {
          this.load(this.page() - 1);
        }
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.busyId.set(null);
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

  private clearForm(): void {
    this.editingId.set(null);
    this.term.set('');
    this.translation.set('');
    this.exampleSentence.set('');
    this.notes.set('');
    this.sourceLanguage.set('en');
    this.targetLanguage.set('bs');
  }
}
