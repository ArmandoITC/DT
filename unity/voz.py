import socket
import edge_tts
import asyncio
from io import BytesIO
from pydub import AudioSegment
from pydub.playback import play

def sintetizar_y_reproducir(texto, voz="es-MX-JorgeNeural"):
    async def generar():
        # Envolvemos el texto en <speak> para que se interprete como SSML, pero sin <voice>
        ssml = f"{texto}"

        # Pasamos la voz correctamente aquí
        tts = edge_tts.Communicate(text=ssml, voice=voz)
        stream = tts.stream()
        audio_bytes = bytearray()

        async for chunk in stream:
            if chunk["type"] == "audio":
                audio_bytes += chunk["data"]

        if not audio_bytes:
            print("No se recibió audio del motor TTS.")
            return

        audio = AudioSegment.from_file(BytesIO(audio_bytes), format="mp3")
        play(audio)

    try:
        loop = asyncio.get_running_loop()
    except RuntimeError:
        loop = None

    if loop and loop.is_running():
        task = loop.create_task(generar())
        loop.run_until_complete(task)
    else:
        asyncio.run(generar())

# Servidor TCP
HOST = '127.0.0.1'
PORT = 50007

with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((HOST, PORT))
    s.listen(1)
    print("Servidor Edge TTS escuchando en el puerto 50007...")

    while True:
        conn, addr = s.accept()
        with conn:
            print(f"Conexión desde: {addr}")
            data = conn.recv(8192)
            if not data:
                continue
            texto_recibido = data.decode('utf-8')
            print(f"Texto recibido:\n{texto_recibido}\n")
            sintetizar_y_reproducir(texto_recibido)

