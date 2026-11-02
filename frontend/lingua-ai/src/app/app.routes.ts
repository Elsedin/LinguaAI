import { Routes } from '@angular/router';
import { Translator } from './features/translator/translator';

export const routes: Routes = [
  { path: '', component: Translator },
  { path: '**', redirectTo: '' },
];
