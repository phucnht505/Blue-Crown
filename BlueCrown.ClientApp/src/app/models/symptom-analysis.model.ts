export interface DiseasePrediction {
  disease: string;
  confidence: number;
}

export interface SymptomLog {
  id: string;
  patientId: string | null;
  symptomsDescription: string;
  predictedDisease: string | null;
  severityLevel: string | null;
  aiAdvice: string | null;
  createdAt: string | null;
}

export interface SymptomAnalysisResponse {
  symptomLog: SymptomLog | null;
  predictedDisease: string;
  confidence: number;
  topPredictions: DiseasePrediction[];
  severityLevel: string;
  advice: string;
  isLowConfidence: boolean;
  recommendedProductId: string | null;
  recommendedProductName: string | null;
  dosageInstructions: string | null;
  shouldSeeDoctor: boolean;
  isEmergency: boolean;
}
