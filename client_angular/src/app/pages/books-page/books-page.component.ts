import { Component, OnInit, OnDestroy } from '@angular/core'; // Import OnDestroy
import { CommonModule } from '@angular/common';
import { Book } from '../../models/book.model';
import { BookService, CreateBookDto } from '../../services/book.service';
import { CategoryService } from '../../services/category.service';
import { AuthService } from '../../services/auth.service'; // Import AuthService
import { BooksListViewComponent } from '../../components/books-list-view/books-list-view.component';
import { CategoryDropdownComponent } from '../../components/category-dropdown/category-dropdown.component';
import { Router } from '@angular/router';
import { ReactiveFormsModule, FormGroup, FormControl, Validators, FormsModule, FormArray } from '@angular/forms';
import { Subscription } from 'rxjs'; // Import Subscription

@Component({
  selector: 'app-books-page',
  standalone: true,
  imports: [CommonModule, BooksListViewComponent, ReactiveFormsModule, FormsModule, CategoryDropdownComponent],
  templateUrl: './books-page.component.html',
  styleUrl: './books-page.component.css'
})
export class BooksPageComponent implements OnInit, OnDestroy { // Implement OnDestroy
  books: Book[] = [];  isLoading: boolean = true;
  error: string | null = null;
  showAddBookForm: boolean = false;
  addBookForm: FormGroup;
  addBookError: string | null = null;
  isLoggedIn: boolean = false;
  isSearching: boolean = false;
  searchTerm: string = '';
  private authSubscription!: Subscription;
  stars: number[] = Array(10).fill(0);
  hoveredStar: number = 0;
  rankFilter: number = 0;
  categoryFilter: string = 'all';
  filteredBooks: Book[] = [];
  allowedCategories: string[] = [];
  categoriesLoading: boolean = false; // <-- Added flag for categories loading
  editBookForm: FormGroup;
  showEditBookForm: boolean = false;
  editingBookId: number | null = null;
  editBookError: string | null = null;
  
  // Properties for the share feature
  sharedListId: string | null = null;
  sharingInProgress: boolean = false;
  linkCopied: boolean = false;

  constructor(
    private bookService: BookService, 
    private categoryService: CategoryService,
    private router: Router,
    private authService: AuthService // Inject AuthService
    ) {
    this.addBookForm = new FormGroup({
      title: new FormControl('', [Validators.required, Validators.minLength(3)]),
      author: new FormControl('', [Validators.required, Validators.minLength(3)]),
      uploadDate: new FormControl('', Validators.required),
      rank: new FormControl('', [Validators.required, Validators.min(1), Validators.max(10)]),
      categories: new FormArray([], Validators.required)
    });
    this.editBookForm = new FormGroup({
      title: new FormControl('', [Validators.required, Validators.minLength(3)]),
      author: new FormControl('', [Validators.required, Validators.minLength(3)]),
      uploadDate: new FormControl('', Validators.required),
      rank: new FormControl('', [Validators.required, Validators.min(1), Validators.max(10)]),
      categories: new FormArray([], Validators.required)
    });
  } // Initialize form group for adding a new book

  ngOnInit(): void {    
    this.fetchBooks();
    this.fetchCategories();
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
        // Ensure every book has a categories array (for type safety)
        this.books = data.map(book => ({ ...book, categories: book.categories ?? [] }));
        this.applyRankFilter(); // This will now apply both rank and category filters
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
    if (this.showAddBookForm) {
      // Always re-initialize categories FormArray when showing the form
      const formArray = this.addBookForm.get('categories') as FormArray;
      formArray.clear();
      this.allowedCategories.forEach(() => formArray.push(new FormControl(false)));
    } else {
      this.addBookForm.reset(); // Reset form if hiding
    }
  }

  fetchCategories(): void {
    this.categoriesLoading = true; // <-- Set loading true when fetching categories
    this.categoryService.getCategories().subscribe({
      next: (cats) => {
        this.allowedCategories = cats;
        // Reset FormArray
        const formArray = this.addBookForm.get('categories') as FormArray;
        formArray.clear();
        cats.forEach(() => formArray.push(new FormControl(false)));
        this.categoriesLoading = false; // <-- Set loading false on success
      },
      error: () => {
        this.allowedCategories = [];
        this.categoriesLoading = false; // <-- Set loading false on error
      }
    });
  }
  onAddBookSubmit(): void {
    const selectedCats = this.selectedCategories();
    if (this.addBookForm.invalid || selectedCats.length === 0 || selectedCats.length > 3) {
      if (selectedCats.length === 0) {
        this.addBookError = 'Please select at least one category.';
      } else if (selectedCats.length > 3) {
        this.addBookError = 'Please select at most 3 categories.';
      } else {
        this.addBookError = 'Please correct the errors in the form.';
      }
      Object.values(this.addBookForm.controls).forEach(control => {
        control.markAsTouched();
      });
      return;
    }
    this.addBookError = null;
    const newBook: any = {
      title: this.addBookForm.value.title,
      author: this.addBookForm.value.author,
      uploadDate: this.addBookForm.value.uploadDate ? new Date(this.addBookForm.value.uploadDate) : null,
      rank: this.addBookForm.value.rank,
      categories: selectedCats
    };
    this.isLoading = true;
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
        this.addBookError = err?.error?.error || err?.error?.message || err?.message || 'Failed to add book. Please try again.';
        this.isLoading = false;
      }
    });
  }

  openEditBookForm(book: Book): void {
    this.showEditBookForm = true;
    this.editingBookId = book.id;
    this.editBookError = null;
    this.editBookForm.patchValue({
      title: book.title,
      author: book.author,
      uploadDate: book.uploadDate ? (new Date(book.uploadDate)).toISOString().substring(0, 10) : '',
      rank: book.rank
    });
    // Set categories
    const formArray = this.editBookForm.get('categories') as FormArray;
    formArray.clear();
    this.allowedCategories.forEach(cat => {
      formArray.push(new FormControl(book.categories?.includes(cat)));
    });
  }

  closeEditBookForm(): void {
    this.showEditBookForm = false;
    this.editingBookId = null;
    this.editBookForm.reset();
  }
  onEditBookSubmit(): void {
    const selectedCats = this.selectedEditCategories();
    if (this.editBookForm.invalid || selectedCats.length === 0 || selectedCats.length > 3) {
      if (selectedCats.length === 0) {
        this.editBookError = 'Please select at least one category.';
      } else if (selectedCats.length > 3) {
        this.editBookError = 'Please select at most 3 categories.';
      } else {
        this.editBookError = 'Please correct the errors in the form.';
      }
      Object.values(this.editBookForm.controls).forEach(control => {
        control.markAsTouched();
      });
      return;
    }
    this.editBookError = null;
    const updatedBook: any = {
      title: this.editBookForm.value.title,
      author: this.editBookForm.value.author,
      uploadDate: this.editBookForm.value.uploadDate ? new Date(this.editBookForm.value.uploadDate) : null,
      rank: this.editBookForm.value.rank,
      categories: selectedCats
    };
    if (this.editingBookId != null) {
      this.isLoading = true;
      this.bookService.updateBook(this.editingBookId, updatedBook).subscribe({
        next: () => {
          this.fetchBooks();
          this.closeEditBookForm();
          this.isLoading = false;
        },
        error: (err) => {
          this.editBookError = err?.error?.error || err?.error?.message || err?.message || 'Failed to update book. Please try again.';
          this.isLoading = false;
        }
      });
    }
  }

  selectedCategories(): string[] {
    return this.allowedCategories.filter((cat, i) => (this.addBookForm.get('categories') as FormArray).at(i).value);
  }

  selectedEditCategories(): string[] {
    return this.allowedCategories.filter((cat, i) => (this.editBookForm.get('categories') as FormArray).at(i).value);
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
        // Ensure every book has a categories array (for type safety)
        this.books = data.map(book => ({ ...book, categories: book.categories ?? [] }));
        this.applyRankFilter(); // Apply both rank and category filters to search results
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error searching books:', err);
        this.error = 'Failed to search books. Please try again later.';
        this.isLoading = false;
      }
    });
  }  resetSearch(): void {
    this.searchTerm = '';
    this.isSearching = false;
    // Don't reset filters, just refetch books and apply existing filters
    this.fetchBooks();
  }

  resetFilters(): void {
    this.rankFilter = 0;
    this.categoryFilter = 'all';
    this.applyRankFilter();
  }
  
  resetAll(): void {
    this.searchTerm = '';
    this.isSearching = false;
    this.rankFilter = 0;
    this.categoryFilter = 'all';
    this.fetchBooks();
  }

  setRank(rank: number): void {
    this.addBookForm.get('rank')?.setValue(rank);
    this.addBookForm.get('rank')?.markAsTouched();
  }

  hoverStars(rank: number): void {
    this.hoveredStar = rank;
  }
  applyRankFilter(): void {
    let filtered = this.books;
    
    // Apply rank filter if set
    if (this.rankFilter > 0) {
      filtered = filtered.filter(book => book.rank >= this.rankFilter);
    }
    
    // Apply category filter if not set to 'all'
    if (this.categoryFilter !== 'all') {
      filtered = filtered.filter(book => 
        book.categories && book.categories.includes(this.categoryFilter)
      );
    }
    
    this.filteredBooks = filtered;
  }

  get categoriesFormArray(): FormArray {
    return this.addBookForm.get('categories') as FormArray;
  }

  get categoryControls(): FormControl[] {
    const arr = this.addBookForm.get('categories');
    return (arr && arr instanceof FormArray) ? (arr.controls as FormControl[]) : [];
  }

  get editCategoryControls(): FormControl[] {
    const arr = this.editBookForm.get('categories');
    return (arr && arr instanceof FormArray) ? (arr.controls as FormControl[]) : [];
  }

  createSharedList(): void {
    if (this.filteredBooks.length === 0) {
      return;
    }

    this.sharingInProgress = true;
    const bookIds = this.filteredBooks.map(book => book.id);
    
    this.bookService.createSharedList(bookIds).subscribe({
      next: (response) => {
        this.sharingInProgress = false;
        this.sharedListId = response.id;
      },
      error: (err) => {
        this.sharingInProgress = false;
        this.error = 'Failed to create shared list. Please try again.';
        console.error('Error creating shared list:', err);
      }
    });
  }

  getFullShareUrl(): string {
    const baseUrl = window.location.origin;
    return `${baseUrl}/share/${this.sharedListId}`;
  }

  copyShareLink(inputElement: HTMLInputElement): void {
    inputElement.select();
    document.execCommand('copy');
    this.linkCopied = true;
    
    // Reset the copied flag after 2 seconds
    setTimeout(() => {
      this.linkCopied = false;
    }, 2000);
  }

  closeShareDialog(): void {
    this.sharedListId = null;
    this.linkCopied = false;
  }

  getWhatsAppShareUrl(): string {
    const shareUrl = this.getFullShareUrl();
    const bookCount = this.filteredBooks.length;
    const message = `Check out this collection of ${bookCount} books! ${shareUrl}`;
    return `https://wa.me/?text=${encodeURIComponent(message)}`;
  }

  getFacebookShareUrl(): string {
    const shareUrl = this.getFullShareUrl();
    return `https://www.facebook.com/sharer/sharer.php?u=${encodeURIComponent(shareUrl)}`;
  }

  getTwitterShareUrl(): string {
    const shareUrl = this.getFullShareUrl();
    const bookCount = this.filteredBooks.length;
    const message = `Check out my curated collection of ${bookCount} books!`;
    return `https://twitter.com/intent/tweet?text=${encodeURIComponent(message)}&url=${encodeURIComponent(shareUrl)}`;
  }

  getEmailShareUrl(): string {
    const shareUrl = this.getFullShareUrl();
    const bookCount = this.filteredBooks.length;
    const subject = `A Book Collection to Check Out`;
    const body = `Hi,\n\nI thought you might enjoy this collection of ${bookCount} books.\n\nCheck it out here: ${shareUrl}\n\nEnjoy!`;
    return `mailto:?subject=${encodeURIComponent(subject)}&body=${encodeURIComponent(body)}`;
  }
}
