
using System.Collections.Generic;
using System.Linq;

namespace CiberAgentes.ViewModels.Achievement
{
    /// <summary>
    /// ViewModel para mostrar la lista completa de logros con estadísticas
    /// </summary>
    public class AchievementListViewModel
    {
        /// <summary>
        /// Lista de logros disponibles
        /// </summary>
        public List<AchievementViewModel> Achievements { get; set; } = new List<AchievementViewModel>();

        /// <summary>
        /// Puntuación total del usuario
        /// </summary>
        public int TotalScore { get; set; }

        /// <summary>
        /// Número total de logros
        /// </summary>
        public int TotalAchievements => Achievements.Count;

        /// <summary>
        /// Número de logros desbloqueados
        /// </summary>
        public int UnlockedAchievements => Achievements.Count(a => a.IsUnlocked);

        /// <summary>
        /// Porcentaje de logros desbloqueados
        /// </summary>
        public int UnlockPercentage => TotalAchievements > 0
            ? (int)(UnlockedAchievements * 100.0 / TotalAchievements)
            : 0;

        /// <summary>
        /// Total de puntos ganados de logros
        /// </summary>
        public int TotalAchievementPoints => Achievements
            .Where(a => a.IsUnlocked)
            .Sum(a => a.RewardPoints);

        /// <summary>
        /// Logros desbloqueados recientemente (en los últimos 7 días)
        /// </summary>
        public List<AchievementViewModel> RecentAchievements => Achievements
            .Where(a => a.IsUnlocked && a.UnlockedAt.HasValue && (System.DateTime.Now - a.UnlockedAt.Value).TotalDays <= 7)
            .OrderByDescending(a => a.UnlockedAt)
            .ToList();

        /// <summary>
        /// Obtiene los próximos logros por desbloquear
        /// </summary>
        public List<AchievementViewModel> NextAchievements => Achievements
            .Where(a => !a.IsUnlocked)
            .OrderBy(a => a.AchievementId)
            .Take(3)
            .ToList();
    }
}
