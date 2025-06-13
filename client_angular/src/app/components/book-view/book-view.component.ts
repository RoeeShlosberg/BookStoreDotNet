import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common'; // Import CommonModule
import { Book } from '../../models/book.model';

@Component({
  selector: 'app-book-view',
  standalone: true, // Ensure it's standalone
  imports: [CommonModule], // Add CommonModule for DatePipe and other common directives
  templateUrl: './book-view.component.html',
  styleUrl: './book-view.component.css'
})
export class BookViewComponent {
  @Input() book: Book | undefined;
}
