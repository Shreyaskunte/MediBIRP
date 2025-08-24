import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { tap } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AuthService {
  tokenKey = 'cn_token';
  constructor(private http: HttpClient) {}
  register(email: string, password: string) {
    return this.http.post(`${environment.apiBase}/users/register`, { email, password });
  }
  login(email:string, password:string) {
    return this.http.post<{token:string}>(`${environment.apiBase}/users/login`, { email, password })
      .pipe(tap(res => localStorage.setItem(this.tokenKey, res.token)));
  }
  getToken() { return localStorage.getItem(this.tokenKey); }
}
