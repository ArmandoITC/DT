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

$regexFechaHora = '/^\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}(:\d{2})?$/';
if (!$inicio || !preg_match($regexFechaHora, $inicio)) {
    echo json_encode(["error" => "Fecha de inicio inválida. Usa formato YYYY-MM-DD HH:MM o HH:MM:SS."]);
    exit;
}

$totalHoras = $dias * 24 + $horas;
if ($totalHoras < 24) {
    echo json_encode(["error" => "El intervalo debe ser de al menos 1 día completo (24 horas) para calcular GDD."]);
    exit;
}
if ($totalHoras > 2160) {
    echo json_encode(["error" => "El intervalo no debe exceder los 90 días (2160 horas)."]);
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
   3. Consulta SQL (promedios por día)
--------------------------------------------------*/
$sql = "
SELECT
    DATE(created_at) AS fecha,
    AVG(field1::NUMERIC) AS temperatura
FROM data_entries
WHERE
    created_at BETWEEN '$inicio_str' AND '$fin_str'
    AND channel_id = 2450378
GROUP BY DATE(created_at)
ORDER BY fecha;
";

/* -------------------------------------------------
   4. Calcular GDD (Base 5 °C)
--------------------------------------------------*/
$datos = $db->query($sql);
$gdd_acumulado = 0;
$resultado = [];

foreach ($datos as $row) {
    $temp = floatval($row['temperatura']);
    $gdd = max(0, $temp - 5);
    $gdd_acumulado += $gdd;

    $resultado[] = [
        'fecha' => $row['fecha'],
        'gdd' => round($gdd, 2),
        'gdd_acumulado' => round($gdd_acumulado, 2)
    ];
}

echo json_encode($resultado, JSON_PRETTY_PRINT);
$db->close();
