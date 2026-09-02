import { ChangeDetectorRef, Component, inject, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Appointment } from '../../models/appointment.model';
import { AppointmentService } from '../../services/appointment.service';

@Component({
  selector: 'app-doctor-appointments',
  standalone: true,
  imports: [RouterLink, DatePipe],
  templateUrl: './doctor-appointments.html',
  styleUrl: './doctor-appointments.css',
})
export class DoctorAppointments implements OnInit {
  private readonly appointmentService = inject(AppointmentService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);

  appointments: Appointment[] = [];
  isLoading = true;
  processingAppointmentId: string | null = null;
  errorMessage = '';
  successMessage = '';

  get pendingAppointments(): Appointment[] {
    return this.appointments
      .filter(appointment => appointment.status?.toLowerCase() === 'pending')
      .sort((a, b) => new Date(a.scheduledAt).getTime() - new Date(b.scheduledAt).getTime());
  }

  get confirmedAppointments(): Appointment[] {
    return this.appointments
      .filter(appointment => appointment.status?.toLowerCase() === 'confirmed')
      .sort((a, b) => new Date(a.scheduledAt).getTime() - new Date(b.scheduledAt).getTime());
  }

  get historyAppointments(): Appointment[] {
    return this.appointments
      .filter(appointment => {
        const status = appointment.status?.toLowerCase();
        return status === 'completed' || status === 'cancelled';
      })
      .sort((a, b) => new Date(b.scheduledAt).getTime() - new Date(a.scheduledAt).getTime());
  }

  ngOnInit(): void {
    this.loadAppointments();
  }

  isOnlineConsult(appointment: Appointment): boolean {
    return appointment.type?.toLowerCase() === 'online_consult';
  }

  isClinicVisit(appointment: Appointment): boolean {
    return appointment.type?.toLowerCase() === 'clinic_visit';
  }

  canConfirm(appointment: Appointment): boolean {
    return new Date(appointment.scheduledAt).getTime() > Date.now();
  }

  canComplete(appointment: Appointment): boolean {
    return this.isClinicVisit(appointment) && new Date(appointment.scheduledAt).getTime() <= Date.now();
  }

  confirmAppointment(appointment: Appointment): void {
    const action = this.isOnlineConsult(appointment) ? 'lịch tư vấn trực tuyến' : 'lịch khám';
    if (!window.confirm(`Xác nhận ${action} của bệnh nhân ${appointment.patientName}?`)) return;
    this.updateStatus(appointment, 'confirmed');
  }

  rejectAppointment(appointment: Appointment): void {
    if (!window.confirm(`Bạn có chắc chắn muốn từ chối lịch của bệnh nhân ${appointment.patientName}?`)) return;
    this.updateStatus(appointment, 'cancelled');
  }

  completeAppointment(appointment: Appointment): void {
    if (this.isOnlineConsult(appointment)) {
      this.errorMessage = 'Tư vấn trực tuyến phải được kết thúc trong phòng Chat.';
      return;
    }

    if (!window.confirm(`Xác nhận đã hoàn thành buổi khám của bệnh nhân ${appointment.patientName}?`)) return;
    this.updateStatus(appointment, 'completed');
  }

  cancelConfirmedAppointment(appointment: Appointment): void {
    if (!window.confirm(`Bạn có chắc chắn muốn hủy lịch đã xác nhận của bệnh nhân ${appointment.patientName}?`)) return;
    this.updateStatus(appointment, 'cancelled');
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

  getTimeLabel(appointment: Appointment): string {
    return this.isOnlineConsult(appointment) ? 'Thời gian tư vấn' : 'Thời gian khám';
  }

  private updateStatus(appointment: Appointment, status: 'confirmed' | 'cancelled' | 'completed'): void {
    this.errorMessage = '';
    this.successMessage = '';
    this.processingAppointmentId = appointment.id;

    this.appointmentService.updateDoctorStatus(appointment.id, status).subscribe({
      next: updatedAppointment => {
        this.appointments = this.appointments.map(item => item.id === updatedAppointment.id ? updatedAppointment : item);
        this.successMessage = this.getSuccessMessage(status);
        this.processingAppointmentId = null;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        console.error('Lỗi cập nhật trạng thái lịch khám:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.processingAppointmentId = null;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private loadAppointments(): void {
    this.appointmentService.getDoctorAppointments().subscribe({
      next: appointments => {
        this.appointments = appointments;
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        console.error('Lỗi tải lịch khám Doctor:', error);
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private getSuccessMessage(status: string): string {
    switch (status) {
      case 'confirmed':
        return 'Đã xác nhận lịch.';
      case 'completed':
        return 'Đã hoàn thành lịch khám.';
      default:
        return 'Đã hủy lịch.';
    }
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;

    if (error?.error?.errors) {
      const errors = Object.values(error.error.errors).flat();
      if (errors.length > 0) return String(errors[0]);
    }

    return 'Không thể xử lý lịch khám. Vui lòng thử lại.';
  }
}
