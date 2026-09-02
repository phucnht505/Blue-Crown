import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { MedicalRecord } from '../../models/medical-record.model';
import { Medication } from '../../models/medication.model';
import { CreatePrescriptionRequest, Prescription } from '../../models/prescription.model';
import { MedicalRecordService } from '../../services/medical-record.service';
import { MedicationService } from '../../services/medication.service';
import { PrescriptionService } from '../../services/prescription.service';

@Component({
  selector: 'app-doctor-prescriptions',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DatePipe],
  templateUrl: './doctor-prescriptions.html',
  styleUrl: './doctor-prescriptions.css',
})
export class DoctorPrescriptions implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly medicalRecordService = inject(MedicalRecordService);
  private readonly medicationService = inject(MedicationService);
  private readonly prescriptionService = inject(PrescriptionService);
  private readonly route = inject(ActivatedRoute);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  records: MedicalRecord[] = [];
  medications: Medication[] = [];
  prescriptions: Prescription[] = [];
  onlineAppointmentId: string | null = null;
  isLoading = true;
  isSaving = false;
  errorMessage = '';
  successMessage = '';
  duplicateMedicationMessage = '';

  prescriptionForm = this.formBuilder.group({
    medicalRecordId: [''],
    diagnosis: ['', Validators.maxLength(2000)],
    items: this.formBuilder.array([this.createItemGroup()]),
  });

  get isOnlineMode(): boolean {
    return !!this.onlineAppointmentId;
  }

  get medicalRecordId() {
    return this.prescriptionForm.controls.medicalRecordId;
  }

  get diagnosis() {
    return this.prescriptionForm.controls.diagnosis;
  }

  get items(): FormArray {
    return this.prescriptionForm.controls.items;
  }

  get selectedMedicalRecord(): MedicalRecord | null {
    return this.records.find(record => record.id === this.medicalRecordId.value) ?? null;
  }

  get availableMedicalRecords(): MedicalRecord[] {
    return this.records.filter(record => !this.prescriptions.some(prescription => prescription.medicalRecordId === record.id));
  }

  get existingOnlinePrescription(): Prescription | null {
    if (!this.onlineAppointmentId) return null;
    return this.prescriptions.find(prescription => prescription.appointmentId === this.onlineAppointmentId) ?? null;
  }

  ngOnInit(): void {
    this.onlineAppointmentId = this.route.snapshot.queryParamMap.get('appointmentId');
    this.configureFormMode();
    this.loadData();
  }

  addMedication(): void {
    this.items.push(this.createItemGroup());
    this.clearMessages();
  }

  removeMedication(index: number): void {
    if (this.items.length === 1) return;

    this.items.removeAt(index);
    this.clearMessages();
  }

  save(): void {
    this.clearMessages();
    this.prescriptionForm.markAllAsTouched();

    if (this.prescriptionForm.invalid) return;

    if (this.isOnlineMode && this.existingOnlinePrescription) {
      this.errorMessage = 'Lịch tư vấn này đã có đơn thuốc.';
      return;
    }

    const rawValue = this.prescriptionForm.getRawValue();
    const medicationIds = rawValue.items.map(item => item['medicationId']);

    if (new Set(medicationIds).size !== medicationIds.length) {
      this.duplicateMedicationMessage = 'Không được thêm cùng một loại thuốc nhiều lần trong đơn.';
      return;
    }

    const items = rawValue.items.map(item => ({
      medicationId: item['medicationId'] ?? '',
      dosage: item['dosage']?.trim() ?? '',
      frequencyPerDay: item['frequencyPerDay'] === null || item['frequencyPerDay'] === undefined ? null : Number(item['frequencyPerDay']),
      durationDays: item['durationDays'] === null || item['durationDays'] === undefined ? null : Number(item['durationDays']),
      instructions: item['instructions']?.trim() || null,
    }));

    let request: CreatePrescriptionRequest;

    if (this.isOnlineMode) {
      request = {
        appointmentId: this.onlineAppointmentId,
        medicalRecordId: null,
        diagnosis: rawValue.diagnosis?.trim() ?? '',
        items,
      };
    } else {
      request = {
        medicalRecordId: rawValue.medicalRecordId ?? '',
        items,
      };
    }

    this.isSaving = true;

    this.prescriptionService.create(request).subscribe({
      next: prescription => {
        this.prescriptions = [prescription, ...this.prescriptions];
        this.successMessage = this.isOnlineMode ? 'Kê đơn sau tư vấn trực tuyến thành công.' : 'Kê đơn thuốc thành công.';
        this.isSaving = false;
        this.resetForm();
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        console.error('Lỗi kê đơn thuốc:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSaving = false;
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
      default:
        return 'Chờ xử lý';
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

  getAppointmentTypeText(type: string | null): string {
    return type?.toLowerCase() === 'online_consult' ? 'Tư vấn trực tuyến' : 'Khám trực tiếp';
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.duplicateMedicationMessage = '';
  }

  private configureFormMode(): void {
    if (this.isOnlineMode) {
      this.medicalRecordId.clearValidators();
      this.diagnosis.setValidators([Validators.required, Validators.maxLength(2000)]);
    } else {
      this.medicalRecordId.setValidators([Validators.required]);
      this.diagnosis.clearValidators();
      this.diagnosis.setValidators([Validators.maxLength(2000)]);
    }

    this.medicalRecordId.updateValueAndValidity();
    this.diagnosis.updateValueAndValidity();
  }

  private loadData(): void {
    forkJoin({
      records: this.medicalRecordService.getDoctorRecords(),
      medications: this.medicationService.getAll(),
      prescriptions: this.prescriptionService.getDoctorPrescriptions(),
    }).subscribe({
      next: result => {
        this.records = result.records;
        this.medications = result.medications;
        this.prescriptions = result.prescriptions;
        this.isLoading = false;

        if (this.isOnlineMode) {
          const existingPrescription = this.existingOnlinePrescription;

          if (existingPrescription) {
            this.successMessage = `Cuộc tư vấn của ${existingPrescription.patientName} đã có đơn thuốc.`;
          }
        } else {
          const requestedMedicalRecordId = this.route.snapshot.queryParamMap.get('medicalRecordId');

          if (requestedMedicalRecordId) {
            const record = this.records.find(item => item.id === requestedMedicalRecordId);
            const existingPrescription = this.prescriptions.find(item => item.medicalRecordId === requestedMedicalRecordId);

            if (record && !existingPrescription) {
              this.prescriptionForm.patchValue({ medicalRecordId: requestedMedicalRecordId });
            } else if (existingPrescription) {
              this.successMessage = `Hồ sơ bệnh án của ${existingPrescription.patientName} đã có đơn thuốc.`;
            }
          }
        }

        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        console.error('Lỗi tải dữ liệu Prescription:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private createItemGroup(): FormGroup {
    return this.formBuilder.group({
      medicationId: ['', Validators.required],
      dosage: ['', [Validators.required, Validators.maxLength(100)]],
      frequencyPerDay: [1, [Validators.required, Validators.min(1), Validators.max(20)]],
      durationDays: [1, [Validators.required, Validators.min(1), Validators.max(365)]],
      instructions: ['', Validators.maxLength(500)],
    });
  }

  private resetForm(): void {
    this.medicalRecordId.setValue('');
    this.diagnosis.setValue('');
    this.items.clear();
    this.items.push(this.createItemGroup());
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();
      if (errors.length > 0) return String(errors[0]);
    }

    return 'Không thể xử lý đơn thuốc. Vui lòng thử lại.';
  }
}
