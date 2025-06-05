import os
from faster_whisper import WhisperModel
import ffmpeg

# Rutas
ruta = "/var/www/html/Unity/grabaciones"
modelo = WhisperModel("base", compute_type="int8")

# Función para convertir y transcribir
def transcribir(archivo_webm):
    base = os.path.splitext(archivo_webm)[0]
    wav_path = os.path.join(ruta, base + ".wav")
    txt_path = os.path.join(ruta, base + ".txt")
    webm_path = os.path.join(ruta, archivo_webm)

    # Convertir a WAV mono 16kHz
    ffmpeg.input(webm_path).output(wav_path, ac=1, ar='16000').run(overwrite_output=True)

    # Transcribir
    segments, _ = modelo.transcribe(wav_path, language="es")

    with open(txt_path, "w", encoding="utf-8") as f:
        for segment in segments:
            f.write(segment.text.strip() + "\n")

    os.remove(wav_path)
    print(f"✅ Transcripción guardada: {txt_path}")

# Procesar todos los .webm
for f in os.listdir(ruta):
    if f.endswith(".webm"):
        transcribir(f)
