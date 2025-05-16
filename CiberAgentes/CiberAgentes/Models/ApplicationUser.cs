// Modelo extendido del usuario de Identity con propiedades adicionales
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace CiberAgentes.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Propiedades adicionales para el usuario
        public int Score { get; set; } = 0;

        // Navegación a contraseñas guardadas
        public virtual ICollection<Password> Passwords { get; set; } = new List<Password>();

        // Navegación a misiones completadas
        public virtual ICollection<UserMission> UserMissions { get; set; } = new List<UserMission>();

        // Navegación a logros desbloqueados
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
}
