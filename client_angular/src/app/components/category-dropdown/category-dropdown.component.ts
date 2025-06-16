import { Component, Input, Output, EventEmitter, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormArray, FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-category-dropdown',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './category-dropdown.component.html',
  styleUrls: ['./category-dropdown.component.css']
})
export class CategoryDropdownComponent {
  @Input() allowedCategories: string[] = [];
  @Input() categoryControls: FormControl[] = [];
  @Input() isInvalid: boolean = false;
  @Input() isTouched: boolean = false;
  @Output() categoriesChanged = new EventEmitter<void>();
  
  isOpen: boolean = false;
  
  constructor(private elementRef: ElementRef) {}

  @HostListener('document:click', ['$event'])
  onClick(event: MouseEvent) {
    // Close dropdown when clicking outside
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.isOpen = false;
    }
  }

  toggleDropdown(event: Event) {
    event.stopPropagation();
    this.isOpen = !this.isOpen;
  }

  selectCategory(event: Event, index: number) {
    event.stopPropagation();
    if (this.categoryControls[index]) {
      // If trying to select more than 3 categories, prevent it
      if (!this.categoryControls[index].value && this.getSelectedCategories().length >= 3) {
        // Don't allow selecting more than 3
        return;
      }
      this.categoryControls[index].setValue(!this.categoryControls[index].value);
      this.categoriesChanged.emit();
    }
  }

  getSelectedCategories(): string[] {
    return this.allowedCategories.filter((_, i) => 
      this.categoryControls[i] && this.categoryControls[i].value);
  }

  // Convenience method to check if any category is selected
  get hasSelectedCategories(): boolean {
    return this.getSelectedCategories().length > 0;
  }
  
  get hasMaxCategories(): boolean {
    return this.getSelectedCategories().length >= 3;
  }
}
