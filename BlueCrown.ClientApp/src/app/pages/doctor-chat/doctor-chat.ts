import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ChatSession } from '../../models/chat.model';
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-doctor-chat',
  standalone: true,
  imports: [DatePipe, RouterLink],
  templateUrl: './doctor-chat.html',
  styleUrl: './doctor-chat.css',
})
export class DoctorChat implements OnInit {
  private readonly chatService = inject(ChatService);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);
  private readonly router = inject(Router);

  availableSessions: ChatSession[] = [];
  mySessions: ChatSession[] = [];
  acceptingSessionId: string | null = null;
  isLoading = true;
  errorMessage = '';

  ngOnInit(): void {
    this.loadSessions();
  }

  acceptSession(session: ChatSession): void {
    this.errorMessage = '';

    if (!window.confirm(`Nhận tư vấn cho Patient "${session.patientName}"?`))
      return;

    this.acceptingSessionId = session.id;

    this.chatService.assignDoctor(session.id).subscribe({
      next: () => {
        this.acceptingSessionId = null;
        this.router.navigate(['/doctor/chat', session.id]);
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.acceptingSessionId = null;
        this.loadSessions();
      },
    });
  }

  getStatusText(status: string | null): string {
    return status?.toLowerCase() === 'closed' ? 'Đã kết thúc' : 'Đang hoạt động';
  }

  private loadSessions(): void {
    this.isLoading = true;

    forkJoin({
      available: this.chatService.getAvailableSessions(),
      mine: this.chatService.getDoctorSessions(),
    }).subscribe({
      next: result => {
        this.availableSessions = result.available;
        this.mySessions = result.mine;
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
    return 'Không thể tải danh sách tư vấn. Vui lòng thử lại.';
  }
}
