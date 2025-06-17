import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { BookService } from '../../services/book.service';
import { SharedListPageComponent } from './shared-list-page.component';
import { Book } from '../../models/book.model';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatOptionModule } from '@angular/material/core';
import { BookViewComponent } from '../../components/book-view/book-view.component';
import { CommonModule } from '@angular/common';

describe('SharedListPageComponent', () => {
  let component: SharedListPageComponent;
  let fixture: ComponentFixture<SharedListPageComponent>;
  let mockBookService: any;
  let mockActivatedRoute: any;

  beforeEach(async () => {
    mockBookService = {
      getSharedList: jasmine.createSpy('getSharedList').and.returnValue(of([]))
    };
    mockActivatedRoute = {
      snapshot: { paramMap: { get: () => 'test-id' } }
    };

    await TestBed.configureTestingModule({
      imports: [
        SharedListPageComponent,
        CommonModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatOptionModule,
        BookViewComponent
      ],
      providers: [
        { provide: BookService, useValue: mockBookService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(SharedListPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call getSharedList on init', () => {
    expect(mockBookService.getSharedList).toHaveBeenCalledWith('test-id');
  });
});
