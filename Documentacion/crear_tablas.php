<?php
header('Content-Type: application/json');
include 'config.php';

$sql = <<<SQL

CREATE DATABASE IF NOT EXISTS CiberAgentes;
USE CiberAgentes;

CREATE TABLE IF NOT EXISTS Passwords (
    PasswordId INT AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL,
    Title VARCHAR(100) NOT NULL,
    Username VARCHAR(100),
    EncryptedPassword TEXT NOT NULL,
    EncryptionIV TEXT NOT NULL,
    SecurityLevel INT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Missions (
    MissionId INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Description TEXT NOT NULL,
    RewardPoints INT,
    Difficulty VARCHAR(20),
    Type VARCHAR(50),
    Content JSON,
    IsActive BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS UserMissions (
    UserMissionId INT AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL,
    MissionId INT NOT NULL,
    CompletedAt DATETIME,
    Score INT,
    Status VARCHAR(20) DEFAULT 'Pendiente'
);

CREATE TABLE IF NOT EXISTS Achievements (
    AchievementId INT AUTO_INCREMENT PRIMARY KEY,
    Title VARCHAR(100) NOT NULL,
    Description TEXT NOT NULL,
    RewardPoints INT,
    ImageUrl VARCHAR(255)
);

CREATE TABLE IF NOT EXISTS UserAchievements (
    UserAchievementId INT AUTO_INCREMENT PRIMARY KEY,
    UserId VARCHAR(255) NOT NULL,
    AchievementId INT NOT NULL,
    UnlockedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);

SQL;

try {
    $pdo->exec($sql);
    echo json_encode(["success" => "Tablas creadas correctamente (PascalCase)"]);
} catch (PDOException $e) {
    echo json_encode(["error" => $e->getMessage()]);
}
?>