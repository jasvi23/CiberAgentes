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

        // P�gina de inicio
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Si el usuario est� autenticado, verificar si hay nuevos logros
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _achievementService.CheckAndUnlockAchievements(userId);
            }

            return View();
        }

        // P�gina de privacidad
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // P�gina de error
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // P�gina de panel de control (dashboard)
        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Aqu� podr�as cargar un ViewModel con estad�sticas del usuario
            return View(user);
        }

        // P�gina de ayuda
        [AllowAnonymous]
        public IActionResult Help()
        {
            return View();
        }
    }
}