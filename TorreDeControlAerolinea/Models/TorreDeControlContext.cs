using System;
using Microsoft.EntityFrameworkCore;

namespace TorreDeControlAerolinea.Models
{
    // Modelo de datos para el avión
    public class Avion
    {
        public int AvionId { get; set; }
        public int AeropuertoSalidaId { get; set; }
        public int AeropuertoLlegadaId { get; set; }
        public DateTime HoraSalida { get; set; }
        public DateTime HoraAterrizaje { get; set; }
        public string EstatusVuelo { get; set; }
        public decimal PesoLimite { get; set; }
        public int LimitePasajeros { get; set; }
        public Aeropuerto AeropuertoSalida { get; set; }
        public Aeropuerto AeropuertoLlegada { get; set; }
    }

    // Modelo de datos para el pasajero
    public class Pasajero
    {
        public int PasajeroId { get; set; }
        public string? Nombre { get; set; }
        public decimal PesoEquipaje { get; set; }
        public int AvionId { get; set; }
        public Avion Avion { get; set; }
    }

    // Modelo de datos para el aeropuerto
    public class Aeropuerto
    {
        public int AeropuertoId { get; set; }
        public string? Nombre { get; set; }
        public int LimiteAviones { get; set; }
    }

    // DbContext para la conexión con la base de datos
    public class TorreDeControlContext : DbContext
    {
        public TorreDeControlContext(DbContextOptions<TorreDeControlContext> options) : base(options)
        {
        }

        public DbSet<Avion> Aviones { get; set; }
        public DbSet<Pasajero> Pasajeros { get; set; }
        public DbSet<Aeropuerto> Aeropuertos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurar las relaciones de clave externa
            modelBuilder.Entity<Avion>()
                .HasOne(a => a.AeropuertoSalida) 
                .WithMany()
                .HasForeignKey(a => a.AeropuertoSalidaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Avion>()
                .HasOne(a => a.AeropuertoLlegada) 
                .WithMany()
                .HasForeignKey(a => a.AeropuertoLlegadaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Pasajero>()
                .HasOne(p => p.Avion)
                .WithMany()
                .HasForeignKey(p => p.AvionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
