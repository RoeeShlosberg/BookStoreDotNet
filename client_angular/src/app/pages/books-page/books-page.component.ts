import { Component, OnInit, OnDestroy } from '@angular/core'; // Import OnDestroy
import { CommonModule } from '@angular/common';
import { Book } from '../../models/book.model';
import { BookService, CreateBookDto } from '../../services/book.service';
import { AuthService } from '../../services/auth.service'; // Import AuthService
import { BooksListViewComponent } from '../../components/books-list-view/books-list-view.component';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs'; // Import Subscription

@Component({
  selector: 'app-books-page',
  standalone: true,
  imports: [CommonModule, BooksListViewComponent, ReactiveFormsModule, FormsModule],
  templateUrl: './books-page.component.html',
  styleUrl: './books-page.component.css'
})
export class BooksPageComponent implements OnInit, OnDestroy { // Implement OnDestroy
  books: Book[] = [];
  isLoading: boolean = true;
  error: string | null = null;
  showAddBookForm: boolean = false;
  addBookForm: FormGroup;
  addBookError: string | null = null;
  isLoggedIn: boolean = false;
  isSearching: boolean = false;
  searchTerm: string = '';
  private authSubscription!: Subscription;

  constructor(
    private bookService: BookService, 
    private router: Router,
    private authService: AuthService // Inject AuthService
    ) {
    this.addBookForm = new FormGroup({
      title: new FormControl('', [Validators.required, Validators.minLength(3)]),
      author: new FormControl('', [Validators.required, Validators.minLength(3)]),
      publishedDate: new FormControl('', Validators.required)
    });
  } // Initialize form group for adding a new book

  ngOnInit(): void {    
    this.fetchBooks();
    this.authSubscription = this.authService.isAuthenticated$.subscribe(status => {
      this.isLoggedIn = status;
      if (!status && this.showAddBookForm) {
        this.showAddBookForm = false; // Hide add book form if user logs out
      }
    });
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']); // Navigate to landing page after logout
  }

  fetchBooks(): void { // Renamed and made public for retry
    this.isLoading = true; // Set loading true at the start of fetch
    this.error = null; // Reset error on new fetch attempt
    this.bookService.getBooks().subscribe({
      next: (data) => {
        console.log('Books fetched successfully:', data);
        
        this.books = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error fetching books:', err);
        this.error = 'Failed to load books. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  retryFetchBooks(): void {
    this.fetchBooks();
  }

  goHome(): void {    
    this.router.navigate(['/']);
  }

  toggleAddBookForm(): void {
    if (!this.isLoggedIn) {
      alert('Please log in to add books.');
      return;
    }
    this.showAddBookForm = !this.showAddBookForm;
    this.addBookError = null; // Clear any previous errors
    if (!this.showAddBookForm) {
      this.addBookForm.reset(); // Reset form if hiding
    }
  }

  onAddBookSubmit(): void {
    if (this.addBookForm.invalid) {
      this.addBookError = 'Please correct the errors in the form.';
      // Mark all fields as touched to display validation messages
      Object.values(this.addBookForm.controls).forEach(control => {
        control.markAsTouched();
      });
      return;
    }
    this.addBookError = null;
    const newBook: CreateBookDto = {
      title: this.addBookForm.value.title,
      author: this.addBookForm.value.author,
      // Ensure publishedDate is in the correct format if necessary, e.g., ISO string
      publishedDate: new Date(this.addBookForm.value.publishedDate) // Sending as Date object
    };

    this.isLoading = true; // Show loading indicator for the page or a specific one for the form
    this.bookService.addBook(newBook).subscribe({
      next: (addedBook) => {
        console.log('Book added successfully', addedBook);
        alert('Book added successfully!'); // Show success message
        this.fetchBooks(); // Refresh the list of books
        this.toggleAddBookForm(); // Hide the form and reset it
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error adding book:', err);
        if (err.error && typeof err.error === 'string') {
          this.addBookError = err.error;
        } else if (err.error && err.error.message && typeof err.error.message === 'string') {
          this.addBookError = err.error.message;
        } else if (err.message) {
          this.addBookError = err.message;
        } else {
          this.addBookError = 'Failed to add book. Please try again.';
        }
        this.isLoading = false;
      }
    });
  }

  searchBooks(): void {
    if (!this.searchTerm.trim()) {
      this.resetSearch();
      return;
    }
    this.isLoading = true;
    this.error = null;
    this.isSearching = true;
    this.bookService.searchBooks(this.searchTerm).subscribe({
      next: (data) => {
        this.books = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error searching books:', err);
        this.error = 'Failed to search books. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  resetSearch(): void {
    this.searchTerm = '';
    this.isSearching = false;
    this.fetchBooks();
  }
}
