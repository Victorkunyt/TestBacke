/*
 * ============================================================================
 * APPLICATIONDBCONTEXT - Entity Framework Core DbContext
 * ============================================================================
 * 
 * DbContext representa uma sessão com o banco de dados e permite:
 * - Consultar dados usando LINQ
 * - Inserir, atualizar e deletar entidades
 * - Rastrear mudanças nas entidades
 * - Gerenciar transações
 * 
 * Entity Framework Core:
 * - Traduz operações C# para SQL automaticamente
 * - Gerencia conexões com o banco de dados
 * - Suporta Migrations para versionamento do schema
 * 
 * Clean Architecture: Esta é a camada Infrastructure.
 * Implementa detalhes técnicos de acesso a dados.
 */

using Microsoft.EntityFrameworkCore;
using Pacientes.Domain.Entities;

namespace Pacientes.Infrastructure.Data;

/// <summary>
/// DbContext principal da aplicação.
/// Representa o banco de dados e suas tabelas.
/// </summary>
public class ApplicationDbContext : DbContext
{
    // Constructor recebe opções de configuração (connection string, provider, etc.)
    // Configurado em Program.cs via AddDbContext
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSet representa uma tabela no banco de dados
    // Patients é a tabela que armazena os pacientes
    // Usado pelos repositories para acessar dados
    public DbSet<Patient> Patients => Set<Patient>();

    /// <summary>
    /// Configura o modelo de dados (schema do banco).
    /// Chamado quando o EF Core está criando o modelo.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Patient
        modelBuilder.Entity<Patient>(entity =>
        {
            // Define a chave primária
            entity.HasKey(x => x.Id);
            
            // Configura propriedades com validações e tamanhos
            entity.Property(x => x.Name)
                .IsRequired()              // NOT NULL no banco
                .HasMaxLength(200);         // VARCHAR(200)
            
            entity.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(200);
            
            entity.Property(x => x.Document)
                .IsRequired()
                .HasMaxLength(50);
            
            // Outras propriedades (DateOfBirth, CreatedAt, UpdatedAt)
            // são configuradas automaticamente com tipos apropriados
        });
    }
}


