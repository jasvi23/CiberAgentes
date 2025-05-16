
namespace CiberAgentes.ViewModels.Mission
{
    /// <summary>
    /// ViewModel para mostrar los resultados después de completar una misión
    /// </summary>
    public class MissionResultViewModel
    {
        /// <summary>
        /// Indica si la misión se completó con éxito
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje descriptivo sobre el resultado
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Para misiones tipo Quiz: Número de respuestas correctas
        /// </summary>
        public int CorrectAnswers { get; set; }

        /// <summary>
        /// Para misiones tipo Quiz: Número total de preguntas
        /// </summary>
        public int TotalQuestions { get; set; }

        /// <summary>
        /// Puntos ganados por completar la misión
        /// </summary>
        public int EarnedPoints { get; set; }

        /// <summary>
        /// Porcentaje de completado (0-100)
        /// </summary>
        public int CompletionPercentage { get; set; }

        /// <summary>
        /// Indica si se desbloquearon nuevos logros
        /// </summary>
        public bool NewAchievementsUnlocked { get; set; }

        /// <summary>
        /// IDs de los nuevos logros desbloqueados
        /// </summary>
        public int[] UnlockedAchievementIds { get; set; } = new int[0];

        /// <summary>
        /// Devuelve una clase CSS según el resultado
        /// </summary>
        public string GetResultClass()
        {
            if (Success)
            {
                return CompletionPercentage >= 90 ? "result-perfect" : "result-success";
            }
            else
            {
                return "result-failure";
            }
        }

        /// <summary>
        /// Devuelve un mensaje sobre el rendimiento
        /// </summary>
        public string GetPerformanceMessage()
        {
            return CompletionPercentage switch
            {
                100 => "¡Perfecto! Has completado la misión con éxito total.",
                >= 90 => "¡Excelente trabajo! Has completado la misión con gran éxito.",
                >= 75 => "¡Buen trabajo! Has completado la misión satisfactoriamente.",
                >= 50 => "Has completado la misión, pero hay margen de mejora.",
                _ => "No has completado la misión con suficiente puntuación."
            };
        }

        /// <summary>
        /// Devuelve una sugerencia basada en el rendimiento
        /// </summary>
        public string GetSuggestion()
        {
            return CompletionPercentage switch
            {
                100 => "¡Sigue así! Estás en camino a convertirte en un experto en ciberseguridad.",
                >= 75 => "Revisa los puntos que fallaste para mejorar aún más.",
                >= 50 => "Considera volver a intentar la misión para mejorar tu puntuación.",
                _ => "Te recomendamos repasar el material y volver a intentar la misión."
            };
        }
    }
}
