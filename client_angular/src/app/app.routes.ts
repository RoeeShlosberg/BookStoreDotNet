import { Routes } from '@angular/router';
import { LandingPageComponent } from './pages/landing-page/landing-page.component';
import { BooksPageComponent } from './pages/books-page/books-page.component';
import { BookPageComponent } from './pages/book-page/book-page.component';
import { LoginPageComponent } from './pages/login-page/login-page.component'; // Added import
import { RegisterPageComponent } from './pages/register-page/register-page.component'; // Added import
import { NotFoundPageComponent } from './pages/not-found-page/not-found-page.component'; // Import NotFoundPageComponent
import { AuthGuard } from './guards/auth.guard';
import { SharedListPageComponent } from './pages/shared-list-page/shared-list-page.component'; // Import SharedListPageComponent

export const routes: Routes = [
  { path: '', component: LandingPageComponent },
  { path: 'books', component: BooksPageComponent, canActivate: [AuthGuard] },
  { path: 'book/:id', component: BookPageComponent, canActivate: [AuthGuard] },
  { path: 'login', component: LoginPageComponent },
  { path: 'register', component: RegisterPageComponent },
  { path: 'share/:id', component: SharedListPageComponent },
  { path: '**', component: NotFoundPageComponent }, // Wildcard route for 404
];
