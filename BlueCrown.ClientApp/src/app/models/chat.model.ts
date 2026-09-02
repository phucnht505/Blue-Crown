export interface ChatSession {
  id: string;
  patientId: string;
  patientName: string;
  doctorId: string | null;
  doctorName: string | null;
  aiSymptomLogId: string | null;
  appointmentId: string | null;
  appointmentType: string | null;
  appointmentStatus: string | null;
  appointmentScheduledAt: string | null;
  status: string | null;
  createdAt: string | null;
}

export interface ChatMessage {
  id: string;
  sessionId: string;
  senderId: string;
  message: string;
  isRead: boolean | null;
  sentAt: string | null;
}

export interface CreateChatSessionRequest {
  aiSymptomLogId: string | null;
}

export interface CreateChatMessageRequest {
  sessionId: string;
  message: string;
}

export interface UpdateChatSessionStatusRequest {
  status: 'active' | 'closed';
}
