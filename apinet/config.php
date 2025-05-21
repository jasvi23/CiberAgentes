<?php
// conexión a Kamatera
header('Content-Type: application/json');
$dbHost = 'localhost';       // IP pública kamatera
$dbName = 'CiberAgentes';          // Nombre de la base android
$dbUser = 'app';                // Usuario con privilegios sobre YourVault
$dbPass = 'pepito'; // Contraseña Mariadb

try {
    $pdo = new PDO(
        "mysql:host={$dbHost};dbname={$dbName};charset=utf8mb4",
        $dbUser,
        $dbPass,
        [
          PDO::ATTR_ERRMODE => PDO::ERRMODE_EXCEPTION,
          PDO::ATTR_DEFAULT_FETCH_MODE => PDO::FETCH_ASSOC
        ]
    );
} catch (PDOException $e) {
    http_response_code(500);
    echo json_encode(['error' => 'Error de conexión a la base de datos']);
    exit;
}
