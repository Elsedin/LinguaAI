import { Routes } from '@angular/router';
import { HistoryList } from './features/history/history-list';
import { Translator } from './features/translator/translator';
import { WritingAssistant } from './features/writing/writing-assistant';

export const routes: Routes = [
  { path: '', component: Translator },
  { path: 'writing', component: WritingAssistant },
  { path: 'history', component: HistoryList },
  { path: '**', redirectTo: '' },
];
