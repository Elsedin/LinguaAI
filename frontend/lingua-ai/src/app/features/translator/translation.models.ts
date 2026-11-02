export const MAX_TEXT_LENGTH = 5000;

export interface LanguageOption {
  code: string;
  label: string;
}

export interface StyleOption {
  value: string;
  label: string;
}

export const SOURCE_LANGUAGES: LanguageOption[] = [
  { code: 'auto', label: 'Detect language' },
  { code: 'bs', label: 'Bosnian' },
  { code: 'hr', label: 'Croatian' },
  { code: 'sr', label: 'Serbian' },
  { code: 'en', label: 'English' },
  { code: 'de', label: 'German' },
  { code: 'fr', label: 'French' },
  { code: 'es', label: 'Spanish' },
  { code: 'it', label: 'Italian' },
];

export const TARGET_LANGUAGES: LanguageOption[] = SOURCE_LANGUAGES.filter(
  (language) => language.code !== 'auto',
);

export const TRANSLATION_STYLES: StyleOption[] = [
  { value: 'natural', label: 'Natural' },
  { value: 'literal', label: 'Literal' },
  { value: 'formal', label: 'Formal' },
  { value: 'informal', label: 'Informal' },
  { value: 'professional', label: 'Professional' },
  { value: 'academic', label: 'Academic' },
];

export interface TranslateRequest {
  text: string;
  sourceLanguage: string;
  targetLanguage: string;
  style: string;
}

export interface TranslateResponse {
  translatedText: string;
  sourceLanguage: string;
  targetLanguage: string;
  style: string;
}
