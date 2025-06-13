import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router'; // Import Router & RouterLink
import { FormsModule, NgForm } from '@angular/forms'; // Import FormsModule and NgForm
import { CommonModule } from '@angular/common'; // Import CommonModule
import { AuthService } from '../../services/auth.service'; // Import AuthService
import { UserCredentials } from '../../models/auth.model'; // Import UserCredentials

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink], // Add FormsModule and RouterLink
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css']
})
export class RegisterPageComponent {
  errorMessage: string | null = null;

  constructor(private authService: AuthService, private router: Router) { }

  onSubmit(form: NgForm): void {
    if (form.invalid) {
      this.errorMessage = "Please fill in all required fields.";
      return;
    }
    if (form.value.password !== form.value.confirmPassword) {
      this.errorMessage = "Passwords do not match.";
      return;
    }
    this.errorMessage = null; // Clear previous error messages
    const credentials: UserCredentials = { username: form.value.username, password: form.value.password };
    this.authService.register(credentials).subscribe({
      next: () => {
        alert('Registration successful! Please login.'); // Or navigate to login page
        this.router.navigate(['/login']);
      },
      error: (err) => {
        if (err.error && typeof err.error === 'string') {
          this.errorMessage = err.error;
        } else if (err.error && err.error.message && typeof err.error.message === 'string') { // Handle nested error message
          this.errorMessage = err.error.message;
        } else if (err.message) {
          this.errorMessage = err.message;
        } else {
          this.errorMessage = 'An unexpected error occurred during registration. Please try again.';
        }
        console.error('Registration error:', err);
      }
    });
  }
}
