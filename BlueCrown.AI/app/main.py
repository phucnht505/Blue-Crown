from fastapi import FastAPI
from app.model_service import disease_model_service
from app.schemas import PredictRequest, PredictResponse

app = FastAPI(
    title="Blue Crown AI API",
    version="1.0.0"
)

@app.get("/health")
def health():
    return {
        "status": "ok",
        "model": "PB3002/ViNMeDicalQA"
    }

@app.post("/predict", response_model=PredictResponse)
def predict(request: PredictRequest):
    return disease_model_service.predict(request.symptoms, top_k=3)