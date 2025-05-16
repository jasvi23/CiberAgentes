using System.Collections.Generic;

namespace CiberAgentes.ViewModels.Mission
{
    /// <summary>
    /// ViewModel para mostrar los detalles completos de una misión
    /// </summary>
    public class MissionDetailViewModel
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
        /// Descripción detallada de la misión
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Tipo de misión (Quiz, Desafío, Tutorial)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Puntos que se obtienen al completar la misión
        /// </summary>
        public int RewardPoints { get; set; }

        /// <summary>
        /// Mensaje de error (si ocurre algún problema)
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Para misiones tipo Quiz: Lista de preguntas
        /// </summary>
        public List<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();

        /// <summary>
        /// Para misiones tipo Desafío: Lista de pasos
        /// </summary>
        public List<ChallengeStep> ChallengeSteps { get; set; } = new List<ChallengeStep>();

        /// <summary>
        /// Para misiones tipo Desafío: Criterios de éxito
        /// </summary>
        public List<string> SuccessCriteria { get; set; } = new List<string>();

        /// <summary>
        /// Para misiones tipo Tutorial: Secciones de contenido
        /// </summary>
        public List<TutorialSection> TutorialSections { get; set; } = new List<TutorialSection>();

        /// <summary>
        /// Indica si el detalle de la misión contiene un quiz
        /// </summary>
        public bool IsQuiz => Type.ToLower() == "quiz" && QuizQuestions.Count > 0;

        /// <summary>
        /// Indica si el detalle de la misión contiene un desafío
        /// </summary>
        public bool IsChallenge => Type.ToLower() == "challenge" && ChallengeSteps.Count > 0;

        /// <summary>
        /// Indica si el detalle de la misión contiene un tutorial
        /// </summary>
        public bool IsTutorial => Type.ToLower() == "tutorial" && TutorialSections.Count > 0;

        /// <summary>
        /// Indica si ocurrió un error al cargar el detalle de la misión
        /// </summary>
        public bool HasError => !string.IsNullOrEmpty(Error);
    }

    /// <summary>
    /// Clase para preguntas de Quiz
    /// </summary>
    public class QuizQuestion
    {
        /// <summary>
        /// ID único de la pregunta
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Texto de la pregunta
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Lista de opciones posibles
        /// </summary>
        public List<string> Options { get; set; } = new List<string>();

        /// <summary>
        /// Índice de la opción correcta
        /// </summary>
        public int CorrectOptionIndex { get; set; }

        /// <summary>
        /// Explicación que se muestra después de responder
        /// </summary>
        public string Explanation { get; set; }
    }

    /// <summary>
    /// Clase para pasos de un Desafío
    /// </summary>
    public class ChallengeStep
    {
        /// <summary>
        /// Número de paso
        /// </summary>
        public int StepNumber { get; set; }

        /// <summary>
        /// Instrucción para este paso
        /// </summary>
        public string Instruction { get; set; }

        /// <summary>
        /// Pista opcional para ayudar al usuario
        /// </summary>
        public string Hint { get; set; }
    }

    /// <summary>
    /// Clase para secciones de un Tutorial
    /// </summary>
    public class TutorialSection
    {
        /// <summary>
        /// Título de la sección
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Contenido principal de la sección
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// URL de imagen ilustrativa (opcional)
        /// </summary>
        public string ImageUrl { get; set; }
    }
    public class QuizAnswer
    {
        public int QuestionId { get; set; }
        public int SelectedOptionIndex { get; set; }
    }
}
