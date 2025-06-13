import { Component } from '@angular/core';
import { RouterLink } from '@angular/router'; // Import RouterLink

@Component({
  selector: 'app-not-found-page',
  standalone: true,
  imports: [RouterLink], // Add RouterLink here
  templateUrl: './not-found-page.component.html',
  styleUrls: ['./not-found-page.component.css']
})
export class NotFoundPageComponent {
  constructor() { }
}
