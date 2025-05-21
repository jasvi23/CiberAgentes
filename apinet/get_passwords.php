<?php
header('Content-Type: application/json');
include 'config.php';

$data = json_decode(file_get_contents("php://input"), true);

if (!empty($data['UserId'])) {
    $stmt = $pdo->prepare("SELECT * FROM Passwords WHERE UserId = ?");
    try {
        $stmt->execute([$data['UserId']]);
        $results = $stmt->fetchAll();
        echo json_encode($results);
    } catch (PDOException $e) {
        echo json_encode(["error" => "Error al obtener contraseñas"]);
    }
} else {
    echo json_encode(["error" => "UserId es obligatorio"]);
}
?>