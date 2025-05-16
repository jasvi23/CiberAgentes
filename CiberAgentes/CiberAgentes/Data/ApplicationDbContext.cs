using Microsoft.EntityFrameworkCore;
using CiberAgentes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CiberAgentes.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para nuestras entidades principales
        public DbSet<Password> Passwords { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<UserMission> UserMissions { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<UserAchievement> UserAchievements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Esta llamada es esencial para mantener la configuración de las tablas de Identity
            // Si no llamamos a base.OnModelCreating, perderemos toda la configuración predeterminada de Identity
            base.OnModelCreating(modelBuilder);

            // Configuración de las relaciones y restricciones

            // Passwords - Relación con User y configuración de índices
            // Establece que cada contraseña pertenece a un usuario (relación uno a muchos)
            // Y configura el borrado en cascada (si se borra el usuario, se borran sus contraseñas)
            modelBuilder.Entity<Password>()
                .HasOne(p => p.User)
                .WithMany(u => u.Passwords)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // UserMissions - Configuración de clave primaria y relaciones
            // Define la clave primaria explícitamente
            modelBuilder.Entity<UserMission>()
                .HasKey(um => um.UserMissionId);

            // Establece la relación: cada UserMission pertenece a un usuario específico
            modelBuilder.Entity<UserMission>()
                .HasOne(um => um.User)
                .WithMany(u => u.UserMissions)
                .HasForeignKey(um => um.UserId);

            // Establece la relación: cada UserMission está asociada a una misión específica
            modelBuilder.Entity<UserMission>()
                .HasOne(um => um.Mission)
                .WithMany(m => m.UserMissions)
                .HasForeignKey(um => um.MissionId);

            // UserAchievements - Configuración similar a UserMissions
            modelBuilder.Entity<UserAchievement>()
                .HasKey(ua => ua.UserAchievementId);

            modelBuilder.Entity<UserAchievement>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAchievements)
                .HasForeignKey(ua => ua.UserId);

            modelBuilder.Entity<UserAchievement>()
                .HasOne(ua => ua.Achievement)
                .WithMany(a => a.UserAchievements)
                .HasForeignKey(ua => ua.AchievementId);

            // Configuración de valores por defecto
            // Asegura que el Score del usuario comience en 0
            modelBuilder.Entity<ApplicationUser>()
                .Property(u => u.Score)
                .HasDefaultValue(0);
        }
    }
}
