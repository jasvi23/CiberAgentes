
using System.ComponentModel.DataAnnotations;

namespace CiberAgentes.ViewModels.Password
{
    /// <summary>
    /// ViewModel para el generador de contraseñas
    /// </summary>
    public class PasswordGeneratorViewModel
    {
        /// <summary>
        /// Contraseña generada o evaluada
        /// </summary>
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Puntuación de seguridad (0-100)
        /// </summary>
        public int SecurityScore { get; set; }

        /// <summary>
        /// Nivel de seguridad en texto (Débil, Moderada, Fuerte)
        /// </summary>
        public string SecurityLevel { get; set; }

        /// <summary>
        /// Mensaje descriptivo sobre la seguridad
        /// </summary>
        public string SecurityMessage { get; set; }

        /// <summary>
        /// Indica si la contraseña fue generada (para seleccionar la pestaña activa)
        /// </summary>
        public bool IsGenerated { get; set; }

        /// <summary>
        /// Opciones para la generación de contraseñas
        /// </summary>
        public GenerateOptionsModel GenerateOptions { get; set; } = new GenerateOptionsModel();

        /// <summary>
        /// Devuelve una clase CSS para colorear el nivel de seguridad
        /// </summary>
        public string GetSecurityClass()
        {
            return SecurityLevel switch
            {
                "Fuerte" => "success",
                "Moderada" => "warning",
                _ => "danger"
            };
        }
    }

    /// <summary>
    /// Modelo para las opciones de generación de contraseñas
    /// </summary>
    public class GenerateOptionsModel
    {
        /// <summary>
        /// Longitud de la contraseña
        /// </summary>
        [Range(8, 32, ErrorMessage = "La longitud debe estar entre 8 y 32 caracteres")]
        [Display(Name = "Longitud")]
        public int Length { get; set; } = 12;

        /// <summary>
        /// Incluir letras minúsculas (a-z)
        /// </summary>
        [Display(Name = "Incluir minúsculas (a-z)")]
        public bool IncludeLowercase { get; set; } = true;

        /// <summary>
        /// Incluir letras mayúsculas (A-Z)
        /// </summary>
        [Display(Name = "Incluir mayúsculas (A-Z)")]
        public bool IncludeUppercase { get; set; } = true;

        /// <summary>
        /// Incluir números (0-9)
        /// </summary>
        [Display(Name = "Incluir números (0-9)")]
        public bool IncludeNumbers { get; set; } = true;

        /// <summary>
        /// Incluir caracteres especiales (!@#$%^&*)
        /// </summary>
        [Display(Name = "Incluir caracteres especiales (!@#$%^&*)")]
        public bool IncludeSpecial { get; set; } = true;
    }
}