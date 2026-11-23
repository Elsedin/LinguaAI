import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../auth/auth.service';
import {
  SOURCE_LANGUAGES,
  TARGET_LANGUAGES,
  TRANSLATION_STYLES,
} from '../translator/translation.models';
import { PreferencesService } from './preferences.service';

@Component({
  selector: 'app-settings-page',
  imports: [FormsModule, RouterLink],
  templateUrl: './settings-page.html',
  styleUrl: './settings-page.scss',
})
export class SettingsPage implements OnInit {
  private readonly preferences = inject(PreferencesService);
  private readonly auth = inject(AuthService);

  protected readonly sourceLanguages = SOURCE_LANGUAGES;
  protected readonly targetLanguages = TARGET_LANGUAGES;
  protected readonly styles = TRANSLATION_STYLES;
  protected readonly currentUser = this.auth.currentUser;

  protected readonly sourceLanguage = signal('');
  protected readonly targetLanguage = signal('');
  protected readonly style = signal('');
  protected readonly loading = signal(false);
  protected readonly saving = signal(false);
  protected readonly saved = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly canSave = computed(() => !this.loading() && !this.saving());

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
    this.saved.set(false);

    this.preferences.get().subscribe({
      next: (prefs) => {
        this.sourceLanguage.set(prefs.preferredSourceLanguage ?? '');
        this.targetLanguage.set(prefs.preferredTargetLanguage ?? '');
        this.style.set(prefs.preferredStyle ?? '');
        this.loading.set(false);
      },
      error: (err: Error) => {
        this.error.set(err.message);
        this.loading.set(false);
      },
    });
  }

  protected save(): void {
    if (!this.canSave()) {
      return;
    }

    this.saving.set(true);
    this.error.set(null);
    this.saved.set(false);

    this.preferences
      .save({
        preferredSourceLanguage: this.normalizeOptional(this.sourceLanguage()),
        preferredTargetLanguage: this.normalizeOptional(this.targetLanguage()),
        preferredStyle: this.normalizeOptional(this.style()),
      })
      .subscribe({
        next: (prefs) => {
          this.sourceLanguage.set(prefs.preferredSourceLanguage ?? '');
          this.targetLanguage.set(prefs.preferredTargetLanguage ?? '');
          this.style.set(prefs.preferredStyle ?? '');
          this.saving.set(false);
          this.saved.set(true);
          window.setTimeout(() => this.saved.set(false), 1800);
        },
        error: (err: Error) => {
          this.error.set(err.message);
          this.saving.set(false);
        },
      });
  }

  private normalizeOptional(value: string): string | null {
    const trimmed = value.trim();
    return trimmed.length > 0 ? trimmed : null;
  }
}
