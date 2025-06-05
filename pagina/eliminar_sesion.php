<?php
require_once 'Database2.php';
$sesionId = isset($_POST['sesion']) ? intval($_POST['sesion']) : 0;
if (!$sesionId) { http_response_code(400); exit; }

$db = new Database2();
$db->execute("DELETE FROM plantas  WHERE sesion=$sesionId");
$db->execute("DELETE FROM sesiones WHERE sesion=$sesionId");
$db->close();
