import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit, signal } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  private http =inject(HttpClient) ;
  // same constructor(private http: HttpClient) { }
  protected readonly title = 'client 1th Angular App';
  protected members = signal<any>([]);
  
  ngOnInit(): void {
    this.http.get('https://localhost:5001/api/members')
    .subscribe({
      next: res => {this.members.set(res); console.log(this.members());},
      error: err => console.log(err),
      complete: () => console.log('Completed')
    });
  }  
}
