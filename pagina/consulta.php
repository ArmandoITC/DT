<?php
header('Content-Type: application/json');

require_once 'Database.php';
$db = new Database();

/* -------------------------------------------------
   1. Leer y validar parámetros
--------------------------------------------------*/
$inicio = $_GET['inicio'] ?? null;
$dias   = isset($_GET['dias'])  ? intval($_GET['dias'])  : 0;
$horas  = isset($_GET['horas']) ? intval($_GET['horas']) : 0;

/* Formato esperado:
   - AAAA-MM-DD HH:MM
   - AAAA-MM-DD HH:MM:SS
*/
$regexFechaHora = '/^\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}(:\d{2})?$/';

if (!$inicio || !preg_match($regexFechaHora, $inicio)) {
    echo json_encode(["error" => "Fecha de inicio inválida. Usa formato YYYY-MM-DD HH:MM o HH:MM:SS."]);
    exit;
}

$totalHoras = $dias * 24 + $horas;
if ($totalHoras < 1 || $totalHoras > 2160) {
    echo json_encode(["error" => "El intervalo debe estar entre 1 hora y 2160 horas."]);
    exit;
}

/* -------------------------------------------------
   2. Calcular rango de consulta
--------------------------------------------------*/
try {
    $fecha_inicio = new DateTime($inicio);
} catch (Exception $e) {
    echo json_encode(["error" => "Error al interpretar la fecha y hora."]);
    exit;
}

$fecha_fin = clone $fecha_inicio;
$fecha_fin->modify("+$totalHoras hours");

$inicio_str = $fecha_inicio->format('Y-m-d H:i:s');
$fin_str    = $fecha_fin->format('Y-m-d H:i:s');

/* -------------------------------------------------
   3. Consulta SQL
--------------------------------------------------*/
$sql = "
SELECT
    EXTRACT(YEAR  FROM created_at) AS anio,
    EXTRACT(MONTH FROM created_at) AS mes,
    EXTRACT(DAY   FROM created_at) AS dia,
    EXTRACT(HOUR  FROM created_at) AS hora,
    AVG(field1::NUMERIC) AS temperatura,
    AVG(field2::NUMERIC) AS humedad,
    AVG(field3::NUMERIC) AS co2,
    AVG(field4::NUMERIC) AS ppf
FROM data_entries
WHERE
    created_at BETWEEN '$inicio_str' AND '$fin_str'
    AND channel_id = 2450378
GROUP BY
    EXTRACT(YEAR  FROM created_at),
    EXTRACT(MONTH FROM created_at),
    EXTRACT(DAY   FROM created_at),
    EXTRACT(HOUR  FROM created_at)
ORDER BY
    anio, mes, dia, hora;
";

/* -------------------------------------------------
   4. Ejecutar y devolver
--------------------------------------------------*/
$resultado = $db->query($sql);
echo json_encode($resultado, JSON_PRETTY_PRINT);

$db->close();


