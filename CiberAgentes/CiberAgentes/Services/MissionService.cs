// Servicio para gestionar misiones y su progreso

using CiberAgentes.Data;
using CiberAgentes.Models;
using CiberAgentes.Services;
using CiberAgentes.ViewModels.Mission;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CiberAgentes.Services
{
    public class MissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly AchievementService _achievementService;

        public MissionService(ApplicationDbContext context, AchievementService achievementService)
        {
            _context = context;
            _achievementService = achievementService;
        }

        // Obtener todas las misiones disponibles para un usuario
        public async Task<List<MissionViewModel>> GetAvailableMissionsForUser(string userId)
        {
            // Obtener todas las misiones activas
            var activeMissions = await _context.Missions
                .Where(m => m.IsActive)
                .ToListAsync();

            // Obtener información de las misiones completadas por el usuario
            var userMissions = await _context.UserMissions
                .Where(um => um.UserId == userId)
                .ToListAsync();

            // Crear un diccionario para acceso rápido
            var userMissionDict = userMissions.ToDictionary(
                um => um.MissionId,
                um => um);

            // Crear lista de ViewModel con estado para el usuario
            return activeMissions.Select(m => new MissionViewModel
            {
                MissionId = m.MissionId,
                Title = m.Title,
                Description = m.Description,
                RewardPoints = m.RewardPoints,
                Difficulty = m.Difficulty,
                Type = m.Type,

                // Estado para el usuario
                Status = userMissionDict.TryGetValue(m.MissionId, out var userMission)
                    ? userMission.Status
                    : "Disponible",

                CompletedAt = userMissionDict.TryGetValue(m.MissionId, out userMission)
                    ? userMission.CompletedAt
                    : null,

                Score = userMissionDict.TryGetValue(m.MissionId, out userMission)
                    ? userMission.Score
                    : 0
            }).ToList();
        }

        // Iniciar una misión para un usuario
        public async Task<MissionDetailViewModel> StartMission(int missionId, string userId)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null) return null;

            // Verificar si el usuario ya ha iniciado/completado esta misión
            var existingUserMission = await _context.UserMissions
                .FirstOrDefaultAsync(um => um.MissionId == missionId && um.UserId == userId);

            if (existingUserMission == null)
            {
                // Crear registro de misión iniciada
                existingUserMission = new UserMission
                {
                    UserId = userId,
                    MissionId = missionId,
                    Status = "En progreso",
                    Score = 0
                };

                _context.UserMissions.Add(existingUserMission);
                await _context.SaveChangesAsync();
            }
            else if (existingUserMission.Status == "Completada")
            {
                // La misión ya fue completada, pero permitimos volver a jugarla
                existingUserMission.Status = "En progreso";
                await _context.SaveChangesAsync();
            }

            // Parsear el contenido JSON de la misión
            var missionContent = JsonConvert.DeserializeObject<JObject>(mission.Content);

            // Preparar el ViewModel específico según el tipo de misión
            switch (mission.Type.ToLower())
            {
                case "quiz":
                    return PrepareQuizMission(mission, missionContent);

                case "challenge":
                    return PrepareChalleneMission(mission, missionContent);

                case "tutorial":
                    return PrepareTutorialMission(mission, missionContent);

                default:
                    // Tipo de misión no reconocido
                    return new MissionDetailViewModel
                    {
                        MissionId = mission.MissionId,
                        Title = mission.Title,
                        Description = mission.Description,
                        Type = mission.Type,
                        Error = "Tipo de misión no soportado"
                    };
            }
        }

        // Preparar misión tipo Quiz
        private MissionDetailViewModel PrepareQuizMission(Mission mission, JObject content)
        {
            var questions = content["questions"]?.ToObject<List<QuizQuestion>>();

            return new MissionDetailViewModel
            {
                MissionId = mission.MissionId,
                Title = mission.Title,
                Description = mission.Description,
                Type = mission.Type,
                RewardPoints = mission.RewardPoints,
                QuizQuestions = questions ?? new List<QuizQuestion>()
            };
        }

        // Preparar misión tipo Desafío
        private MissionDetailViewModel PrepareChalleneMission(Mission mission, JObject content)
        {
            var steps = content["steps"]?.ToObject<List<ChallengeStep>>();
            var criteria = content["criteria"]?.ToObject<List<string>>();

            return new MissionDetailViewModel
            {
                MissionId = mission.MissionId,
                Title = mission.Title,
                Description = mission.Description,
                Type = mission.Type,
                RewardPoints = mission.RewardPoints,
                ChallengeSteps = steps ?? new List<ChallengeStep>(),
                SuccessCriteria = criteria ?? new List<string>()
            };
        }

        // Preparar misión tipo Tutorial
        private MissionDetailViewModel PrepareTutorialMission(Mission mission, JObject content)
        {
            var sections = content["sections"]?.ToObject<List<TutorialSection>>();

            return new MissionDetailViewModel
            {
                MissionId = mission.MissionId,
                Title = mission.Title,
                Description = mission.Description,
                Type = mission.Type,
                RewardPoints = mission.RewardPoints,
                TutorialSections = sections ?? new List<TutorialSection>()
            };
        }

        // Completar una misión de tipo Quiz
        public async Task<MissionResultViewModel> CompleteQuizMission(int missionId, string userId, List<QuizAnswer> answers)
        {
            var mission = await _context.Missions.FindAsync(missionId);
            if (mission == null || mission.Type.ToLower() != "quiz")
                return new MissionResultViewModel { Success = false, Message = "Misión no encontrada o tipo incorrecto" };

            // Obtener el contenido de la misión
            var missionContent = JsonConvert.DeserializeObject<JObject>(mission.Content);
            var questions = missionContent["questions"]?.ToObject<List<QuizQuestion>>();

            if (questions == null || !questions.Any())
                return new MissionResultViewModel { Success = false, Message = "Formato de preguntas inválido" };

            // Verificar respuestas
            int correctAnswers = 0;
            int totalQuestions = questions.Count;

            foreach (var answer in answers)
            {
                var question = questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question != null && question.CorrectOptionIndex == answer.SelectedOptionIndex)
                {
                    correctAnswers++;
                }
            }

            // Calcular puntuación
            int score = (int)Math.Round((double)correctAnswers / totalQuestions * mission.RewardPoints);

            // Actualizar estado de la misión para el usuario
            var userMission = await _context.UserMissions
                .FirstOrDefaultAsync(um => um.MissionId == missionId && um.UserId == userId);

            if (userMission == null)
            {
                userMission = new UserMission
                {
                    UserId = userId,
                    MissionId = missionId
                };
                _context.UserMissions.Add(userMission);
            }

            // Determinar si completó con éxito (75% de respuestas correctas)
            bool isSuccess = (double)correctAnswers / totalQuestions >= 0.75;

            userMission.Status = isSuccess ? "Completada" : "Fallida";
            userMission.Score = score;
            userMission.CompletedAt = DateTime.UtcNow;

            // Aumentar puntuación del usuario si completó con éxito
            if (isSuccess)
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.Score += score;
                }
            }

            await _context.SaveChangesAsync();

            // Verificar si se desbloquearon nuevos logros
            await _achievementService.CheckAndUnlockAchievements(userId);

            // Devolver resultado
            return new MissionResultViewModel
            {
                Success = true,
                Message = isSuccess ? "¡Misión completada con éxito!" : "Misión completada, pero no con suficiente puntuación. ¡Inténtalo de nuevo!",
                CorrectAnswers = correctAnswers,
                TotalQuestions = totalQuestions,
                EarnedPoints = isSuccess ? score : 0,
                CompletionPercentage = (int)Math.Round((double)correctAnswers / totalQuestions * 100)
            };
        }
    }
}