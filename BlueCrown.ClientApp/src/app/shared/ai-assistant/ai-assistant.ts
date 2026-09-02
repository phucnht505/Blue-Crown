import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectorRef, Component, inject, OnDestroy } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { finalize, Subscription, timeout } from 'rxjs';
import { SymptomAnalysisResponse } from '../../models/symptom-analysis.model';
import { AuthService } from '../../services/auth.service';
import { SymptomAiService } from '../../services/symptom-ai.service';

@Component({
  selector: 'app-ai-assistant',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './ai-assistant.html',
  styleUrl: './ai-assistant.css'
})
export class AiAssistant implements OnDestroy {
  private readonly symptomAiService = inject(SymptomAiService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);

  isOpen = false;
  symptomsDescription = '';
  isLoading = false;
  errorMessage = '';
  showAccountPrompt = false;
  result: SymptomAnalysisResponse | null = null;

  private readonly authSubscription: Subscription;

  constructor() {
    this.authSubscription = this.authService.currentUser$.subscribe(() => {
      this.resetAnalysis();
    });
  }

  toggle(): void {
    this.isOpen = !this.isOpen;
  }

  close(): void {
    this.isOpen = false;
    this.showAccountPrompt = false;
  }

  analyze(): void {
    const symptoms = this.symptomsDescription.trim();

    if (!symptoms) {
      this.errorMessage = 'Vui lòng nhập triệu chứng.';
      return;
    }

    if (symptoms.length < 5) {
      this.errorMessage = 'Mô tả triệu chứng phải có ít nhất 5 ký tự.';
      return;
    }

    if (symptoms.length > 2000) {
      this.errorMessage = 'Mô tả triệu chứng không được vượt quá 2000 ký tự.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.result = null;
    this.showAccountPrompt = false;

    this.symptomAiService.analyze(symptoms)
      .pipe(
        timeout(15000),
        finalize(() => {
          this.isLoading = false;
          this.cdr.markForCheck();
        })
      )
      .subscribe({
        next: (result: SymptomAnalysisResponse) => {
          this.result = result;
          this.cdr.markForCheck();
        },
        error: (error: HttpErrorResponse | Error) => {
          if (error.name === 'TimeoutError') {
            this.errorMessage = 'Hệ thống phản hồi quá lâu. Vui lòng thử lại.';
          } else if (error instanceof HttpErrorResponse) {
            this.errorMessage = error.error?.message || 'Không thể phân tích triệu chứng. Vui lòng thử lại.';
          } else {
            this.errorMessage = 'Không thể phân tích triệu chứng. Vui lòng thử lại.';
          }

          this.cdr.markForCheck();
        }
      });
  }

  consultDoctor(): void {
    if (!this.authService.isAuthenticated()) {
      this.showAccountPrompt = true;
      return;
    }

    const user = this.authService.getCurrentUser();

    if (user?.role?.toLowerCase() !== 'patient') {
      this.errorMessage = 'Tính năng tư vấn bác sĩ trực tuyến hiện dành cho tài khoản bệnh nhân.';
      return;
    }

    this.router.navigate(['/patient/chat']);
  }

  goToLogin(): void {
    this.showAccountPrompt = false;
    this.router.navigate(['/login'], {
      queryParams: { returnUrl: '/patient/chat' }
    });
  }

  goToRegister(): void {
    this.showAccountPrompt = false;
    this.router.navigate(['/register'], {
      queryParams: { returnUrl: '/patient/chat' }
    });
  }

  closeAccountPrompt(): void {
    this.showAccountPrompt = false;
  }

  clear(): void {
    this.resetAnalysis();
  }

  formatConfidence(value: number): string {
    return `${(value * 100).toFixed(1)}%`;
  }

  getSeverityLabel(level: string): string {
    switch (level) {
      case 'high':
        return 'Cao';
      case 'medium':
        return 'Trung bình';
      default:
        return 'Thấp';
    }
  }

  private resetAnalysis(): void {
    this.symptomsDescription = '';
    this.result = null;
    this.errorMessage = '';
    this.isLoading = false;
    this.showAccountPrompt = false;
    this.cdr.markForCheck();
  }

  ngOnDestroy(): void {
    this.authSubscription.unsubscribe();
  }
}
