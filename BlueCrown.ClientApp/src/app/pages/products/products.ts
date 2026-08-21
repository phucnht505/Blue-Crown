import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ProductCard } from '../../shared/product-card/product-card';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [FormsModule, ProductCard],
  templateUrl: './products.html',
  styleUrl: './products.css'
})
export class Products {

  searchText = '';

  products = [
    {
      id: '83759a31-7272-428c-9015-4eebca315cd4',
      name: 'Paracetamol 500mg',
      genericName: 'Paracetamol',
      category: 'Thuốc giảm đau',
      price: 15000
    },
    {
      id: 'c464dfa6-0a2b-4552-9a2c-74adfd41b21b',
      name: 'Amoxicillin 500mg',
      genericName: 'Amoxicillin',
      category: 'Kháng sinh',
      price: 45000
    },
    {
      id: '3',
      name: 'Vitamin C 500mg',
      genericName: 'Vitamin C',
      category: 'Vitamin',
      price: 35000
    },
    {
      id: '4',
      name: 'Berocca',
      genericName: 'Multivitamin',
      category: 'Vitamin',
      price: 120000
    }
  ];

  get filteredProducts() {
    const keyword = this.searchText.trim().toLowerCase();

    if (!keyword)
      return this.products;

    return this.products.filter(x =>
      x.name.toLowerCase().includes(keyword) ||
      x.genericName.toLowerCase().includes(keyword) ||
      x.category.toLowerCase().includes(keyword)
    );
  }
}
