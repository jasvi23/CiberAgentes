// Relación entre usuarios y misiones

using CiberAgentes.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CiberAgentes.Models
{
    public class UserMission
    {
        [Key]
        public int UserMissionId { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int MissionId { get; set; }

        public DateTime? CompletedAt { get; set; }

        public int Score { get; set; }

        [Required, MaxLength(20)]
        public string Status { get; set; } = "Pendiente"; // "Pendiente", "Completada", "Fallida"

        // Navegación
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("MissionId")]
        public virtual Mission Mission { get; set; }
    }
}
