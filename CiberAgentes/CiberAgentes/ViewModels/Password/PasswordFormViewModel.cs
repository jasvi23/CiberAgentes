
using System.ComponentModel.DataAnnotations;

namespace CiberAgentes.ViewModels.Password
{
    /// <summary>
    /// ViewModel para los formularios de creación y edición de contraseñas
    /// </summary>
    public class PasswordFormViewModel
    {
        /// <summary>
        /// ID de la contraseña (solo para edición)
        /// </summary>
        public int PasswordId { get; set; }

        /// <summary>
        /// Título o descripción de la contraseña
        /// </summary>
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede exceder los 100 caracteres")]
        [Display(Name = "Título")]
        public string Title { get; set; }

        /// <summary>
        /// Nombre de usuario asociado a esta contraseña
        /// </summary>
        [StringLength(100, ErrorMessage = "El nombre de usuario no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; }

        /// <summary>
        /// Valor de la contraseña (sin cifrar)
        /// </summary>
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        /// <summary>
        /// Contraseña maestra del usuario (para cifrado)
        /// </summary>
        [Required(ErrorMessage = "La contraseña maestra es obligatoria para cifrar")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña maestra")]
        public string MasterPassword { get; set; }

        /// <summary>
        /// Indica si se trata de una nueva contraseña o edición
        /// </summary>
        public bool IsNewPassword => PasswordId == 0;
    }
}