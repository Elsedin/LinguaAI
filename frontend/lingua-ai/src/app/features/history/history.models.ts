export interface HistoryItem {
  id: string;
  sourceText: string;
  translatedText: string;
  sourceLanguage: string;
  targetLanguage: string;
  style: string;
  operationType: string;
  isFavorite: boolean;
  createdAtUtc: string;
}

export interface PagedHistory {
  items: HistoryItem[];
  page: number;
  pageSize: number;
  totalCount: number;
}
