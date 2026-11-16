export const MAX_WRITING_LENGTH = 5000;

export interface WritingOperationOption {
  value: string;
  label: string;
  description: string;
}

export const WRITING_OPERATIONS: WritingOperationOption[] = [
  {
    value: 'rewrite',
    label: 'Rewrite',
    description: 'Improve clarity and flow',
  },
  {
    value: 'grammar',
    label: 'Grammar',
    description: 'Fix grammar, spelling, punctuation',
  },
  {
    value: 'simplify',
    label: 'Simplify',
    description: 'Shorter sentences, easier words',
  },
  {
    value: 'tone',
    label: 'Tone',
    description: 'Adjust the tone while keeping meaning',
  },
  {
    value: 'explain',
    label: 'Explain',
    description: 'Explain a word or phrase clearly',
  },
];

export interface ToneOption {
  value: string;
  label: string;
}

export const WRITING_TONES: ToneOption[] = [
  { value: 'formal', label: 'Formal' },
  { value: 'informal', label: 'Informal' },
  { value: 'professional', label: 'Professional' },
  { value: 'friendly', label: 'Friendly' },
  { value: 'confident', label: 'Confident' },
];

export interface LanguageOption {
  code: string;
  label: string;
}

export const WRITING_LANGUAGES: LanguageOption[] = [
  { code: '', label: 'Auto / keep language' },
  { code: 'bs', label: 'Bosnian' },
  { code: 'hr', label: 'Croatian' },
  { code: 'sr', label: 'Serbian' },
  { code: 'en', label: 'English' },
  { code: 'de', label: 'German' },
  { code: 'fr', label: 'French' },
  { code: 'es', label: 'Spanish' },
  { code: 'it', label: 'Italian' },
];

export interface WritingRequest {
  text: string;
  operation: string;
  language?: string | null;
  tone?: string | null;
}

export interface WritingResponse {
  resultText: string;
  operation: string;
  language?: string | null;
  tone?: string | null;
}
