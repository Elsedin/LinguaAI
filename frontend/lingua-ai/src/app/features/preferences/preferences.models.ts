export interface UserPreferences {
  preferredSourceLanguage?: string | null;
  preferredTargetLanguage?: string | null;
  preferredStyle?: string | null;
}

export interface UpdatePreferencesRequest {
  preferredSourceLanguage?: string | null;
  preferredTargetLanguage?: string | null;
  preferredStyle?: string | null;
}
