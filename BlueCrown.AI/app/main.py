import os

from fastapi import FastAPI, Header, HTTPException

from app.model_service import disease_model_service
from app.schemas import PredictRequest, PredictResponse


app = FastAPI(
    title="Blue Crown AI API",
    version="1.0.0"
)


def verify_api_key(x_api_key: str | None) -> None:
    expected_key = os.getenv("AI_API_KEY")

    if not expected_key:
        return

    if x_api_key != expected_key:
        raise HTTPException(
            status_code=401,
            detail="Invalid AI API key."
        )


@app.get("/health")
def health():
    return {
        "status": "ok",
        "model": "PB3002/ViNMeDicalQA"
    }


@app.post("/predict", response_model=PredictResponse)
def predict(
    request: PredictRequest,
    x_api_key: str | None = Header(default=None)
):
    verify_api_key(x_api_key)

    return disease_model_service.predict(
        request.symptoms,
        top_k=3
    )