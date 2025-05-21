<?php
header('Content-Type: application/json');
include 'config.php';

$data = json_decode(file_get_contents("php://input"), true);

if (!empty($data['UserId']) && !empty($data['MissionId']) && isset($data['Score']) && !empty($data['Status'])) {
    $stmt = $pdo->prepare("INSERT INTO UserMissions (UserId, MissionId, CompletedAt, Score, Status)
                           VALUES (?, ?, NOW(), ?, ?)");

    try {
        $stmt->execute([
            $data['UserId'],
            $data['MissionId'],
            $data['Score'],
            $data['Status']
        ]);
        echo json_encode(["success" => "Misión registrada"]);
    } catch (PDOException $e) {
        echo json_encode(["error" => "Error al registrar misión"]);
    }
} else {
    echo json_encode(["error" => "Datos obligatorios faltantes"]);
}
?>