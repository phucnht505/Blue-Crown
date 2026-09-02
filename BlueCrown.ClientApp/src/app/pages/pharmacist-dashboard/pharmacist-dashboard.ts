import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-pharmacist-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './pharmacist-dashboard.html',
  styleUrl: './pharmacist-dashboard.css',
})
export class PharmacistDashboard { }
