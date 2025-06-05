<?php
header('Content-Type: application/json');
require_once 'Database2.php';

try {
    $db = new Database2();
    $conn = $db->getConn(); // ← obtener conexión para escape seguro

    // Obtener el ID más bajo disponible
    $ids = array_column($db->query("SELECT sesion FROM sesiones ORDER BY sesion"), 'sesion');
    $libre = 1;
    while (in_array($libre, $ids)) $libre++;

    // Insertar nueva sesión vacía
    $db->execute("INSERT INTO sesiones (sesion) VALUES ($libre)");

    // Insertar 84 plantas con datos aleatorios
    for ($i = 1; $i <= 84; $i++) {
        $tam = round(mt_rand(30, 100) / 10, 1); // tamaño entre 3.0 y 10.0
        $obs = pg_escape_literal($conn, "Planta generada automáticamente");

        $sql = "INSERT INTO plantas (planta_id, sesion, tamaño, observaciones)
                VALUES ($i, $libre, $tam, $obs)";
        $db->execute($sql);
    }

    echo json_encode(["idSesion" => $libre]);
    $db->close();
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error" => $e->getMessage()]);
}
