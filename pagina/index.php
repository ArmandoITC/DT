<!DOCTYPE html>
<html lang="es">
<head>
  <meta charset="UTF-8">
  <title>DT · Dashboard interactivo</title>

  <!-- Bootstrap 5 -->
  <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">

  <!-- Flatpickr (con selector de hora) -->
  <link href="https://cdn.jsdelivr.net/npm/flatpickr/dist/flatpickr.min.css" rel="stylesheet">

  <!-- Plotly -->
  <script src="https://cdn.plot.ly/plotly-latest.min.js"></script>

  <style>
    body { background:#111; color:#eee; }
    .navbar-brand { font-weight:700; }
    footer { background:#0d6efd; color:#fff; }
    footer a { color:#fff; text-decoration:underline; }
  </style>
</head>
<body>

<!-- NAVBAR -->
<nav class="navbar navbar-expand-lg navbar-dark bg-primary shadow-sm">
  <div class="container">
    <a class="navbar-brand" href="#">DT Dashboard</a>
    <button class="navbar-toggler" data-bs-toggle="collapse" data-bs-target="#nav">
      <span class="navbar-toggler-icon"></span>
    </button>
    <div class="collapse navbar-collapse" id="nav">
      <ul class="navbar-nav ms-auto">
        <li class="nav-item"><a class="nav-link active" href="#">Gráfica</a></li>
        <li class="nav-item"><a class="nav-link" href="#">Plantas</a></li>
        <li class="nav-item"><a class="nav-link" href="#">Acerca de</a></li>
      </ul>
    </div>
  </div>
</nav>

<div class="container py-5">
  <h2 class="mb-4 fw-bold">Consulta de datos DT</h2>

  <!-- ID de sesión -->
  <div class="alert alert-info" id="sesionInfo">Cargando ID de sesión…</div>

  <!-- FILTROS -->
  <div class="row g-3 align-items-end">
    <div class="col-md-4">
      <label for="fechaInicio" class="form-label">Fecha y hora de inicio</label>
      <input type="text" id="fechaInicio" class="form-control" placeholder="Selecciona fecha y hora">
    </div>
    <div class="col-md-2">
      <label for="dias" class="form-label">Días</label>
      <select id="dias" class="form-select">
        <?php for ($i = 0; $i <= 90; $i++) echo "<option value='$i'>$i</option>"; ?>
      </select>
    </div>
    <div class="col-md-2">
      <label for="horas" class="form-label">Horas</label>
      <select id="horas" class="form-select">
        <?php for ($i = 0; $i <= 23; $i++) echo "<option value='$i'>$i</option>"; ?>
      </select>
    </div>
    <div class="col-md-2">
      <button class="btn btn-success w-100" onclick="consultar()">Consultar</button>
    </div>
    <div class="col-md-2">
      <button class="btn btn-danger w-100" id="recordBtn">Grabar audio</button>
    </div>
  </div>

  <!-- GRÁFICA PRINCIPAL -->
  <div class="mt-5 rounded shadow" id="grafica" style="width:100%;height:600px;background:#222;"></div>

  <!-- SEGUNDA GRÁFICA: GDD -->
  <div class="mt-5 rounded shadow" id="graficaGDD" style="width:100%;height:500px;background:#222;"></div>
</div>

<!-- FOOTER -->
<footer class="py-3 mt-auto">
  <div class="container text-center small">
    © <?php echo date('Y'); ?> DT · Desarrollado con Plotly ·
    <a href="https://github.com/">Código fuente</a>
  </div>
</footer>

<!-- LIBRERÍAS JS -->
<script src="https://cdn.jsdelivr.net/npm/flatpickr"></script>
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>

<script>
// Flatpickr
const fp = flatpickr("#fechaInicio", {
  enableTime: true,
  time_24hr: true,
  seconds: true,
  dateFormat: "Y-m-d H:i:S",
  maxDate: "today",
  defaultDate: new Date()
});

let idSesion = null;

async function obtenerSesion() {
  const div = document.getElementById("sesionInfo");
  idSesion = sessionStorage.getItem("idSesion");
  if (!idSesion) {
    try {
      const res  = await fetch("crear_sesion.php");
      const data = await res.json();
      if (!data.idSesion) throw new Error("idSesion vacío");
      idSesion = data.idSesion;
      sessionStorage.setItem("idSesion", idSesion);
    } catch (err) {
      div.classList.replace("alert-info", "alert-danger");
      div.textContent = "Error al crear sesión";
      console.error(err);
      return;
    }
  }
  div.textContent = "ID de sesión activa: " + idSesion;
}

async function consultar() {
  fp.close();

  const inicio = fp.input.value;
  const dias   = parseInt(document.getElementById("dias").value);
  const horas  = parseInt(document.getElementById("horas").value);

  if (!inicio || (dias === 0 && horas === 0) || dias < 0 || horas < 0 || dias > 90) {
    alert("Verifica la fecha y selecciona un intervalo entre 1 hora y 90 días.");
    return;
  }

  try {
    const url = `consulta.php?inicio=${encodeURIComponent(inicio)}&dias=${dias}&horas=${horas}&t=${Date.now()}`;
    const res = await fetch(url);
    const data = await res.json();

    if (!Array.isArray(data) || !data.length) {
      document.getElementById("grafica").innerHTML = "<p class='text-warning p-3'>No hay datos.</p>";
      return;
    }

    const lab = d => `${d.anio}-${d.mes.padStart(2,'0')}-${d.dia.padStart(2,'0')} ${d.hora.padStart(2,'0')}:00`;
    const labels      = data.map(lab),
          temperatura = data.map(d => +d.temperatura),
          humedad     = data.map(d => +d.humedad),
          co2         = data.map(d => +d.co2),
          ppf         = data.map(d => +d.ppf);

    const traces = [
      { x: labels, y: temperatura, name: "Temperatura (°C)", mode: "lines+markers" },
      { x: labels, y: humedad,     name: "Humedad (%)",      mode: "lines+markers" },
      { x: labels, y: co2,         name: "CO₂ (ppm)",        mode: "lines+markers" },
      { x: labels, y: ppf,         name: "PPF (µmol m⁻² s⁻¹)", mode: "lines+markers" }
    ];

    Plotly.purge("grafica");
    Plotly.newPlot("grafica", traces, {
      title: "Promedios horarios",
      plot_bgcolor: "#222",
      paper_bgcolor: "#222",
      font: { color: "#fff" },
      xaxis: { tickangle: -45, rangeslider: { visible: true } },
      yaxis: { title: "Valor promedio" },
      hovermode: "x unified"
    }, {
      responsive: true,
      displaylogo: false
    });

    // Actualizar sesión
    const avg = arr => arr.reduce((s,v) => s+v, 0) / arr.length;
    await fetch("actualizar_sesion.php", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        sesion: idSesion,
        temperatura: avg(temperatura).toFixed(2),
        humedad:     avg(humedad).toFixed(2),
        co2:         avg(co2).toFixed(2),
        iluminacion: avg(ppf).toFixed(2)
      })
    });

    // También generar gráfica GDD
    consultarGDD(inicio, dias, horas);

  } catch (err) {
    document.getElementById("grafica").innerHTML = "<p class='text-danger p-3'>Error al procesar datos.</p>";
    console.error(err);
  }
}

async function consultarGDD(inicio, dias, horas) {
  const totalHoras = dias * 24 + horas;
  if (totalHoras < 24) {
    document.getElementById("graficaGDD").innerHTML = "<p class='text-warning p-3'>Intervalo muy corto para calcular GDD (mínimo 1 día).</p>";
    return;
  }

  try {
    const url = `consultagdd.php?inicio=${encodeURIComponent(inicio)}&dias=${dias}&horas=${horas}&t=${Date.now()}`;
    const res = await fetch(url);
    const data = await res.json();

    if (!Array.isArray(data) || !data.length || data.error) {
      document.getElementById("graficaGDD").innerHTML = "<p class='text-warning p-3'>No hay datos de GDD.</p>";
      return;
    }

    const fechas = data.map(d => d.fecha);
    const gddDiario = data.map(d => d.gdd);
    const gddAcumulado = data.map(d => d.gdd_acumulado);

    Plotly.purge("graficaGDD");
    Plotly.newPlot("graficaGDD", [
      { x: fechas, y: gddDiario, name: "GDD Diario", type: "bar" },
      { x: fechas, y: gddAcumulado, name: "GDD Acumulado", type: "scatter", mode: "lines+markers" }
    ], {
      title: "Grado Día de Crecimiento (Base 5 °C)",
      plot_bgcolor: "#222",
      paper_bgcolor: "#222",
      font: { color: "#fff" },
      xaxis: { title: "Fecha", tickangle: -45 },
      yaxis: { title: "GDD" },
      hovermode: "x unified"
    }, {
      responsive: true,
      displaylogo: false
    });

  } catch (err) {
    document.getElementById("graficaGDD").innerHTML = "<p class='text-danger p-3'>Error al obtener datos de GDD.</p>";
    console.error(err);
  }
}

// Escucha cambios
document.getElementById("dias").addEventListener("change", consultar);
document.getElementById("horas").addEventListener("change", consultar);
document.addEventListener("DOMContentLoaded", obtenerSesion);

// Cerrar sesión al salir
window.addEventListener("beforeunload", () => {
  if (idSesion) {
    navigator.sendBeacon("eliminar_sesion.php", new URLSearchParams({ sesion: idSesion }));
  }
});

// Grabación
let mediaRecorder = null;
let audioChunks   = [];

const recordBtn = document.getElementById("recordBtn");
recordBtn.addEventListener("click", toggleRecording);

async function toggleRecording() {
  if (!mediaRecorder || mediaRecorder.state === "inactive") {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      mediaRecorder = new MediaRecorder(stream);
      audioChunks = [];

      mediaRecorder.ondataavailable = e => audioChunks.push(e.data);
      mediaRecorder.onstop = async () => {
        const blob = new Blob(audioChunks, { type: "audio/webm" });
        const nombre = `grabacion_${idSesion}.webm`;
        const formData = new FormData();
        formData.append("audio", blob, nombre);

        try {
          const res = await fetch("guardar_audio.php", { method: "POST", body: formData });
          if (res.ok) {
            alert("✅ Audio guardado correctamente");
          } else {
            const txt = await res.text();
            throw new Error(txt || "Error del servidor");
          }
        } catch (err) {
          alert("❌ No se pudo guardar la grabación");
          console.error(err);
        }
      };

      mediaRecorder.start();
      recordBtn.classList.replace("btn-danger", "btn-secondary");
      recordBtn.textContent = "⏹️ Detener";

    } catch (err) {
      alert("Se denegó el permiso de micrófono o no es compatible.");
      console.error(err);
    }
  } else if (mediaRecorder.state === "recording") {
    mediaRecorder.stop();
    recordBtn.classList.replace("btn-secondary", "btn-danger");
    recordBtn.textContent = "Grabar audio";
  }
}
</script>
</body>
</html>
