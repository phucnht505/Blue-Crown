import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { ChatSession } from '../../models/chat.model';
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-patient-chat',
  standalone: true,
  imports: [DatePipe, RouterLink],
  templateUrl: './patient-chat.html',
  styleUrl: './patient-chat.css',
})
export class PatientChat implements OnInit {
  private readonly chatService = inject(ChatService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);
  private readonly router = inject(Router);

  sessions: ChatSession[] = [];
  isLoading = true;
  isCreating = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit(): void {
    this.loadSessions();
  }

  get activeSession(): ChatSession | null {
    return this.sessions.find(session => session.status?.toLowerCase() === 'active') ?? null;
  }

  createSession(): void {
    this.errorMessage = '';
    this.successMessage = '';

    if (this.activeSession) {
      this.errorMessage = 'Bạn đang có một cuộc tư vấn chưa kết thúc.';
      return;
    }

    if (!window.confirm('Bạn muốn tạo yêu cầu tư vấn trực tuyến với bác sĩ?'))
      return;

    this.isCreating = true;

    this.chatService.createSession({ aiSymptomLogId: null }).subscribe({
      next: session => {
        this.sessions = [session, ...this.sessions];
        this.successMessage = 'Đã tạo yêu cầu tư vấn. Vui lòng chờ bác sĩ tiếp nhận.';
        this.isCreating = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isCreating = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  openChat(session: ChatSession): void {
    this.errorMessage = '';

    if (!session.id) {
      this.errorMessage = 'Chat Session không hợp lệ.';
      return;
    }

    this.router.navigate(['/patient/chat', session.id]);
  }

  getStatusText(status: string | null): string {
    return status?.toLowerCase() === 'closed' ? 'Đã kết thúc' : 'Đang hoạt động';
  }

  getStatusClass(status: string | null): string {
    return status?.toLowerCase() === 'closed' ? 'status-closed' : 'status-active';
  }

  private loadSessions(): void {
    this.isLoading = true;

    this.chatService.getPatientSessions().subscribe({
      next: sessions => {
        this.sessions = sessions;
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;
    return 'Không thể xử lý yêu cầu. Vui lòng thử lại.';
  }
}
