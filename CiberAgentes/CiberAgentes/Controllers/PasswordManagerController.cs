using CiberAgentes.Data;
using CiberAgentes.Models;
using CiberAgentes.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CiberAgentes.ViewModels.Password;

namespace CiberAgentes.Controllers
{
    public class PasswordManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly AchievementService _achievementService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PasswordManagerController(
            ApplicationDbContext context,
            PasswordService passwordService,
            AchievementService achievementService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _passwordService = passwordService;
            _achievementService = achievementService;
            _userManager = userManager;
        }

        // Página principal del gestor de contraseñas
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Obtener las contraseñas del usuario
            var passwords = await _context.Passwords
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.UpdatedAt)
                .Select(p => new PasswordViewModel
                {
                    PasswordId = p.PasswordId,
                    Title = p.Title,
                    Username = p.Username,
                    SecurityLevel = p.SecurityLevel,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync();

            return View(passwords);
        }

        // Mostrar formulario para agregar nueva contraseña
        public IActionResult Create()
        {
            return View(new PasswordFormViewModel());
        }

        // Procesar formulario para agregar nueva contraseña
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PasswordFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Para cifrar la contraseña necesitamos la contraseña maestra (la contraseña de la cuenta)
                // Sin embargo, por seguridad, no podemos acceder a la contraseña del usuario
                // Utilizamos la contraseña maestra que proporciona el usuario en el formulario
                if (string.IsNullOrEmpty(model.MasterPassword))
                {
                    ModelState.AddModelError("MasterPassword", "Debes proporcionar tu contraseña maestra para cifrar");
                    return View(model);
                }

                // Evaluar la seguridad de la contraseña
                int securityLevel = _passwordService.EvaluatePasswordStrength(model.Password);

                // Cifrar la contraseña con la contraseña maestra
                var (encryptedPassword, iv) = _passwordService.EncryptPassword(model.Password, model.MasterPassword);

                // Crear nuevo registro de contraseña
                var password = new Password
                {
                    UserId = userId,
                    Title = model.Title,
                    Username = model.Username,
                    EncryptedPassword = encryptedPassword,
                    EncryptionIV = iv,
                    SecurityLevel = securityLevel,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Passwords.Add(password);
                await _context.SaveChangesAsync();

                // Verificar si se desbloquearon nuevos logros
                await _achievementService.CheckAndUnlockAchievements(userId);

                TempData["SuccessMessage"] = "Contraseña guardada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al guardar la contraseña: " + ex.Message);
                return View(model);
            }
        }

        // Mostrar detalles de una contraseña (con su valor descifrado)
        public async Task<IActionResult> Details(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var password = await _context.Passwords
                .FirstOrDefaultAsync(p => p.PasswordId == id && p.UserId == userId);

            if (password == null)
            {
                return NotFound();
            }

            var viewModel = new PasswordDetailsViewModel
            {
                PasswordId = password.PasswordId,
                Title = password.Title,
                Username = password.Username,
                SecurityLevel = password.SecurityLevel,
                CreatedAt = password.CreatedAt,
                UpdatedAt = password.UpdatedAt
            };

            return View(viewModel);
        }

        // Descifrar contraseña (AJAX)
        [HttpPost]
        public async Task<IActionResult> Decrypt(int id, string masterPassword)
        {
            if (string.IsNullOrEmpty(masterPassword))
            {
                return Json(new { success = false, message = "Debes proporcionar tu contraseña maestra para descifrar" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var password = await _context.Passwords
                .FirstOrDefaultAsync(p => p.PasswordId == id && p.UserId == userId);

            if (password == null)
            {
                return Json(new { success = false, message = "Contraseña no encontrada" });
            }

            try
            {
                // Descifrar la contraseña
                string decryptedPassword = _passwordService.DecryptPassword(
                    password.EncryptedPassword,
                    password.EncryptionIV,
                    masterPassword);

                if (decryptedPassword == null)
                {
                    return Json(new { success = false, message = "La contraseña maestra no es correcta" });
                }

                return Json(new { success = true, password = decryptedPassword });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error al descifrar la contraseña" });
            }
        }

        // Mostrar formulario para editar contraseña
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var password = await _context.Passwords
                .FirstOrDefaultAsync(p => p.PasswordId == id && p.UserId == userId);

            if (password == null)
            {
                return NotFound();
            }

            var viewModel = new PasswordFormViewModel
            {
                PasswordId = password.PasswordId,
                Title = password.Title,
                Username = password.Username
            };

            return View(viewModel);
        }

        // Procesar formulario para editar contraseña
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PasswordFormViewModel model)
        {
            if (id != model.PasswordId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var password = await _context.Passwords
                .FirstOrDefaultAsync(p => p.PasswordId == id && p.UserId == userId);

            if (password == null)
            {
                return NotFound();
            }

            try
            {
                // Actualizar datos básicos
                password.Title = model.Title;
                password.Username = model.Username;

                // Si se proporciona nueva contraseña, cifrarla y actualizarla
                if (!string.IsNullOrEmpty(model.Password) && !string.IsNullOrEmpty(model.MasterPassword))
                {
                    // Evaluar la seguridad de la nueva contraseña
                    password.SecurityLevel = _passwordService.EvaluatePasswordStrength(model.Password);

                    // Cifrar la nueva contraseña
                    var (encryptedPassword, iv) = _passwordService.EncryptPassword(model.Password, model.MasterPassword);
                    password.EncryptedPassword = encryptedPassword;
                    password.EncryptionIV = iv;
                }

                password.UpdatedAt = DateTime.UtcNow;

                _context.Update(password);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Contraseña actualizada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Ocurrió un error al actualizar la contraseña: " + ex.Message);
                return View(model);
            }
        }

        // Mostrar confirmación para eliminar contraseña
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var password = await _context.Passwords
                .FirstOrDefaultAsync(p => p.PasswordId == id && p.UserId == userId);

            if (password == null)
            {
                return NotFound();
            }

            return View(new PasswordViewModel
            {
                PasswordId = password.PasswordId,
                Title = password.Title,
                Username = password.Username
            });
        }

        // Procesar eliminación de contraseña
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var password = await _context.Passwords
                .FirstOrDefaultAsync(p => p.PasswordId == id && p.UserId == userId);

            if (password == null)
            {
                return NotFound();
            }

            _context.Passwords.Remove(password);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contraseña eliminada exitosamente";
            return RedirectToAction(nameof(Index));
        }
    }
}