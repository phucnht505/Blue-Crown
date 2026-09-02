import os

os.environ["HF_HOME"] = r"D:\253OU\DAN\BlueCrown.Api\BlueCrown.AI\.hf_cache"

from huggingface_hub import snapshot_download

MODEL_ID = "PB3002/ViNMeDicalQA"
MODEL_DIR = r"D:\253OU\DAN\BlueCrown.Api\BlueCrown.AI\models\ViNMeDicalQA"

snapshot_download(repo_id=MODEL_ID, local_dir=MODEL_DIR)

print("Downloaded model:")
print(MODEL_DIR)