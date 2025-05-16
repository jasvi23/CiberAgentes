using System;

namespace CiberAgentes.ViewModels.Password
{
    /// <summary>
    /// ViewModel para mostrar detalles completos de una contraseña
    /// </summary>
    public class PasswordDetailsViewModel
    {
        /// <summary>
        /// ID único de la contraseña
        /// </summary>
        public int PasswordId { get; set; }

        /// <summary>
        /// Título o descripción de la contraseña
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Nombre de usuario asociado a esta contraseña
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Nivel de seguridad calculado (0-100)
        /// </summary>
        public int SecurityLevel { get; set; }

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Valor de la contraseña descifrada (solo se establece después de descifrar)
        /// </summary>
        public string DecryptedPassword { get; set; }

        /// <summary>
        /// Indica si la contraseña ha sido descifrada
        /// </summary>
        public bool IsDecrypted => !string.IsNullOrEmpty(DecryptedPassword);

        /// <summary>
        /// Devuelve una cadena que representa el nivel de seguridad de la contraseña
        /// </summary>
        public string GetSecurityLevelText()
        {
            if (SecurityLevel < 40)
                return "Débil";
            else if (SecurityLevel < 70)
                return "Moderada";
            else
                return "Fuerte";
        }

        /// <summary>
        /// Devuelve una clase CSS para colorear el nivel de seguridad
        /// </summary>
        public string GetSecurityLevelClass()
        {
            if (SecurityLevel < 40)
                return "text-danger";
            else if (SecurityLevel < 70)
                return "text-warning";
            else
                return "text-success";
        }
    }
}