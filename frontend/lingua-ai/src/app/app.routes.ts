import { Routes } from '@angular/router';
import { HistoryList } from './features/history/history-list';
import { Translator } from './features/translator/translator';

export const routes: Routes = [
  { path: '', component: Translator },
  { path: 'history', component: HistoryList },
  { path: '**', redirectTo: '' },
];
