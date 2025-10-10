import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from "./layout/header/header.component";
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-root',
  imports: [ HeaderComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  
  title = signal('Skinet');
  baseUri = '/api/';
  private http = inject(HttpClient);

  ngOnInit(): void {
    this.http.get(`${this.baseUri}products`)
      .subscribe({
        next: (response) => console.log('Fetched products:', response),
        error: (err) => console.log('Failed to fetch products:', err),
        complete: () => console.log('Fetch products request completed')
      });
  }
}
