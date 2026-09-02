import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnDestroy, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { interval, Subscription } from 'rxjs';
import { ChatMessage, ChatSession } from '../../models/chat.model';
import { AuthService } from '../../services/auth.service';
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-chat-room',
  standalone: true,
  imports: [FormsModule, DatePipe],
  templateUrl: './chat-room.html',
  styleUrl: './chat-room.css',
})
export class ChatRoom implements OnInit, OnDestroy {
  private readonly chatService = inject(ChatService);
  private readonly authService = inject(AuthService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly changeDetectorRef = inject(ChangeDetectorRef);
  private pollingSubscription?: Subscription;

  sessionId = '';
  session: ChatSession | null = null;
  messages: ChatMessage[] = [];
  messageText = '';
  isLoading = true;
  isSending = false;
  isClosing = false;
  errorMessage = '';
  fieldError = '';

  get currentUserId(): string {
    return this.authService.getCurrentUser()?.userId ?? '';
  }

  get isDoctor(): boolean {
    return this.authService.getCurrentUser()?.role?.toLowerCase() === 'doctor';
  }

  get isClosed(): boolean {
    return this.session?.status?.toLowerCase() === 'closed';
  }

  get canCloseSession(): boolean {
    if (!this.session || this.isClosed) return false;
    if (this.isDoctor) return true;
    return !this.session.appointmentId;
  }

  get canCreateOnlinePrescription(): boolean {
    return this.isDoctor &&
      this.isClosed &&
      !!this.session?.appointmentId &&
      this.session.appointmentType?.toLowerCase() === 'online_consult' &&
      this.session.appointmentStatus?.toLowerCase() === 'completed';
  }

  ngOnInit(): void {
    this.sessionId = this.route.snapshot.paramMap.get('id') ?? '';

    if (!this.sessionId) {
      this.errorMessage = 'Chat Session không hợp lệ.';
      this.isLoading = false;
      return;
    }

    this.loadSession();
  }

  ngOnDestroy(): void {
    this.pollingSubscription?.unsubscribe();
  }

  sendMessage(): void {
    this.fieldError = '';
    this.errorMessage = '';

    if (this.isClosed) {
      this.fieldError = 'Cuộc tư vấn đã kết thúc.';
      return;
    }

    const message = this.messageText.trim();

    if (!message) {
      this.fieldError = 'Vui lòng nhập nội dung tin nhắn.';
      return;
    }

    if (message.length > 2000) {
      this.fieldError = 'Tin nhắn không được vượt quá 2000 ký tự.';
      return;
    }

    this.isSending = true;

    this.chatService.sendMessage({ sessionId: this.sessionId, message }).subscribe({
      next: createdMessage => {
        this.messages = [...this.messages, createdMessage];
        this.messageText = '';
        this.isSending = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isSending = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  closeSession(): void {
    if (!this.session || !this.canCloseSession) return;

    if (!window.confirm('Bạn có chắc chắn muốn kết thúc cuộc tư vấn này?')) return;

    this.isClosing = true;
    this.errorMessage = '';

    this.chatService.updateSessionStatus(this.session.id, { status: 'closed' }).subscribe({
      next: () => {
        this.refreshSessionAfterClose();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isClosing = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  goToOnlinePrescription(): void {
    const appointmentId = this.session?.appointmentId;

    if (!appointmentId) {
      this.errorMessage = 'Không xác định được lịch tư vấn trực tuyến.';
      return;
    }

    this.router.navigate(['/doctor/prescriptions'], { queryParams: { appointmentId } });
  }

  goBack(): void {
    const role = this.authService.getCurrentUser()?.role?.toLowerCase();

    if (role === 'doctor') {
      this.router.navigate(['/doctor/chat']);
      return;
    }

    this.router.navigate(['/patient/chat']);
  }

  isOwnMessage(message: ChatMessage): boolean {
    return message.senderId === this.currentUserId;
  }

  private loadSession(): void {
    this.chatService.getSessionById(this.sessionId).subscribe({
      next: session => {
        this.session = session;
        this.loadMessages(true);
        this.startPolling();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private refreshSessionAfterClose(): void {
    this.chatService.getSessionById(this.sessionId).subscribe({
      next: session => {
        this.session = session;
        this.isClosing = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        this.errorMessage = this.getApiErrorMessage(error);
        this.isClosing = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private refreshSession(): void {
    this.chatService.getSessionById(this.sessionId).subscribe({
      next: session => {
        this.session = session;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private loadMessages(showLoading: boolean): void {
    if (showLoading) this.isLoading = true;

    this.chatService.getMessages(this.sessionId).subscribe({
      next: messages => {
        this.messages = messages;
        this.markUnreadMessages(messages);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
      error: error => {
        if (showLoading) this.errorMessage = this.getApiErrorMessage(error);
        this.isLoading = false;
        this.changeDetectorRef.detectChanges();
      },
    });
  }

  private startPolling(): void {
    this.pollingSubscription?.unsubscribe();

    this.pollingSubscription = interval(3000).subscribe(() => {
      if (!this.isClosed) {
        this.loadMessages(false);
        this.refreshSession();
      }
    });
  }

  private markUnreadMessages(messages: ChatMessage[]): void {
    const unreadMessages = messages.filter(message => message.senderId !== this.currentUserId && message.isRead !== true);

    for (const message of unreadMessages) {
      this.chatService.markMessageAsRead(message.id).subscribe({
        next: () => message.isRead = true,
      });
    }
  }

  private getApiErrorMessage(error: any): string {
    if (typeof error?.error === 'string') return error.error;
    if (error?.error?.message) return error.error.message;
    return 'Không thể xử lý yêu cầu. Vui lòng thử lại.';
  }
}
