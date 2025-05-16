using CiberAgentes.Models;
using CiberAgentes.Services;
using CiberAgentes.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Threading.Tasks;

namespace CiberAgentes.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AchievementService _achievementService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            AchievementService achievementService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _achievementService = achievementService;
        }

        // Página principal de login/registro
        [AllowAnonymous]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginRegisterViewModel());
        }

        // Procesar login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                // Intentar iniciar sesión con email y contraseña
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    // Redirigir a la URL de retorno o a la página principal
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    // Implementar si se añade 2FA en el futuro
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    // Cuenta bloqueada por intentos fallidos
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    // Credenciales inválidas
                    ModelState.AddModelError(string.Empty, "Intento de inicio de sesión no válido.");
                    var viewModel = new LoginRegisterViewModel { Login = model };
                    return View("Index", viewModel);
                }
            }

            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            var loginViewModel = new LoginRegisterViewModel { Login = model };
            return View("Index", loginViewModel);
        }

        // Procesar registro
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["ActiveTab"] = "Register"; // Para activar la pestaña de registro si hay errores

            if (ModelState.IsValid)
            {
                // Crear un nuevo usuario con los datos del formulario
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Score = 0 // Puntuación inicial
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Iniciar sesión automáticamente después del registro
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Redirigir a la URL de retorno o a la página principal
                    return RedirectToLocal(returnUrl);
                }

                // Si hay errores, añadirlos al ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            var viewModel = new LoginRegisterViewModel { Register = model };
            return View("Index", viewModel);
        }

        // Cerrar sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        // Página de olvido de contraseña
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Procesar solicitud de recuperación de contraseña
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Buscar el usuario por email
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Si no encontramos al usuario o no está confirmado, no revelar esa info
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // No revelar que el usuario no existe o no está confirmado
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                // Generar token de reinicio y enviar email (implementación real)
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { userId = user.Id, code = code },
                    protocol: HttpContext.Request.Scheme);

                // TODO: Enviar email con el enlace de recuperación

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // Si llegamos aquí, algo falló, volver a mostrar el formulario
            return View(model);
        }

        // Página de confirmación de solicitud de recuperación
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // Página de reinicio de contraseña
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                // Si no hay código, mostrar error
                return BadRequest("Se requiere un código para restablecer la contraseña.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        // Procesar reinicio de contraseña
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Buscar usuario por email
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // No revelar que el usuario no existe
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            // Reiniciar la contraseña
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            // Si hay errores, añadirlos al ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // Página de confirmación de reinicio de contraseña
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // Página de cuenta bloqueada
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        // Método para redirigir a URL local o a la página principal
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        // Método para mostrar acceso denegado
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Acceso como invitado para demostración (no requiere registro)
        [AllowAnonymous]
        public async Task<IActionResult> GuestDemo()
        {
            // Crear un usuario temporal o usar uno existente para demo
            var guestEmail = "invitado@ciberagentes.com";
            var guestPassword = "Invitado123!";

            var existingUser = await _userManager.FindByEmailAsync(guestEmail);

            if (existingUser == null)
            {
                // Crear usuario de invitado si no existe
                var guestUser = new ApplicationUser
                {
                    UserName = guestEmail,
                    Email = guestEmail,
                    EmailConfirmed = true,
                    Score = 100 // Puntuación inicial para demo
                };

                var result = await _userManager.CreateAsync(guestUser, guestPassword);

                if (result.Succeeded)
                {
                    existingUser = guestUser;
                }
                else
                {
                    // Si hay un error al crear el usuario invitado, redirigir a login
                    return RedirectToAction(nameof(Index));
                }
            }

            // Iniciar sesión como invitado
            await _signInManager.SignInAsync(existingUser, isPersistent: false);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
