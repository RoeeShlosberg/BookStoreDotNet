import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookService, CreateBookDto } from '../../services/book.service'; // Import CreateBookDto
import { AuthService } from '../../services/auth.service';
import { Book } from '../../models/book.model';
import { CommonModule, formatDate } from '@angular/common'; // Import formatDate
import { BookViewComponent } from '../../components/book-view/book-view.component';
import { CategoryDropdownComponent } from '../../components/category-dropdown/category-dropdown.component';
import { Subscription } from 'rxjs';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, FormArray } from '@angular/forms'; // Import ReactiveFormsModule and form-related classes
import { CategoryService } from '../../services/category.service';


@Component({
  selector: 'app-book-page',
  standalone: true,
  imports: [CommonModule, BookViewComponent, ReactiveFormsModule, CategoryDropdownComponent], // Add ReactiveFormsModule
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
  private authSubscription!: Subscription;  stars: number[] = Array(10).fill(0);
  hoveredEditStar: number = 0;
  allowedCategories: string[] = [];
  categoriesLoading: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private bookService: BookService,
    private authService: AuthService,
    private categoryService: CategoryService,
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
    this.fetchCategories();
    this.initializeForm(); // Initialize form structure
  }

  fetchCategories(): void {
    this.categoriesLoading = true;
    this.categoryService.getCategories().subscribe({
      next: (cats) => {
        this.allowedCategories = cats;
        // Reset FormArray
        if (this.editBookForm) {
          const formArray = this.editBookForm.get('categories') as FormArray;
          formArray?.clear();
          cats.forEach(() => formArray?.push(new FormControl(false)));
        }
        this.categoriesLoading = false;
      },
      error: () => {
        this.allowedCategories = [];
        this.categoriesLoading = false;
      }
    });
  }

  initializeForm(): void {    
    this.editBookForm = new FormGroup({
      title: new FormControl('', [Validators.required, Validators.minLength(3)]),
      author: new FormControl('', [Validators.required, Validators.minLength(3)]),
      uploadDate: new FormControl('', Validators.required),
      rank: new FormControl('', [Validators.required, Validators.min(1), Validators.max(10)]),
      categories: new FormArray([], Validators.required)
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
        uploadDate: formatDate(this.book.uploadDate, 'yyyy-MM-dd', 'en-US'), // Format date for input
        rank: this.book.rank // Include rank if needed
      });
      // Set categories
      const formArray = this.editBookForm.get('categories') as FormArray;
      formArray.clear();      this.allowedCategories.forEach(cat => {
        formArray.push(new FormControl(this.book?.categories?.includes(cat)));
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

  setEditRank(rank: number): void {
    this.editBookForm.get('rank')?.setValue(rank);
    this.editBookForm.get('rank')?.markAsTouched();
  }

  hoverEditStars(rank: number): void {
    this.hoveredEditStar = rank;
  }
  onEditBookSubmit(): void {
    if (!this.book) {
      this.editError = 'Cannot update: Book data is missing.';
      return;
    }
    
    const selectedCats = this.selectedEditCategories();
    if (this.editBookForm.invalid || selectedCats.length === 0 || selectedCats.length > 3) {
      if (selectedCats.length === 0) {
        this.editError = 'Please select at least one category.';
      } else if (selectedCats.length > 3) {
        this.editError = 'Please select at most 3 categories.';
      } else {
        this.editError = 'Please correct the errors in the form.';
      }
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
      uploadDate: this.editBookForm.value.uploadDate ? new Date(this.editBookForm.value.uploadDate) : new Date(), // Convert to Date object
      rank: this.editBookForm.value.rank, // Include rank if needed
      categories: selectedCats
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
  }  selectedEditCategories(): string[] {
    return this.allowedCategories.filter((cat, i) => (this.editBookForm.get('categories') as FormArray).at(i).value);
  }

  get editCategoryControls(): FormControl[] {
    const arr = this.editBookForm.get('categories');
    return (arr && arr instanceof FormArray) ? (arr.controls as FormControl[]) : [];
  }
}