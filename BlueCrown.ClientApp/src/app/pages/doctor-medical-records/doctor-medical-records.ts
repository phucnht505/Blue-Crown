import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Appointment } from '../../models/appointment.model';
import { CreateMedicalRecordRequest, MedicalRecord, UpdateMedicalRecordRequest } from '../../models/medical-record.model';
import { AppointmentService } from '../../services/appointment.service';
import { MedicalRecordService } from '../../services/medical-record.service';

@Component({
  selector: 'app-doctor-medical-records',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DatePipe],
  templateUrl: './doctor-medical-records.html',
  styleUrl: './doctor-medical-records.css',
})
export class DoctorMedicalRecords implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly appointmentService = inject(AppointmentService);
  private readonly medicalRecordService = inject(MedicalRecordService);
  private readonly route = inject(ActivatedRoute);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  appointments: Appointment[] = [];
  records: MedicalRecord[] = [];

  isLoading = true;
  isSaving = false;
  editingRecordId: string | null = null;
  errorMessage = '';
  successMessage = '';

  recordForm = this.formBuilder.nonNullable.group({
    appointmentId: ['', Validators.required],
    diagnosis: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(500)]],
    notes: ['', Validators.maxLength(3000)],
  });

  get appointmentId() {
    return this.recordForm.controls.appointmentId;
  }

  get diagnosis() {
    return this.recordForm.controls.diagnosis;
  }

  get notes() {
    return this.recordForm.controls.notes;
  }

  get completedAppointmentsWithoutRecord(): Appointment[] {
    return this.appointments
      .filter(appointment => appointment.status?.toLowerCase() === 'completed')
      .filter(appointment => !this.records.some(record => record.appointmentId === appointment.id))
      .sort((a, b) => new Date(b.scheduledAt).getTime() - new Date(a.scheduledAt).getTime());
  }

  get selectedAppointment(): Appointment | null {
    return this.appointments.find(appointment => appointment.id === this.appointmentId.value) ?? null;
  }

  ngOnInit(): void {
    this.loadData();
  }

  save(): void {
    this.clearMessages();
    this.recordForm.markAllAsTouched();

    if (this.recordForm.invalid) return;

    const value = this.recordForm.getRawValue();

    if (this.editingRecordId) {
      const request: UpdateMedicalRecordRequest = {
        diagnosis: value.diagnosis.trim(),
        notes: value.notes.trim() || null,
      };

      this.isSaving = true;

      this.medicalRecordService.update(this.editingRecordId, request).subscribe({
        next: (updatedRecord) => {
          this.records = this.records.map(record => record.id === updatedRecord.id ? updatedRecord : record);
          this.successMessage = 'Cập nhật hồ sơ bệnh án thành công.';
          this.isSaving = false;
          this.cancelEdit();
          this.changeDetectorRef.detectChanges();
        },
        error: (error) => {
          this.errorMessage = this.getApiErrorMessage(error);
          this.isSaving = false;
          this.changeDetectorRef.detectChanges();
        },
      });

      return;
    }

    const request: CreateMedicalRecordRequest = {
      appointmentId: value.appointmentId,
      diagnosis: value.diagnosis.trim(),
      notes: value.notes.trim() || null,
    };

    this.isSaving = true;

    this.medicalRecordService.create(request).subscribe({
      next: (record) => {
        this.records = [record, ...this.records];
        this.successMessage = 'Tạo hồ sơ bệnh án thành công.';
        this.isSaving = false;
        this.resetForm();
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  startEdit(record: MedicalRecord): void {
    this.clearMessages();
    this.editingRecordId = record.id;

    this.recordForm.patchValue({
      appointmentId: record.appointmentId ?? '',
      diagnosis: record.diagnosis,
      notes: record.notes ?? '',
    });

    this.recordForm.controls.appointmentId.disable();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  cancelEdit(): void {
    this.editingRecordId = null;
    this.recordForm.controls.appointmentId.enable();
    this.resetForm();
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

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private loadData(): void {
    this.isLoading = true;

    forkJoin({
      appointments: this.appointmentService.getDoctorAppointments(),
      records: this.medicalRecordService.getDoctorRecords(),
    }).subscribe({
      next: (result) => {
        this.appointments = result.appointments;
        this.records = result.records;
        this.isLoading = false;

        const requestedAppointmentId = this.route.snapshot.queryParamMap.get('appointmentId');

        if (requestedAppointmentId && this.canCreateForAppointment(requestedAppointmentId)) {
          this.recordForm.patchValue({ appointmentId: requestedAppointmentId });
        }

        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private canCreateForAppointment(appointmentId: string): boolean {
    const appointment = this.appointments.find(item => item.id === appointmentId);

    if (!appointment || appointment.status?.toLowerCase() !== 'completed') return false;

    return !this.records.some(record => record.appointmentId === appointmentId);
  }

  private resetForm(): void {
    this.recordForm.reset({
      appointmentId: '',
      diagnosis: '',
      notes: '',
    });
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();
      if (errors.length > 0) return String(errors[0]);
    }

    return 'Không thể xử lý hồ sơ bệnh án. Vui lòng thử lại.';
  }
}
