import time
import psycopg2
from datetime import datetime

# Configura tus datos de conexión
conexion = psycopg2.connect(
    host="66.179.211.230",
    dbname="Unity",
    user="postgres",
    password="Armando18",
    port=5432
)

cursor = conexion.cursor()

def ejecutar_limpieza():
    try:
        cursor.execute("SELECT eliminar_sesiones_inactivas_1h();")
        conexion.commit()
        print(f"[{datetime.now()}] Sesiones inactivas eliminadas correctamente.")
    except Exception as e:
        print(f"[{datetime.now()}] ERROR: {e}")
        conexion.rollback()

# Bucle principal: cada 30 minutos
while True:
    ejecutar_limpieza()
    time.sleep(1800)  # 1800 segundos = 30 minutos
