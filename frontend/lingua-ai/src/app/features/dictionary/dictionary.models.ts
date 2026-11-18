export interface DictionaryEntry {
  id: string;
  term: string;
  translation: string;
  sourceLanguage: string;
  targetLanguage: string;
  exampleSentence?: string | null;
  notes?: string | null;
  createdAtUtc: string;
}

export interface PagedDictionary {
  items: DictionaryEntry[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface DictionaryEntryRequest {
  term: string;
  translation: string;
  sourceLanguage: string;
  targetLanguage: string;
  exampleSentence?: string | null;
  notes?: string | null;
}

export interface LanguageOption {
  code: string;
  label: string;
}

export const DICTIONARY_LANGUAGES: LanguageOption[] = [
  { code: 'bs', label: 'Bosnian' },
  { code: 'hr', label: 'Croatian' },
  { code: 'sr', label: 'Serbian' },
  { code: 'en', label: 'English' },
  { code: 'de', label: 'German' },
  { code: 'fr', label: 'French' },
  { code: 'es', label: 'Spanish' },
  { code: 'it', label: 'Italian' },
];
