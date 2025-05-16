using System;

namespace CiberAgentes.ViewModels.Mission
{
    /// <summary>
    /// ViewModel para mostrar misiones en formato de lista
    /// </summary>
    public class MissionViewModel
    {
        /// <summary>
        /// ID único de la misión
        /// </summary>
        public int MissionId { get; set; }

        /// <summary>
        /// Título de la misión
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Descripción breve de la misión
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Puntos que se obtienen al completar la misión
        /// </summary>
        public int RewardPoints { get; set; }

        /// <summary>
        /// Nivel de dificultad (Fácil, Medio, Difícil)
        /// </summary>
        public string Difficulty { get; set; }

        /// <summary>
        /// Tipo de misión (Quiz, Desafío, Tutorial)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Estado de la misión para el usuario (Disponible, En progreso, Completada, Fallida)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Fecha en que se completó la misión (si está completada)
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Puntuación obtenida en la misión (si está completada)
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Indica si la misión está disponible para iniciar
        /// </summary>
        public bool IsAvailable => Status == "Disponible";

        /// <summary>
        /// Indica si la misión está en progreso
        /// </summary>
        public bool IsInProgress => Status == "En progreso";

        /// <summary>
        /// Indica si la misión está completada
        /// </summary>
        public bool IsCompleted => Status == "Completada";

        /// <summary>
        /// Indica si la misión falló
        /// </summary>
        public bool IsFailed => Status == "Fallida";

        /// <summary>
        /// Devuelve una clase CSS según el estado de la misión
        /// </summary>
        public string GetStatusClass()
        {
            return Status switch
            {
                "Completada" => "mission-completed",
                "En progreso" => "mission-in-progress",
                "Fallida" => "mission-failed",
                _ => "mission-available"
            };
        }

        /// <summary>
        /// Devuelve una clase CSS según la dificultad de la misión
        /// </summary>
        public string GetDifficultyClass()
        {
            return Difficulty switch
            {
                "Fácil" => "bg-success",
                "Medio" => "bg-warning",
                "Difícil" => "bg-danger",
                _ => "bg-info"
            };
        }

        /// <summary>
        /// Devuelve un icono según el tipo de misión
        /// </summary>
        public string GetTypeIcon()
        {
            return Type switch
            {
                "Quiz" => "fa-question-circle",
                "Desafío" => "fa-trophy",
                "Tutorial" => "fa-book",
                _ => "fa-flag"
            };
        }
    }
}