import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProductCard } from '../../shared/product-card/product-card';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, ProductCard],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {

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
}
