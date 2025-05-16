using CiberAgentes.Services;
using CiberAgentes.ViewModels.Password;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CiberAgentes.Controllers
{
    public class PasswordGeneratorController : Controller
    {
        private readonly PasswordService _passwordService;

        public PasswordGeneratorController(PasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        // Página principal del generador de contraseñas
        [AllowAnonymous] // Permitimos acceso sin autenticación a esta página
        public IActionResult Index()
        {
            return View(new PasswordGeneratorViewModel());
        }

        // Método para evaluar la seguridad de una contraseña
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Evaluate(PasswordGeneratorViewModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("Password", "Debes ingresar una contraseña para evaluar");
                return View("Index", model);
            }

            // Evaluar la seguridad de la contraseña
            model.SecurityScore = _passwordService.EvaluatePasswordStrength(model.Password);

            // Determinar nivel de seguridad y mensaje
            if (model.SecurityScore < 40)
            {
                model.SecurityLevel = "Débil";
                model.SecurityMessage = "Tu contraseña es vulnerable. Intenta añadir más longitud, símbolos y variedad de caracteres.";
            }
            else if (model.SecurityScore < 70)
            {
                model.SecurityLevel = "Moderada";
                model.SecurityMessage = "Tu contraseña es aceptable, pero podría ser más segura. Intenta hacer algunos ajustes.";
            }
            else
            {
                model.SecurityLevel = "Fuerte";
                model.SecurityMessage = "¡Excelente! Tu contraseña es muy segura.";
            }

            return View("Index", model);
        }

        // Método para generar una contraseña segura
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Generate(PasswordGeneratorViewModel model)
        {
            // Validar la longitud mínima
            if (model.GenerateOptions.Length < 8)
            {
                ModelState.AddModelError("GenerateOptions.Length", "La longitud mínima debe ser 8 caracteres");
                return View("Index", model);
            }

            // Generar la contraseña con las opciones seleccionadas
            model.Password = _passwordService.GenerateSecurePassword(
                model.GenerateOptions.Length,
                model.GenerateOptions.IncludeLowercase,
                model.GenerateOptions.IncludeUppercase,
                model.GenerateOptions.IncludeNumbers,
                model.GenerateOptions.IncludeSpecial
            );

            // Evaluar la seguridad de la contraseña generada
            model.SecurityScore = _passwordService.EvaluatePasswordStrength(model.Password);

            // Determinar nivel de seguridad y mensaje
            if (model.SecurityScore < 40)
            {
                model.SecurityLevel = "Débil";
                model.SecurityMessage = "La contraseña generada es débil. Intenta ajustar las opciones para mayor seguridad.";
            }
            else if (model.SecurityScore < 70)
            {
                model.SecurityLevel = "Moderada";
                model.SecurityMessage = "La contraseña generada es moderadamente segura.";
            }
            else
            {
                model.SecurityLevel = "Fuerte";
                model.SecurityMessage = "¡Excelente! La contraseña generada es muy segura.";
            }

            // Marcar que acabamos de generar una contraseña
            model.IsGenerated = true;

            return View("Index", model);
        }
    }
}