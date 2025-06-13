import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookService, CreateBookDto } from '../../services/book.service'; // Import CreateBookDto
import { AuthService } from '../../services/auth.service';
import { Book } from '../../models/book.model';
import { CommonModule, formatDate } from '@angular/common'; // Import formatDate
import { BookViewComponent } from '../../components/book-view/book-view.component';
import { Subscription } from 'rxjs';
import { ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms'; // Import ReactiveFormsModule and form-related classes


@Component({
  selector: 'app-book-page',
  standalone: true,
  imports: [CommonModule, BookViewComponent, ReactiveFormsModule], // Add ReactiveFormsModule
  templateUrl: './book-page.component.html',
  styleUrl: './book-page.component.css'
})
export class BookPageComponent implements OnInit, OnDestroy {
  book: Book | null = null;
  isLoading: boolean = true;
  error: string | null = null;
  deleteError: string | null = null;
  editError: string | null = null; // For edit operation errors
  isLoggedIn: boolean = false;
  showEditForm: boolean = false;
  editBookForm!: FormGroup; // Initialize in constructor or ngOnInit
  private authSubscription!: Subscription;

  constructor(
    private route: ActivatedRoute,
    private bookService: BookService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {    
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.fetchBook(id);
    } else {
      this.error = 'Book ID not found in URL.';
      this.isLoading = false;
      console.error('Book ID is null or undefined from route parameters.');
    }
    this.authSubscription = this.authService.isAuthenticated$.subscribe(status => {
      this.isLoggedIn = status;
      if (!status) {
        this.showEditForm = false; // Hide edit form if user logs out
      }
    });
    this.initializeForm(); // Initialize form structure
  }

  initializeForm(): void {    
    this.editBookForm = new FormGroup({
      title: new FormControl('', [Validators.required, Validators.minLength(3)]),
      author: new FormControl('', [Validators.required, Validators.minLength(3)]),
      publishedDate: new FormControl('', Validators.required)
    });
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
  }

  fetchBook(id: string): void {
    this.isLoading = true;
    this.error = null;
    this.deleteError = null;
    this.editError = null; // Reset edit error on new fetch
    this.bookService.getBookById(id).subscribe({
      next: (data) => {
        this.book = data;
        this.isLoading = false;
        if (this.showEditForm) { // If form was open, re-populate with potentially new data
          this.populateFormForEdit();
        }
      },
      error: (err) => {
        console.error('Error fetching book:', err);
        this.error = 'Failed to load book details. The book may not exist or there was a server error.';
        this.isLoading = false;
      }
    });
  }

  populateFormForEdit(): void {
    if (this.book) {
      this.editBookForm.patchValue({
        title: this.book.title,
        author: this.book.author,
        publishedDate: formatDate(this.book.publishedDate, 'yyyy-MM-dd', 'en-US') // Format date for input
      });
    }
  }

  goBackToBooks(): void {
    this.router.navigate(['/books']); // Navigate back to the books list
  }

  toggleEditForm(): void {
    if (!this.isLoggedIn) {
      alert('Please log in to edit books.');
      return;
    }
    if (!this.book) return; // Should not happen if button is visible

    this.showEditForm = !this.showEditForm;
    this.editError = null; // Clear previous edit errors

    if (this.showEditForm) {
      this.populateFormForEdit();
    } else {
      this.editBookForm.reset();
    }
  }

  onEditBookSubmit(): void {
    if (!this.book) {
      this.editError = 'Cannot update: Book data is missing.';
      return;
    }
    if (this.editBookForm.invalid) {
      this.editError = 'Please correct the errors in the form.';
      Object.values(this.editBookForm.controls).forEach(control => {
        control.markAsTouched();
      });
      return;
    }

    this.editError = null;
    const updatedBookData: Book = {
      id: this.book.id, // Keep the original ID
      title: this.editBookForm.value.title,
      author: this.editBookForm.value.author,
      publishedDate: new Date(this.editBookForm.value.publishedDate)
    };

    const bookIdToRefresh = this.book.id; // Store the ID for re-fetching

    this.isLoading = true; // Indicate loading for the PUT operation
    this.bookService.updateBook(bookIdToRefresh, updatedBookData).subscribe({
      next: () => { // The actual updatedBook data from PUT might not be used directly
        // isLoading remains true as fetchBook will take over the loading state management.
        this.showEditForm = false; // Hide the form
        this.editBookForm.reset();
        alert('Book updated successfully!');
        
        // Always refetch the book details to ensure UI is up-to-date
        this.fetchBook(bookIdToRefresh.toString()); 
      },
      error: (err) => {
        console.error('Error updating book:', err);
        if (err.error && typeof err.error === 'string') {
          this.editError = err.error;
        } else if (err.error && err.error.message && typeof err.error.message === 'string') {
          this.editError = err.error.message;
        } else if (err.message) {
          this.editError = err.message;
        } else {
          this.editError = 'Failed to update book. Please try again.';
        }
        this.isLoading = false; // Crucial: set isLoading to false on PUT error
      }
    });
  }

  deleteBook(): void {
    if (!this.isLoggedIn) {
      alert('Please log in to delete books.');
      return;
    }
    if (this.book) {
      if (window.confirm(`Are you sure you want to delete "${this.book.title}"?`)) {
        this.isLoading = true; // Indicate loading state during deletion
        this.deleteError = null;
        this.bookService.deleteBook(this.book.id).subscribe({
          next: () => {
            console.log('Book deleted successfully:', this.book?.id);
            this.isLoading = false;
            this.router.navigate(['/books']); // Navigate to books list on success
          },
          error: (err) => {
            console.error('Error deleting book:', err);
            this.deleteError = 'Failed to delete book. Please try again.';
            if (err.status === 401 || err.status === 403) {
              this.deleteError = 'You are not authorized to delete this book.';
            } else if (err.error && typeof err.error === 'string') {
              this.deleteError = err.error;
            } else if (err.message) {
              this.deleteError = err.message;
            }
            this.isLoading = false;
          }
        });
      }
    }
  }
}