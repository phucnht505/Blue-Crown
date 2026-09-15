from pathlib import Path
import time
import numpy as np
import onnxruntime as ort
from transformers import AutoTokenizer

BASE_DIR = Path(__file__).resolve().parent.parent
MODEL_DIR = BASE_DIR / "models" / "ViNMeDicalQA-onnx"
MODEL_PATH = MODEL_DIR / "model.onnx"

class DiseaseModelService:
    def __init__(self):
        if not MODEL_PATH.exists():
            raise RuntimeError(f"Khong tim thay ONNX model tai: {MODEL_PATH}")

        print("[AI] Loading tokenizer...")
        self.tokenizer = AutoTokenizer.from_pretrained(MODEL_DIR, local_files_only=True)

        print("[AI] Loading ONNX Runtime model...")

        session_options = ort.SessionOptions()
        session_options.graph_optimization_level = ort.GraphOptimizationLevel.ORT_ENABLE_ALL

        self.session = ort.InferenceSession(
            str(MODEL_PATH),
            sess_options=session_options,
            providers=["CPUExecutionProvider"]
        )

        self.input_names = {item.name for item in self.session.get_inputs()}
        self.id2label = self._load_labels()

        print("[AI] ONNX model loaded.")
        print(f"[AI] Inputs: {self.input_names}")

        self._warm_up()

    def _load_labels(self) -> dict:
        import json

        config_path = MODEL_DIR / "config.json"

        with open(config_path, "r", encoding="utf-8") as file:
            config = json.load(file)

        id2label = config.get("id2label", {})

        return {int(key): value for key, value in id2label.items()}

    def _warm_up(self):
        print("[AI] Warming up model...")

        inputs = self._tokenize("Tôi cảm thấy mệt mỏi và đau đầu.")

        start = time.perf_counter()
        self.session.run(None, inputs)
        elapsed = time.perf_counter() - start

        print(f"[AI] Warm-up completed in {elapsed:.2f} seconds.")

    def _tokenize(self, text: str) -> dict:
        encoded = self.tokenizer(
            text,
            return_tensors="np",
            truncation=True,
            max_length=160
        )

        return {
            key: value.astype(np.int64)
            for key, value in encoded.items()
            if key in self.input_names
        }

    def predict(self, symptoms: str, top_k: int = 3) -> dict:
        text = symptoms.strip()

        print(f"[AI] Predict start: {text[:80]}")

        total_start = time.perf_counter()

        tokenize_start = time.perf_counter()
        inputs = self._tokenize(text)
        tokenize_time = time.perf_counter() - tokenize_start

        inference_start = time.perf_counter()
        outputs = self.session.run(None, inputs)
        inference_time = time.perf_counter() - inference_start

        logits = outputs[0][0]

        logits = logits - np.max(logits)
        probabilities = np.exp(logits)
        probabilities = probabilities / probabilities.sum()

        top_k = min(top_k, len(probabilities))
        indices = np.argsort(probabilities)[::-1][:top_k]

        predictions = []

        for index in indices:
            index = int(index)

            predictions.append({
                "disease": self.id2label.get(index, f"LABEL_{index}"),
                "confidence": round(float(probabilities[index]), 6)
            })

        total_time = time.perf_counter() - total_start

        print(f"[AI] Tokenization: {tokenize_time:.3f}s")
        print(f"[AI] Inference: {inference_time:.3f}s")
        print(f"[AI] Total prediction: {total_time:.3f}s")

        return {
            "predicted_disease": predictions[0]["disease"],
            "confidence": predictions[0]["confidence"],
            "top_predictions": predictions
        }

disease_model_service = DiseaseModelService()