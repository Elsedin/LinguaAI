export interface LanguagePairStat {
  sourceLanguage: string;
  targetLanguage: string;
  count: number;
}

export interface ActivityDay {
  date: string;
  count: number;
}

export interface RecentActivity {
  id: string;
  sourcePreview: string;
  translatedPreview: string;
  sourceLanguage: string;
  targetLanguage: string;
  style: string;
  isFavorite: boolean;
  createdAtUtc: string;
}

export interface DashboardSummary {
  totalTranslations: number;
  favoriteCount: number;
  dictionaryEntryCount: number;
  topLanguagePairs: LanguagePairStat[];
  activityByDay: ActivityDay[];
  recentActivity: RecentActivity[];
}
