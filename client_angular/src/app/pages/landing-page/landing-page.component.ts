import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service'; // Import AuthService
import { Subscription } from 'rxjs';
import { CommonModule } from '@angular/common'; // Import CommonModule

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [CommonModule], // Add CommonModule
  templateUrl: './landing-page.component.html',
  styleUrl: './landing-page.component.css'
})
export class LandingPageComponent implements OnInit, OnDestroy {
  isLoggedIn: boolean = false;
  userName: string | null = null; // Add userName property
  private authSubscription!: Subscription;

  constructor(
    private router: Router,
    private authService: AuthService // Inject AuthService
  ) {
  }

  ngOnInit(): void {    
    this.authSubscription = this.authService.isAuthenticated$.subscribe(status => {
      this.isLoggedIn = status;
      if (status) {
        this.userName = this.authService.getUsername();
      } else {
        this.userName = null;
      }
    });
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  navigateToBooks() {
    this.router.navigate(['/books']); // Navigate to the /books route
    // console.log('Navigate to books clicked! Placeholder for actual navigation.');
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }

  navigateToRegister(): void {
    this.router.navigate(['/register']);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']); // Navigate to landing page after logout
  }
}
