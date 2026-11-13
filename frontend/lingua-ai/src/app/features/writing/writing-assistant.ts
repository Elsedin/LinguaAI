import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import {
  MAX_WRITING_LENGTH,
  WRITING_LANGUAGES,
  WRITING_OPERATIONS,
} from './writing.models';
import { WritingService } from './writing.service';

@Component({
  selector: 'app-writing-assistant',
  imports: [FormsModule, RouterLink],
  templateUrl: './writing-assistant.html',
  styleUrl: './writing-assistant.scss',
})
export class WritingAssistant {
  private readonly writing = inject(WritingService);

  protected readonly operations = WRITING_OPERATIONS;
  protected readonly languages = WRITING_LANGUAGES;
  protected readonly maxLength = MAX_WRITING_LENGTH;

  protected readonly sourceText = signal('');
  protected readonly resultText = signal('');
  protected readonly operation = signal('rewrite');
  protected readonly language = signal('');
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly copied = signal(false);

  protected readonly characterCount = computed(() => this.sourceText().length);
  protected readonly canRun = computed(
    () => this.sourceText().trim().length > 0 && !this.loading(),
  );
  protected readonly selectedOperation = computed(
    () => this.operations.find((item) => item.value === this.operation()) ?? this.operations[0],
  );

  protected run(): void {
    if (!this.canRun()) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);
    this.copied.set(false);

    const language = this.language().trim();
    this.writing
      .transform({
        text: this.sourceText().trim(),
        operation: this.operation(),
        language: language.length > 0 ? language : null,
      })
      .subscribe({
        next: (response) => {
          this.resultText.set(response.resultText);
          this.loading.set(false);
        },
        error: (err: Error) => {
          this.error.set(err.message);
          this.loading.set(false);
        },
      });
  }

  protected clearAll(): void {
    this.sourceText.set('');
    this.resultText.set('');
    this.error.set(null);
    this.copied.set(false);
  }

  protected useResultAsSource(): void {
    const text = this.resultText();
    if (!text) {
      return;
    }

    this.sourceText.set(text);
    this.resultText.set('');
    this.error.set(null);
    this.copied.set(false);
  }

  protected async copyResult(): Promise<void> {
    const text = this.resultText();
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
