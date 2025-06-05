import pexpect
import time
from datetime import datetime

# Datos de conexión
ssh_usuario = "armando"
ssh_host = "67.217.242.228"
ssh_password = "CONtrasena20."  # ?? ALMACENA CON CUIDADO
puerto_local = "9999"
host_remoto = "74.208.36.209"
puerto_remoto = "5432"

# Comando SSH con redirección de puerto
comando = f"ssh -L {puerto_local}:{host_remoto}:{puerto_remoto} {ssh_usuario}@{ssh_host} -N"

while True:
    with open("/var/www/html/Unity/tunel.log", "a") as f:
        f.write(f"[{datetime.now()}] Intentando abrir túnel SSH...\n")

    child = pexpect.spawn(comando)

    try:
        i = child.expect(["password:", "Permission denied", pexpect.EOF, pexpect.TIMEOUT], timeout=15)
        
        if i == 0:
            child.sendline(ssh_password)
            with open("/var/www/html/Unity/tunel.log", "a") as f:
                f.write(f"[{datetime.now()}] Túnel SSH abierto. Esperando que se mantenga activo...\n")

            # Espera mientras el túnel siga vivo
            while child.isalive():
                time.sleep(10)

            with open("/var/www/html/Unity/tunel.log", "a") as f:
                f.write(f"[{datetime.now()}] El túnel se cerró. Reintentando en 30 segundos...\n")
            time.sleep(30)

        elif i == 1:
            with open("/var/www/html/Unity/tunel.log", "a") as f:
                f.write(f"[{datetime.now()}] ERROR: Contraseña incorrecta.\n")
            break

        else:
            with open("/var/www/html/Unity/tunel.log", "a") as f:
                f.write(f"[{datetime.now()}] ERROR: No se pudo establecer el túnel. Reintentando en 30 segundos.\n")
            time.sleep(30)

    except Exception as e:
        with open("/var/www/html/Unity/tunel.log", "a") as f:
            f.write(f"[{datetime.now()}] ERROR inesperado: {str(e)}\n")
        time.sleep(30)