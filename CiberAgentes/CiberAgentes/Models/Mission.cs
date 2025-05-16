using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CiberAgentes.Models
{
    public class Mission
    {
        [Key]
        public int MissionId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; }

        public int RewardPoints { get; set; }

        [Required, MaxLength(20)]
        public string Difficulty { get; set; } // "Fácil", "Medio", "Difícil"

        [Required, MaxLength(50)]
        public string Type { get; set; } // "Quiz", "Desafío", "Tutorial"

        // Contenido JSON que define la estructura de la misión
        // Ejemplo para un quiz: preguntas, opciones, respuestas correctas
        // Ejemplo para un desafío: pasos, criterios de éxito
        [Required]
        public string Content { get; set; }

        public bool IsActive { get; set; } = true;

        // Navegación
        public virtual ICollection<UserMission> UserMissions { get; set; } = new List<UserMission>();
    }
}