<?php
header('Content-Type: application/json');
include 'config.php';

try {
    $stmt = $pdo->query("SELECT * FROM Missions WHERE IsActive = TRUE");
    $missions = $stmt->fetchAll();
    echo json_encode($missions);
} catch (PDOException $e) {
    echo json_encode(["error" => "Error al obtener misiones"]);
}
?>