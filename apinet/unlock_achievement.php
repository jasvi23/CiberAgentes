<?php
header('Content-Type: application/json');
include 'config.php';

$data = json_decode(file_get_contents("php://input"), true);

if (!empty($data['UserId']) && !empty($data['AchievementId'])) {
    $stmt = $pdo->prepare("INSERT INTO UserAchievements (UserId, AchievementId, UnlockedAt)
                           VALUES (?, ?, NOW())");

    try {
        $stmt->execute([
            $data['UserId'],
            $data['AchievementId']
        ]);
        echo json_encode(["success" => "Logro registrado"]);
    } catch (PDOException $e) {
        echo json_encode(["error" => "Error al registrar logro"]);
    }
} else {
    echo json_encode(["error" => "UserId y AchievementId obligatorios"]);
}
?>