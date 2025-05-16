// Modelo para las contraseñas guardadas en el gestor

using CiberAgentes.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CiberAgentes.Models
{
    public class Password
    {
        [Key]
        public int PasswordId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        public string EncryptedPassword { get; set; }

        [Required]
        public string EncryptionIV { get; set; }

        public int SecurityLevel { get; set; }

        // Fechas automáticas
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Propiedad de navegación
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
