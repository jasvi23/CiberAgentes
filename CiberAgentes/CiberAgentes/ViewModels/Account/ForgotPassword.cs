using System.ComponentModel.DataAnnotations;

namespace CiberAgentes.ViewModels.Account
{
    /// <summary>
    /// ViewModel para el formulario de olvido de contraseña
    /// </summary>
    public class ForgotPasswordViewModel
    {
        /// <summary>
        /// Email del usuario
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato de email no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    /// <summary>
    /// ViewModel para el formulario de reinicio de contraseña
    /// </summary>
    public class ResetPasswordViewModel
    {
        /// <summary>
        /// Email del usuario
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato de email no es válido")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        /// <summary>
        /// Nueva contraseña
        /// </summary>
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La contraseña debe tener al menos {2} caracteres", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Confirmación de la nueva contraseña
        /// </summary>
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// Código de restablecimiento
        /// </summary>
        public string Code { get; set; }
    }
}
