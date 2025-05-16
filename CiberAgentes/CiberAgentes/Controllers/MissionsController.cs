using CiberAgentes.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CiberAgentes.ViewModels.Mission;

namespace CiberAgentes.Controllers
{
    public class MissionsController : Controller
    {
        private readonly MissionService _missionService;

        public MissionsController(MissionService missionService)
        {
            _missionService = missionService;
        }

        // Listar todas las misiones disponibles
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var missions = await _missionService.GetAvailableMissionsForUser(userId);

            return View(missions);
        }

        // Mostrar detalle de una misión
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var missionDetail = await _missionService.StartMission(id, userId);

            if (missionDetail == null)
            {
                return NotFound();
            }

            // Uso de vistas parciales específicas según el tipo de misión
            switch (missionDetail.Type.ToLower())
            {
                case "quiz":
                    return View("Quiz", missionDetail);

                case "challenge":
                    return View("Challenge", missionDetail);

                case "tutorial":
                    return View("Tutorial", missionDetail);

                default:
                    return View(missionDetail);
            }
        }

        // Completar una misión tipo Quiz
        [HttpPost]
        public async Task<IActionResult> CompleteQuiz(int id, [FromBody] List<QuizAnswer> answers)
        {
            if (answers == null || !answers.Any())
            {
                return BadRequest("No se proporcionaron respuestas");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _missionService.CompleteQuizMission(id, userId, answers);

            return Json(result);
        }
    }
}