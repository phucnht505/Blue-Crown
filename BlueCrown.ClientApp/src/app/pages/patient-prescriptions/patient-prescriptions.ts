import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Prescription } from '../../models/prescription.model';
import { PrescriptionService } from '../../services/prescription.service';

@Component({
  selector: 'app-patient-prescriptions',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './patient-prescriptions.html',
  styleUrl: './patient-prescriptions.css',
})
export class PatientPrescriptions implements OnInit {
  private readonly prescriptionService = inject(PrescriptionService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  prescriptions: Prescription[] = [];
  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.prescriptionService.getPatientPrescriptions().subscribe({
      next: (prescriptions) => {
        this.prescriptions = prescriptions;
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  getStatusText(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'issued':
        return 'Đã kê đơn';
      case 'approved':
        return 'Đã duyệt';
      case 'dispensed':
        return 'Đã cấp thuốc';
      case 'cancelled':
        return 'Đã hủy';
      case 'pending':
        return 'Chờ xử lý';
      default:
        return 'Không xác định';
    }
  }

  getStatusClass(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'issued':
      case 'approved':
        return 'status-approved';
      case 'dispensed':
        return 'status-dispensed';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return 'status-pending';
    }
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;
    return 'Không thể tải đơn thuốc. Vui lòng thử lại.';
  }
}
