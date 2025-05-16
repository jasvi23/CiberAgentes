// Archivo: ViewModels/Account/LoginRegisterViewModel.cs
// ViewModel para la página de login y registro combinada

using System.ComponentModel.DataAnnotations;

namespace CiberAgentes.ViewModels.Account
{
    /// <summary>
    /// ViewModel que combina formularios de login y registro en una sola vista
    /// </summary>
    public class LoginRegisterViewModel
    {
        /// <summary>
        /// Modelo para el formulario de inicio de sesión
        /// </summary>
        public LoginViewModel Login { get; set; } = new LoginViewModel();

        /// <summary>
        /// Modelo para el formulario de registro
        /// </summary>
        public RegisterViewModel Register { get; set; } = new RegisterViewModel();
    }

    /// <summary>
    /// ViewModel para el formulario de inicio de sesión
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>
        /// Email del usuario
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato de email no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Opción para recordar al usuario
        /// </summary>
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// ViewModel para el formulario de registro
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>
        /// Nombre de usuario o agente
        /// </summary>
        [Required(ErrorMessage = "El nombre de agente es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        [Display(Name = "Nombre de Agente")]
        public string UserName { get; set; }

        /// <summary>
        /// Email del usuario
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato de email no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Contraseña del usuario
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmación de contraseña
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Aceptación de términos y condiciones
        /// </summary>
        [Required(ErrorMessage = "Debes aceptar los términos y condiciones")]
        [Display(Name = "Acepto los términos y condiciones")]
        public bool AcceptTerms { get; set; }
    }
}