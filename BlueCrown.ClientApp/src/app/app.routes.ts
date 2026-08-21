import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./layouts/public-layout/public-layout')
        .then(m => m.PublicLayout),
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./pages/home/home')
            .then(m => m.Home)
      },
      {
        path: 'products',
        loadComponent: () =>
          import('./pages/products/products')
            .then(m => m.Products)
      },
      {
        path: 'products/:id',
        loadComponent: () =>
          import('./pages/product-detail/product-detail')
            .then(m => m.ProductDetail)
      },
      {
        path: 'cart',
        loadComponent: () =>
          import('./pages/cart/cart')
            .then(m => m.Cart)
      },
      {
        path: 'checkout',
        loadComponent: () =>
          import('./pages/checkout/checkout')
            .then(m => m.Checkout)
      }
    ]
  },

  {
    path: 'login',
    loadComponent: () =>
      import('./pages/login/login')
        .then(m => m.Login)
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./pages/register/register')
        .then(m => m.Register)
  },

  {
    path: '**',
    redirectTo: ''
  }
];
