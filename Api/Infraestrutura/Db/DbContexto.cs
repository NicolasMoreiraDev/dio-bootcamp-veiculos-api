using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    public DbContexto(DbContextOptions<DbContexto> options) : base(options)
    {
    }
    
    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador()
            {
                Id = 1,
                Email = "adminstrador@teste.com",
                // O login usará o BCrypt.
                Senha = "123456", 
                Perfil = "Adm"
            }
        );
    }
    }