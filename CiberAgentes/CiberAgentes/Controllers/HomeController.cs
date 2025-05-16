using System.Diagnostics;
using CiberAgentes.Models;
using CiberAgentes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CiberAgentes.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AchievementService _achievementService;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            AchievementService achievementService)
        {
            _logger = logger;
            _userManager = userManager;
            _achievementService = achievementService;
        }

        // Página de inicio
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Si el usuario está autenticado, verificar si hay nuevos logros
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _achievementService.CheckAndUnlockAchievements(userId);
            }

            return View();
        }

        // Página de privacidad
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // Página de error
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Página de panel de control (dashboard)
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Aquí podrías cargar un ViewModel con estadísticas del usuario
            return View(user);
        }

        // Página de ayuda
        [AllowAnonymous]
        public IActionResult Help()
        {
            return View();
        }
    }
}