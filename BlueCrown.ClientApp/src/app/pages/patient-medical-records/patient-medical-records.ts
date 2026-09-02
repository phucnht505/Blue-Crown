import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { MedicalRecord } from '../../models/medical-record.model';
import { MedicalRecordService } from '../../services/medical-record.service';

@Component({
  selector: 'app-patient-medical-records',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './patient-medical-records.html',
  styleUrl: './patient-medical-records.css',
})
export class PatientMedicalRecords implements OnInit {
  private readonly medicalRecordService = inject(MedicalRecordService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  records: MedicalRecord[] = [];
  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.loadRecords();
  }

  getAppointmentTypeText(type: string | null): string {
    switch (type?.toLowerCase()) {
      case 'online_consult':
        return 'Tư vấn trực tuyến';
      case 'clinic_visit':
        return 'Khám tại phòng khám';
      default:
        return 'Không xác định';
    }
  }

  private loadRecords(): void {
    this.medicalRecordService.getPatientRecords().subscribe({
      next: (records) => {
        this.records = records;
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

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;
    return 'Không thể tải hồ sơ bệnh án. Vui lòng thử lại.';
  }
}
