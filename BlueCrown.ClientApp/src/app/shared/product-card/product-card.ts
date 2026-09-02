import {
  Component,
  Input,
} from '@angular/core';
import {
  RouterLink,
} from '@angular/router';
import {
  DecimalPipe,
} from '@angular/common';

import {
  Product,
} from '../../models/product.model';

@Component({
  selector: 'app-product-card',
  standalone: true,
  imports: [
    RouterLink,
    DecimalPipe,
  ],
  templateUrl: './product-card.html',
  styleUrl: './product-card.css',
})
export class ProductCard {
  @Input({ required: true })
  product!: Product;
}
