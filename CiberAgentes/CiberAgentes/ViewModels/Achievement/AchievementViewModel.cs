
using System;

namespace CiberAgentes.ViewModels.Achievement
{
    ///<summary>ViewModel para mostrar información de un logro</summary>
    public class AchievementViewModel
    {
        /// <summary>
        /// ID único del logro
        /// </summary>
        public int AchievementId { get; set; }

        /// <summary>
        /// Título del logro
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Descripción detallada del logro
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Puntos que se obtienen al desbloquear este logro
        /// </summary>
        public int RewardPoints { get; set; }

        /// <summary>
        /// URL de la imagen o icono del logro
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Indica si el logro está desbloqueado por el usuario
        /// </summary>
        public bool IsUnlocked { get; set; }

        /// <summary>
        /// Fecha en que se desbloqueó el logro (si está desbloqueado)
        /// </summary>
        public DateTime? UnlockedAt { get; set; }

        /// <summary>
        /// Devuelve una clase CSS según el estado del logro
        /// </summary>
        public string GetStatusClass()
        {
            return IsUnlocked ? "achievement-unlocked" : "achievement-locked";
        }

        /// <summary>
        /// Devuelve una clase de opacidad para logros bloqueados
        /// </summary>
        public string GetOpacityClass()
        {
            return IsUnlocked ? "" : "opacity-50";
        }

        /// <summary>
        /// Formatea la fecha de desbloqueo en formato legible
        /// </summary>
        public string GetFormattedUnlockDate()
        {
            return UnlockedAt?.ToString("dd/MM/yyyy HH:mm") ?? "No desbloqueado";
        }
    }
}
