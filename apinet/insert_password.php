<?php
header('Content-Type: application/json');
include 'config.php';

$data = json_decode(file_get_contents("php://input"), true);

if (!empty($data['UserId']) && !empty($data['Title']) && !empty($data['EncryptedPassword']) && !empty($data['EncryptionIV'])) {
    $stmt = $pdo->prepare("INSERT INTO Passwords (UserId, Title, Username, EncryptedPassword, EncryptionIV, SecurityLevel)
                           VALUES (?, ?, ?, ?, ?, ?)");

    try {
        $stmt->execute([
            $data['UserId'],
            $data['Title'],
            $data['Username'] ?? null,
            $data['EncryptedPassword'],
            $data['EncryptionIV'],
            $data['SecurityLevel'] ?? 0
        ]);

        echo json_encode(["success" => "Contraseña guardada correctamente"]);
    } catch (PDOException $e) {
        echo json_encode(["error" => "Error al guardar contraseña"]);
    }
} else {
    echo json_encode(["error" => "Campos obligatorios faltantes"]);
}
?>