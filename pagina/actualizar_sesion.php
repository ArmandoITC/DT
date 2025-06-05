<?php
header('Content-Type: application/json');
require_once 'Database2.php';

try {
    // Leer JSON recibido
    $input = json_decode(file_get_contents('php://input'), true);

    // Validar entrada mínima
    if (
        !isset($input['sesion']) ||
        !is_numeric($input['temperatura']) ||
        !is_numeric($input['humedad']) ||
        !is_numeric($input['co2']) ||
        !is_numeric($input['iluminacion'])
    ) {
        http_response_code(400);
        echo json_encode(["error" => "Datos incompletos o inválidos."]);
        exit;
    }

    // Preparar valores
    $sesion       = intval($input['sesion']);
    $temperatura  = floatval($input['temperatura']);
    $humedad      = floatval($input['humedad']);
    $co2          = floatval($input['co2']);
    $iluminacion  = floatval($input['iluminacion']);

    $db = new Database2();

    // Ejecutar actualización
    $sql = "
        UPDATE sesiones
        SET 
            temperatura = $temperatura,
            humedad = $humedad,
            co2 = $co2,
            iluminacion = $iluminacion
        WHERE sesion = $sesion
    ";
    $db->execute($sql);
    $db->close();

    echo json_encode(["success" => true, "mensaje" => "Sesión $sesion actualizada."]);
} catch (Throwable $e) {
    http_response_code(500);
    echo json_encode(["error" => "Error de servidor: " . $e->getMessage()]);
}
