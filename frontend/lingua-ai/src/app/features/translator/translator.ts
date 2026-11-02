import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslationService } from './translation.service';
import {
  MAX_TEXT_LENGTH,
  SOURCE_LANGUAGES,
  TARGET_LANGUAGES,
  TRANSLATION_STYLES,
} from './translation.models';

@Component({
  selector: 'app-translator',
  imports: [FormsModule],
  templateUrl: './translator.html',
  styleUrl: './translator.scss',
})
export class Translator {
  private readonly translationService = inject(TranslationService);

  protected readonly sourceLanguages = SOURCE_LANGUAGES;
  protected readonly targetLanguages = TARGET_LANGUAGES;
  protected readonly styles = TRANSLATION_STYLES;
  protected readonly maxLength = MAX_TEXT_LENGTH;

  protected readonly sourceText = signal('');
  protected readonly translatedText = signal('');
  protected readonly sourceLanguage = signal('auto');
  protected readonly targetLanguage = signal('bs');
  protected readonly style = signal('natural');
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly copied = signal(false);

  protected readonly characterCount = computed(() => this.sourceText().length);
  protected readonly canTranslate = computed(
    () => this.sourceText().trim().length > 0 && !this.loading(),
  );
  protected readonly canSwap = computed(() => this.sourceLanguage() !== 'auto');

  protected translate(): void {
    if (!this.canTranslate()) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.copied.set(false);

    this.translationService
      .translate({
        text: this.sourceText().trim(),
        sourceLanguage: this.sourceLanguage(),
        targetLanguage: this.targetLanguage(),
        style: this.style(),
      })
      .subscribe({
        next: (response) => {
          this.translatedText.set(response.translatedText);
          if (this.sourceLanguage() === 'auto' && response.sourceLanguage !== 'auto') {
            this.sourceLanguage.set(response.sourceLanguage);
          }
          this.loading.set(false);
        },
        error: (err: Error) => {
          this.error.set(err.message);
          this.loading.set(false);
        },
      });
  }

  protected swapLanguages(): void {
    if (!this.canSwap()) {
      return;
    }

    const previousSource = this.sourceLanguage();
    this.sourceLanguage.set(this.targetLanguage());
    this.targetLanguage.set(previousSource);

    const previousText = this.sourceText();
    this.sourceText.set(this.translatedText());
    this.translatedText.set(previousText);
    this.error.set(null);
  }

  protected clearAll(): void {
    this.sourceText.set('');
    this.translatedText.set('');
    this.error.set(null);
    this.copied.set(false);
  }

  protected async copyTranslation(): Promise<void> {
    const text = this.translatedText();
    if (!text) {
      return;
    }

    try {
      await navigator.clipboard.writeText(text);
      this.copied.set(true);
      window.setTimeout(() => this.copied.set(false), 1600);
    } catch {
      this.error.set('Could not copy to clipboard.');
    }
  }
}
