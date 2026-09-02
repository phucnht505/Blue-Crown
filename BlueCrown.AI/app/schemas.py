from pydantic import BaseModel, Field

class PredictRequest(BaseModel):
    symptoms: str = Field(min_length=5, max_length=2000)

class DiseasePrediction(BaseModel):
    disease: str
    confidence: float

class PredictResponse(BaseModel):
    predicted_disease: str
    confidence: float
    top_predictions: list[DiseasePrediction]