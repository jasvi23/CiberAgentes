// Servicio para manejar los logros y su desbloqueo

using CiberAgentes.Data;
using CiberAgentes.Models;
using CiberAgentes.Data;
using CiberAgentes.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CiberAgentes.ViewModels.Achievement;

namespace CiberAgentes.Services
{
    public class AchievementService
    {
        private readonly ApplicationDbContext _context;

        public AchievementService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Verificar y desbloquear logros basados en acciones del usuario
        public async Task CheckAndUnlockAchievements(string userId)
        {
            var user = await _context.Users
                .Include(u => u.UserMissions)
                .Include(u => u.Passwords)
                .Include(u => u.UserAchievements)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return;

            // Obtener todos los logros disponibles
            var allAchievements = await _context.Achievements.ToListAsync();

            // Logros ya desbloqueados por el usuario
            var unlockedAchievementIds = user.UserAchievements.Select(ua => ua.AchievementId).ToHashSet();

            // Lista para almacenar nuevos logros desbloqueados
            var newlyUnlockedAchievements = new List<Achievement>();

            // Verificar cada tipo de logro
            await CheckPasswordAchievements(user, allAchievements, unlockedAchievementIds, newlyUnlockedAchievements);
            await CheckMissionAchievements(user, allAchievements, unlockedAchievementIds, newlyUnlockedAchievements);
            await CheckScoreAchievements(user, allAchievements, unlockedAchievementIds, newlyUnlockedAchievements);

            // Guardar los cambios si se desbloquearon nuevos logros
            if (newlyUnlockedAchievements.Any())
            {
                foreach (var achievement in newlyUnlockedAchievements)
                {
                    // Añadir el logro al usuario
                    user.UserAchievements.Add(new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = achievement.AchievementId,
                        UnlockedAt = DateTime.UtcNow
                    });

                    // Incrementar la puntuación del usuario
                    user.Score += achievement.RewardPoints;
                }

                await _context.SaveChangesAsync();
            }
        }

        // Verificar logros relacionados con contraseñas
        private async Task CheckPasswordAchievements(
            ApplicationUser user,
            List<Achievement> allAchievements,
            HashSet<int> unlockedAchievementIds,
            List<Achievement> newlyUnlockedAchievements)
        {
            // Ejemplo: Logro por crear primera contraseña
            var firstPasswordAchievement = allAchievements.FirstOrDefault(a =>
                a.Title == "Primera Contraseña" && !unlockedAchievementIds.Contains(a.AchievementId));

            if (firstPasswordAchievement != null && user.Passwords.Any())
            {
                newlyUnlockedAchievements.Add(firstPasswordAchievement);
            }

            // Ejemplo: Logro por crear 5 contraseñas seguras
            var fiveSecurePasswordsAchievement = allAchievements.FirstOrDefault(a =>
                a.Title == "Maestro de Contraseñas" && !unlockedAchievementIds.Contains(a.AchievementId));

            if (fiveSecurePasswordsAchievement != null &&
                user.Passwords.Count(p => p.SecurityLevel >= 80) >= 5)
            {
                newlyUnlockedAchievements.Add(fiveSecurePasswordsAchievement);
            }
        }

        // Verificar logros relacionados con misiones
        private async Task CheckMissionAchievements(
            ApplicationUser user,
            List<Achievement> allAchievements,
            HashSet<int> unlockedAchievementIds,
            List<Achievement> newlyUnlockedAchievements)
        {
            // Obtener misiones completadas
            var completedMissions = user.UserMissions
                .Where(um => um.Status == "Completada")
                .ToList();

            // Ejemplo: Logro por completar primera misión
            var firstMissionAchievement = allAchievements.FirstOrDefault(a =>
                a.Title == "Primera Misión" && !unlockedAchievementIds.Contains(a.AchievementId));

            if (firstMissionAchievement != null && completedMissions.Any())
            {
                newlyUnlockedAchievements.Add(firstMissionAchievement);
            }

            // Ejemplo: Logro por completar 5 misiones
            var fiveMissionsAchievement = allAchievements.FirstOrDefault(a =>
                a.Title == "Agente en Entrenamiento" && !unlockedAchievementIds.Contains(a.AchievementId));

            if (fiveMissionsAchievement != null && completedMissions.Count >= 5)
            {
                newlyUnlockedAchievements.Add(fiveMissionsAchievement);
            }

            // Ejemplo: Logro por completar 10 misiones
            var tenMissionsAchievement = allAchievements.FirstOrDefault(a =>
                a.Title == "Agente Experto" && !unlockedAchievementIds.Contains(a.AchievementId));

            if (tenMissionsAchievement != null && completedMissions.Count >= 10)
            {
                newlyUnlockedAchievements.Add(tenMissionsAchievement);
            }
        }

        // Verificar logros relacionados con la puntuación
        private async Task CheckScoreAchievements(
            ApplicationUser user,
            List<Achievement> allAchievements,
            HashSet<int> unlockedAchievementIds,
            List<Achievement> newlyUnlockedAchievements)
        {
            // Verificar logros basados en la puntuación total
            foreach (var achievement in allAchievements.Where(a =>
                a.Title.StartsWith("Puntuación:") && !unlockedAchievementIds.Contains(a.AchievementId)))
            {
                // Extraer el valor de puntuación requerido del título (ejemplo: "Puntuación: 500")
                if (int.TryParse(achievement.Title.Split(':')[1].Trim(), out int requiredScore))
                {
                    if (user.Score >= requiredScore)
                    {
                        newlyUnlockedAchievements.Add(achievement);
                    }
                }
            }
        }

        // Obtener todos los logros con estado (desbloqueado o no) para un usuario
        public async Task<List<AchievementViewModel>> GetUserAchievementsWithStatus(string userId)
        {
            // Obtener todos los logros
            var allAchievements = await _context.Achievements.ToListAsync();

            // Obtener logros desbloqueados por el usuario
            var userAchievements = await _context.UserAchievements
                .Where(ua => ua.UserId == userId)
                .ToListAsync();

            // Crear un conjunto de IDs de logros desbloqueados para búsqueda rápida
            var unlockedAchievementIds = userAchievements.Select(ua => ua.AchievementId).ToHashSet();

            // Crear lista de ViewModel con estado
            return allAchievements.Select(a => new AchievementViewModel
            {
                AchievementId = a.AchievementId,
                Title = a.Title,
                Description = a.Description,
                RewardPoints = a.RewardPoints,
                ImageUrl = a.ImageUrl,
                IsUnlocked = unlockedAchievementIds.Contains(a.AchievementId),
                UnlockedAt = userAchievements
                    .FirstOrDefault(ua => ua.AchievementId == a.AchievementId)?.UnlockedAt
            }).ToList();
        }
    }
}
