import { Routes } from '@angular/router';
import { DictionaryPage } from './features/dictionary/dictionary-page';
import { HistoryList } from './features/history/history-list';
import { Translator } from './features/translator/translator';
import { WritingAssistant } from './features/writing/writing-assistant';

export const routes: Routes = [
  { path: '', component: Translator },
  { path: 'writing', component: WritingAssistant },
  { path: 'history', component: HistoryList },
  { path: 'dictionary', component: DictionaryPage },
  { path: '**', redirectTo: '' },
];
