import { Routes } from '@angular/router';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { BooksPageComponent } from './pages/books-page/books-page.component';
import { BookPageComponent } from './pages/book-page/book-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component'; // Added import
import { RegisterPageComponent } from './pages/register-page/register-page.component'; // Added import
import { NotFoundPageComponent } from './pages/not-found-page/not-found-page.component'; // Import NotFoundPageComponent

export const routes: Routes = [
  { path: '', component: LandingPageComponent },
  { path: 'books', component: BooksPageComponent },
  { path: 'book/:id', component: BookPageComponent },
  { path: 'login', component: LoginPageComponent }, // Added route
  { path: 'register', component: RegisterPageComponent }, // Added route
  { path: '**', component: NotFoundPageComponent }, // Wildcard route for 404
];
