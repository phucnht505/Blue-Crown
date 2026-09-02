import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { roleGuard } from './guards/role.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./layouts/public-layout/public-layout').then(m => m.PublicLayout),
    children: [
      {
        path: '',
        loadComponent: () => import('./pages/home/home').then(m => m.Home),
      },
      {
        path: 'products',
        loadComponent: () => import('./pages/products/products').then(m => m.Products),
      },
      {
        path: 'products/:id',
        loadComponent: () => import('./pages/product-detail/product-detail').then(m => m.ProductDetail),
      },
      {
        path: 'cart',
        loadComponent: () => import('./pages/cart/cart').then(m => m.Cart),
      },
      {
        path: 'checkout',
        loadComponent: () => import('./pages/checkout/checkout').then(m => m.Checkout),
      },
      {
        path: 'order-lookup',
        loadComponent: () => import('./pages/guest-order-lookup/guest-order-lookup').then(m => m.GuestOrderLookup),
      },
      {
        path: 'my-orders',
        canActivate: [authGuard],
        loadComponent: () => import('./pages/my-orders/my-orders').then(m => m.MyOrders),
      },
      {
        path: 'login',
        loadComponent: () => import('./pages/login/login').then(m => m.Login),
      },
      {
        path: 'forgot-password',
        loadComponent: () => import('./pages/forgot-password/forgot-password').then(m => m.ForgotPassword),
      },
      {
        path: 'register',
        loadComponent: () => import('./pages/register/register').then(m => m.Register),
      },
      {
        path: 'unauthorized',
        loadComponent: () => import('./pages/unauthorized/unauthorized').then(m => m.Unauthorized),
      },
      {
        path: 'account/profile',
        canActivate: [authGuard],
        loadComponent: () => import('./pages/account-profile/account-profile').then(m => m.AccountProfilePage),
      },
      {
        path: 'patient/dashboard',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['patient'] },
        loadComponent: () => import('./pages/patient-dashboard/patient-dashboard').then(m => m.PatientDashboard),
      },
      {
        path: 'patient/profile',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['patient'] },
        loadComponent: () => import('./pages/patient-profile/patient-profile').then(m => m.PatientProfilePage),
      },
      {
        path: 'patient/health-metrics',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['patient'] },
        loadComponent: () => import('./pages/patient-health-metrics/patient-health-metrics').then(m => m.PatientHealthMetrics),
      },
      {
        path: 'patient/health-goals',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['patient'] },
        loadComponent: () => import('./pages/patient-health-goals/patient-health-goals').then(m => m.PatientHealthGoals),
      },
      {
        path: 'patient/appointments',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['patient'] },
        loadComponent: () => import('./pages/patient-appointments/patient-appointments').then(m => m.PatientAppointments),
      },
      {
        path: 'patient/medical-records',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['patient'] },
        loadComponent: () => import('./pages/patient-medical-records/patient-medical-records').then(m => m.PatientMedicalRecords),
      },
      {
        path: 'patient/prescriptions',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['patient'] },
        loadComponent: () => import('./pages/patient-prescriptions/patient-prescriptions').then(m => m.PatientPrescriptions),
      },
      {
        path: 'patient/chat',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['patient'] },
        loadComponent: () => import('./pages/patient-chat/patient-chat').then(m => m.PatientChat),
      },
      {
        path: 'patient/chat/:id',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['patient'] },
        loadComponent: () => import('./pages/chat-room/chat-room').then(m => m.ChatRoom),
      },
      {
        path: 'doctor/dashboard',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['doctor'] },
        loadComponent: () => import('./pages/doctor-dashboard/doctor-dashboard').then(m => m.DoctorDashboard),
      },
      {
        path: 'doctor/appointments',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['doctor'] },
        loadComponent: () => import('./pages/doctor-appointments/doctor-appointments').then(m => m.DoctorAppointments),
      },
      {
        path: 'doctor/health-goals',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['doctor'] },
        loadComponent: () => import('./pages/doctor-health-goals/doctor-health-goals').then(m => m.DoctorHealthGoals),
      },
      {
        path: 'doctor/medical-records',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['doctor'] },
        loadComponent: () => import('./pages/doctor-medical-records/doctor-medical-records').then(m => m.DoctorMedicalRecords),
      },
      {
        path: 'doctor/prescriptions',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['doctor'] },
        loadComponent: () => import('./pages/doctor-prescriptions/doctor-prescriptions').then(m => m.DoctorPrescriptions),
      },
      {
        path: 'doctor/chat',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['doctor'] },
        loadComponent: () => import('./pages/doctor-chat/doctor-chat').then(m => m.DoctorChat),
      },
      {
        path: 'doctor/chat/:id',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['doctor'] },
        loadComponent: () => import('./pages/chat-room/chat-room').then(m => m.ChatRoom),
      },
      {
        path: 'pharmacist/dashboard',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['pharmacist'] },
        loadComponent: () => import('./pages/pharmacist-dashboard/pharmacist-dashboard').then(m => m.PharmacistDashboard),
      },
      {
        path: 'pharmacist/prescriptions',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['pharmacist'] },
        loadComponent: () => import('./pages/pharmacist-prescriptions/pharmacist-prescriptions').then(m => m.PharmacistPrescriptions),
      },
      {
        path: 'pharmacist/stock',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['pharmacist'] },
        loadComponent: () => import('./pages/pharmacist-stock/pharmacist-stock').then(m => m.PharmacistStock),
      },
      {
        path: 'pharmacist/inventory',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['pharmacist'] },
        loadComponent: () => import('./pages/pharmacist-inventory/pharmacist-inventory').then(m => m.PharmacistInventory),
      },
      {
        path: 'pharmacist/suppliers',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['pharmacist'] },
        loadComponent: () => import('./pages/pharmacist-suppliers/pharmacist-suppliers').then(m => m.PharmacistSuppliers),
      },
      {
        path: 'pharmacist/medications',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['pharmacist'] },
        loadComponent: () => import('./pages/pharmacist-medications/pharmacist-medications').then(m => m.PharmacistMedications),
      },
      {
        path: 'pharmacist/products',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['pharmacist'] },
        loadComponent: () => import('./pages/pharmacist-products/pharmacist-products').then(m => m.PharmacistProducts),
      },
      {
        path: 'admin/dashboard',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['admin'] },
        loadComponent: () => import('./pages/admin-dashboard/admin-dashboard').then(m => m.AdminDashboard),
      },
      {
        path: 'admin/users',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['admin'] },
        loadComponent: () => import('./pages/admin-users/admin-users').then(m => m.AdminUsers),
      },
      {
        path: 'admin/doctors',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['admin'] },
        loadComponent: () => import('./pages/admin-doctors/admin-doctors').then(m => m.AdminDoctors),
      },
      {
        path: 'admin/pharmacists',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['admin'] },
        loadComponent: () => import('./pages/admin-pharmacists/admin-pharmacists').then(m => m.AdminPharmacists),
      },
      {
        path: 'admin/categories',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['admin'] },
        loadComponent: () => import('./pages/admin-categories/admin-categories').then(m => m.AdminCategories),
      },
      {
        path: 'admin/inventory-receipts',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['admin'] },
        loadComponent: () => import('./pages/admin-inventory-receipts/admin-inventory-receipts').then(m => m.AdminInventoryReceipts),
      },
      {
        path: 'order-management',
        canActivate: [authGuard, roleGuard],
        data: { roles: ['admin', 'pharmacist'] },
        loadComponent: () => import('./pages/order-management/order-management').then(m => m.OrderManagement),
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];
