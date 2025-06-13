import { Component, Input } from '@angular/core';
import { Book } from '../../models/book.model'; // Updated import path
import { BookViewComponent } from '../book-view/book-view.component';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router'; // Import Router

@Component({
  selector: 'app-books-list-view',
  standalone: true,
  imports: [CommonModule, BookViewComponent],
  templateUrl: './books-list-view.component.html',
  styleUrl: './books-list-view.component.css'
})
export class BooksListViewComponent {
  @Input() books: Book[] = [];

  constructor(private router: Router) {} // Inject Router

  goToBookPage(book: Book): void {    
    this.router.navigate(['/book', book.id]); // Corrected navigation path
  }

  getBooks(): Book[] {
    return this.books || [];
  }
}
