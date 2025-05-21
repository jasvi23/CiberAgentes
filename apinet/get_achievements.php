<?php
header('Content-Type: application/json');
include 'config.php';

try {
    $stmt = $pdo->query("SELECT * FROM Achievements");
    $achievements = $stmt->fetchAll();
    echo json_encode($achievements);
} catch (PDOException $e) {
    echo json_encode(["error" => "Error al obtener logros"]);
}
?>