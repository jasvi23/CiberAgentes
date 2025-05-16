using CiberAgentes.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CiberAgentes.Controllers
{
    public class AchievementsController : Controller
    {
        private readonly AchievementService _achievementService;

        public AchievementsController(AchievementService achievementService)
        {
            _achievementService = achievementService;
        }

        // Mostrar la página de logros del usuario
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var achievements = await _achievementService.GetUserAchievementsWithStatus(userId);

            return View(achievements);
        }
    }
}
