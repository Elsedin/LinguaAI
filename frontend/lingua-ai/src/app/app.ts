import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthPanel } from './features/auth/auth-panel';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AuthPanel],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {}
