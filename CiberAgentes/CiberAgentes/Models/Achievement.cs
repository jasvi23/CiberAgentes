using CiberAgentes.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CiberAgentes.Models
{
    public class Achievement
    {
        [Key]
        public int AchievementId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        public int RewardPoints { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; }

        // Navegación
        public virtual ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
}
