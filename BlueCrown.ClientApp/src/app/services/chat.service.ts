import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { ChatMessage, ChatSession, CreateChatMessageRequest, CreateChatSessionRequest, UpdateChatSessionStatusRequest } from '../models/chat.model';

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private readonly http = inject(HttpClient);
  private readonly sessionApiUrl = '/api/ChatSession';
  private readonly messageApiUrl = '/api/ChatMessage';

  getPatientSessions(): Observable<ChatSession[]> {
    return this.http.get<ChatSession[]>(`${this.sessionApiUrl}/my`);
  }

  getDoctorSessions(): Observable<ChatSession[]> {
    return this.http.get<ChatSession[]>(`${this.sessionApiUrl}/doctor`);
  }

  getAvailableSessions(): Observable<ChatSession[]> {
    return this.http.get<ChatSession[]>(`${this.sessionApiUrl}/doctor/available`);
  }

  getSessionById(id: string): Observable<ChatSession> {
    return this.http.get<ChatSession>(`${this.sessionApiUrl}/${id}`);
  }

  createSession(request: CreateChatSessionRequest): Observable<ChatSession> {
    return this.http.post<ChatSession>(this.sessionApiUrl, request);
  }

  assignDoctor(id: string): Observable<void> {
    return this.http.put<void>(`${this.sessionApiUrl}/${id}/assign-doctor`, {});
  }

  updateSessionStatus(id: string, request: UpdateChatSessionStatusRequest): Observable<void> {
    return this.http.put<void>(`${this.sessionApiUrl}/${id}/status`, request);
  }

  getMessages(sessionId: string): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.messageApiUrl}/session/${sessionId}`);
  }

  sendMessage(request: CreateChatMessageRequest): Observable<ChatMessage> {
    return this.http.post<ChatMessage>(this.messageApiUrl, request);
  }

  markMessageAsRead(id: string): Observable<void> {
    return this.http.put<void>(`${this.messageApiUrl}/${id}/read`, {});
  }
}

