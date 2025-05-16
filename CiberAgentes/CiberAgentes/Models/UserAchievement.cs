// Relación entre usuarios y logros

using CiberAgentes.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CiberAgentes.Models
{
    public class UserAchievement
    {
        [Key]
        public int UserAchievementId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int AchievementId { get; set; }

        public DateTime UnlockedAt { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("AchievementId")]
        public virtual Achievement Achievement { get; set; }
    }
}