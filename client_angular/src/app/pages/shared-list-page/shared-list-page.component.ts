import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { BookService } from '../../services/book.service';
import { Book } from '../../models/book.model';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { BookViewComponent } from '../../components/book-view/book-view.component';
import { MatIconModule } from '@angular/material/icon';
import { MatSliderModule } from '@angular/material/slider';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';

@Component({  selector: 'app-shared-list-page',
  standalone: true,  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatIconModule,
    MatSliderModule,
    MatButtonModule,
    BookViewComponent
  ],
  templateUrl: './shared-list-page.component.html',
  styleUrls: ['./shared-list-page.component.css']
})
export class SharedListPageComponent implements OnInit {
  books: Book[] = [];
  filteredBooks: Book[] = [];
  searchTerm = '';
  categories: string[] = [];
  selectedCategories: string[] = [];
  isLoading = true;
  hasError = false;
  errorMessage = '';
  minRank = 1;
  maxRank = 10;
  selectedMinRank = 1;
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService
  ) {}

  ngOnInit(): void {
    const listId = this.route.snapshot.paramMap.get('id');
    if (listId) {
      this.bookService.getSharedList(listId).subscribe({
        next: (books) => {
          this.books = books;
          this.filteredBooks = books;
          this.categories = Array.from(new Set(books.flatMap(b => b.categories)));
          this.isLoading = false;
        },
        error: (error) => {
          this.isLoading = false;
          this.hasError = true;
          this.errorMessage = error.status === 401 ? 
            'Unauthorized access. This shared list may have been deleted or is private.' : 
            'Error loading the shared list. Please try again later.';
          console.error('Error fetching shared list:', error);
        }
      });
    } else {
      this.isLoading = false;
      this.hasError = true;
      this.errorMessage = 'No list ID provided. Please check the URL.';
    }
  }  onSearch(event: Event) {
    const input = event.target as HTMLInputElement;
    this.searchTerm = input.value;
    this.applyFilters();
  }

  onCategoryChange(selected: string[]) {
    this.selectedCategories = selected;
    this.applyFilters();
  }

  onRankChange(value: number) {
    this.selectedMinRank = value;
    this.applyFilters();
  }

  applyFilters() {
    this.filteredBooks = this.books.filter(book => {
      const matchesSearch = !this.searchTerm || 
        book.title.toLowerCase().includes(this.searchTerm.toLowerCase()) || 
        book.author.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      const matchesCategory = this.selectedCategories.length === 0 || 
        this.selectedCategories.some(cat => book.categories.includes(cat));
      
      const matchesRank = book.rank >= this.selectedMinRank;
      
      return matchesSearch && matchesCategory && matchesRank;
    });
  }

  navigateToHome() {
    this.router.navigate(['/']);
  }
}
