<?php
// guardar_audio.php
if (!isset($_FILES['audio'])) {
    http_response_code(400);
    exit('No se recibió ningún archivo');
}
$destino = '/var/www/html/Unity/grabaciones/';
if (!is_dir($destino)) mkdir($destino, 0775, true);

$nombre = basename($_FILES['audio']['name']);
$ruta   = $destino . $nombre;

if (move_uploaded_file($_FILES['audio']['tmp_name'], $ruta)) {
    echo 'OK';
} else {
    http_response_code(500);
    echo 'Error al mover el archivo';
}
?>
