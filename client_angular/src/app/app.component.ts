import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router'; // Import RouterOutlet

@Component({
  selector: 'app-root',
  standalone: true, // Mark AppComponent as standalone
  imports: [RouterOutlet], // Import RouterOutlet for <router-outlet>
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Book Store';
  // navigateToBooks method is removed as it's now in LandingPageComponent
}
