using Microsoft.EntityFrameworkCore;
using QueueSystemBackend.Models;

namespace QueueSystemBackend.Data
{
    public class QueueContext : DbContext
    {
         public QueueContext(DbContextOptions<QueueContext> options) : base(options) { }

       public DbSet<Service> Services { get; set; }
       public DbSet<Ticket> Tickets { get; set; } // Adicionando DbSet para Ticket

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações adicionais, se necessário
        }
    }
}
  