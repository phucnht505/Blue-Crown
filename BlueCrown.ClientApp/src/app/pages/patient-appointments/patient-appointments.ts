import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { Appointment, AppointmentDoctor, CreateAppointmentRequest } from '../../models/appointment.model';
import { AppointmentService } from '../../services/appointment.service';

@Component({
  selector: 'app-patient-appointments',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, DatePipe, DecimalPipe],
  templateUrl: './patient-appointments.html',
  styleUrl: './patient-appointments.css',
})
export class PatientAppointments implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly appointmentService = inject(AppointmentService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  appointments: Appointment[] = [];
  doctors: AppointmentDoctor[] = [];

  isLoading = true;
  isSaving = false;
  cancellingAppointmentId: string | null = null;
  errorMessage = '';
  successMessage = '';
  dateErrorMessage = '';

  readonly minDateTime = this.toDateTimeLocal(new Date());

  appointmentForm = this.formBuilder.nonNullable.group({
    doctorId: ['', Validators.required],
    scheduledAt: ['', Validators.required],
    type: ['', Validators.required],
  });

  get doctorId() {
    return this.appointmentForm.controls.doctorId;
  }

  get scheduledAt() {
    return this.appointmentForm.controls.scheduledAt;
  }

  get type() {
    return this.appointmentForm.controls.type;
  }

  get selectedDoctor(): AppointmentDoctor | null {
    return this.doctors.find(doctor => doctor.id === this.doctorId.value) ?? null;
  }

  get isOnlineConsult(): boolean {
    return this.type.value === 'online_consult';
  }

  get upcomingAppointments(): Appointment[] {
    const now = Date.now();

    return this.appointments
      .filter(appointment => {
        const status = appointment.status?.toLowerCase();
        return new Date(appointment.scheduledAt).getTime() > now && (status === 'pending' || status === 'confirmed');
      })
      .sort((a, b) => new Date(a.scheduledAt).getTime() - new Date(b.scheduledAt).getTime());
  }

  get appointmentHistory(): Appointment[] {
    const now = Date.now();

    return this.appointments
      .filter(appointment => {
        const status = appointment.status?.toLowerCase();
        return new Date(appointment.scheduledAt).getTime() <= now || status === 'completed' || status === 'cancelled';
      })
      .sort((a, b) => new Date(b.scheduledAt).getTime() - new Date(a.scheduledAt).getTime());
  }

  ngOnInit(): void {
    this.loadData();
  }

  save(): void {
    this.clearMessages();
    this.appointmentForm.markAllAsTouched();

    if (this.appointmentForm.invalid) {
      return;
    }

    const value = this.appointmentForm.getRawValue();
    const scheduledDate = new Date(value.scheduledAt);

    if (Number.isNaN(scheduledDate.getTime()) || scheduledDate.getTime() <= Date.now()) {
      this.dateErrorMessage = 'Thời gian khám phải nằm trong tương lai.';
      return;
    }

    const request: CreateAppointmentRequest = {
      doctorId: value.doctorId,
      scheduledAt: scheduledDate.toISOString(),
      type: value.type,
    };

    this.isSaving = true;

    this.appointmentService.create(request).subscribe({
      next: (appointment) => {
        this.appointments = [appointment, ...this.appointments];
        this.successMessage = value.type === 'online_consult'
          ? 'Đặt lịch tư vấn trực tuyến miễn phí thành công.'
          : 'Đặt lịch khám thành công.';
        this.isSaving = false;

        this.appointmentForm.reset({
          doctorId: '',
          scheduledAt: '',
          type: '',
        });

        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        console.error('Lỗi đặt lịch khám:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSaving = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  cancelAppointment(appointment: Appointment): void {
    const confirmed = window.confirm(`Bạn có chắc chắn muốn hủy lịch khám với bác sĩ ${appointment.doctorName}?`);

    if (!confirmed) {
      return;
    }

    this.clearMessages();
    this.cancellingAppointmentId = appointment.id;

    this.appointmentService.cancel(appointment.id).subscribe({
      next: (response) => {
        this.appointments = this.appointments.filter(item => item.id !== appointment.id);
        this.successMessage = response.message;
        this.cancellingAppointmentId = null;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        console.error('Lỗi hủy lịch khám:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.cancellingAppointmentId = null;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  canCancel(appointment: Appointment): boolean {
    return appointment.status?.toLowerCase() === 'pending' && new Date(appointment.scheduledAt).getTime() > Date.now();
  }

  isAppointmentFree(appointment: Appointment): boolean {
    return appointment.type?.toLowerCase() === 'online_consult';
  }

  getTypeText(type: string | null): string {
    switch (type?.toLowerCase()) {
      case 'online_consult':
        return 'Tư vấn trực tuyến';
      case 'clinic_visit':
        return 'Khám tại phòng khám';
      default:
        return 'Không xác định';
    }
  }

  getStatusText(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'confirmed':
        return 'Đã xác nhận';
      case 'completed':
        return 'Hoàn thành';
      case 'cancelled':
        return 'Đã hủy';
      default:
        return 'Chờ xác nhận';
    }
  }

  getStatusClass(status: string | null): string {
    switch (status?.toLowerCase()) {
      case 'confirmed':
        return 'status-confirmed';
      case 'completed':
        return 'status-completed';
      case 'cancelled':
        return 'status-cancelled';
      default:
        return 'status-pending';
    }
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.dateErrorMessage = '';
  }

  private loadData(): void {
    this.isLoading = true;
    this.errorMessage = '';

    forkJoin({
      doctors: this.appointmentService.getDoctors(),
      appointments: this.appointmentService.getMyAppointments(),
    }).subscribe({
      next: (result) => {
        this.doctors = result.doctors;
        this.appointments = result.appointments;
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: (error) => {
        console.error('Lỗi tải dữ liệu lịch khám:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private toDateTimeLocal(date: Date): string {
    const offset = date.getTimezoneOffset();
    const localDate = new Date(date.getTime() - offset * 60000);
    return localDate.toISOString().slice(0, 16);
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') {
      return error.error;
    }

    if (error?.error?.message) {
      return error.error.message;
    }

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();

      if (errors.length > 0) {
        return String(errors[0]);
      }
    }

    return 'Không thể xử lý lịch khám. Vui lòng thử lại.';
  }
}
