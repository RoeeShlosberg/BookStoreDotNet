import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Book } from '../models/book.model';

// Define a type for the book data when creating a new book, omitting the id
export type CreateBookDto = Omit<Book, 'id'>;

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private apiUrl = 'http://localhost:5000/api/books'; // Define the API URL

  constructor(private http: HttpClient) { } // Inject HttpClient

  getBooks(): Observable<Book[]> {
    return this.http.get<Book[]>(this.apiUrl);
  }

  getBookById(id: string): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/${id}`);
  }

  addBook(bookData: CreateBookDto): Observable<Book> {
    return this.http.post<Book>(this.apiUrl, bookData);
  }

  updateBook(id: number, bookData: CreateBookDto): Observable<Book> {
    return this.http.put<Book>(`${this.apiUrl}/${id}`, bookData);
  }

  deleteBook(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  searchBooks(query: string): Observable<Book[]> {
    return this.http.get<Book[]>(`${this.apiUrl}/search`, { params: { searchTerm: query } });
  }
}
