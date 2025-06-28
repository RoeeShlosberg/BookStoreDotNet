import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router'; // Import Router & RouterLink
import { FormsModule, NgForm } from '@angular/forms'; // Import FormsModule and NgForm
import { CommonModule } from '@angular/common'; // Import CommonModule
import { AuthService } from '../../services/auth.service'; // Import AuthService
import { UserCredentials } from '../../models/auth.model'; // Import UserCredentials

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink], // Add FormsModule and RouterLink
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css']
})
export class LoginPageComponent {
  errorMessage: string | null = null;

  constructor(private authService: AuthService, private router: Router) { }

  onSubmit(form: NgForm): void {
    if (form.invalid) {
      this.errorMessage = "Please fill in all required fields.";
      return;
    }
    this.errorMessage = null; // Clear previous error messages
    const credentials: UserCredentials = form.value;
    this.authService.login(credentials).subscribe({
      next: () => {
        this.router.navigate(['/books']); // Navigate to books page on successful login
      },
      error: (err) => {
        // Assuming the backend error response has a message property
        // or a more complex error object that might need parsing.
        if (err.error && typeof err.error === 'string') {
          this.errorMessage = err.error;
        } else if (err.message) {
          this.errorMessage = err.message;
        } else {
          this.errorMessage = 'An unexpected error occurred during login. Please try again.';
        }
        console.error('Login error:', err);
        alert('Login failed, please check your credentials and try again.'); // Optionally show an alert on error
      }
    });
  }
}
