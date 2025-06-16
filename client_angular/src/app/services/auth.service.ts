import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { UserCredentials, User, AuthResponse } from '../models/auth.model'; // Import auth models
import { Router } from '@angular/router'; // Import Router if you need to navigate after login/logout

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5000/api/users'; // Base API URL for users
  private readonly TOKEN_KEY = 'authToken';
  private readonly USER_ID_KEY = 'authUserId';
  private readonly USERNAME_KEY = 'authUsername';

  // BehaviorSubject to track authentication state
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
  }

  // Make hasToken public for AuthGuard
  public hasToken(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }

  register(credentials: UserCredentials): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/register`, credentials);
  }

  login(credentials: UserCredentials): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response && response.token) {
          localStorage.setItem(this.TOKEN_KEY, response.token);
          localStorage.setItem(this.USER_ID_KEY, response.id.toString());
          localStorage.setItem(this.USERNAME_KEY, response.username);
          console.log('AuthService: Emitting true from login'); // Log before next()
          this.isAuthenticatedSubject.next(true); // Update auth state
          console.log('Token stored:', response.token);
          console.log('User ID stored:', response.id);
          console.log('Username stored:', response.username);
        } else {
          console.error('Login response did not include a token.', response);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_ID_KEY);
    localStorage.removeItem(this.USERNAME_KEY);
    console.log('AuthService: Emitting false from logout'); // Log before next()
    this.isAuthenticatedSubject.next(false); // Update auth state
    console.log('User logged out, token and user info removed.');
    this.router.navigate(['/']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getUserId(): string | null {
    return localStorage.getItem(this.USER_ID_KEY);
  }

  getUsername(): string | null {
    return localStorage.getItem(this.USERNAME_KEY);
  }

  isLoggedIn(): boolean {
    const token = this.getToken();
    // Potentially add token expiration check here in the future
    return !!token;
  }
}
