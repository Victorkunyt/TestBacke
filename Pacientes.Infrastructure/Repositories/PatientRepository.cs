/*
 * ============================================================================
 * PATIENTREPOSITORY - Implementação do Repositório (Infrastructure Layer)
 * ============================================================================
 * 
 * Repository Pattern:
 * - Abstrai o acesso a dados
 * - Centraliza lógica de persistência
 * - Facilita testes (pode criar implementação em memória)
 * - Permite trocar banco de dados sem alterar código de negócio
 * 
 * Entity Framework Core:
 * - Usa DbContext para acessar o banco
 * - LINQ é traduzido para SQL automaticamente
 * - AsNoTracking() melhora performance em consultas somente leitura
 * 
 * Clean Architecture:
 * - Esta é a camada Infrastructure
 * - Implementa IPatientRepository definido na camada Application
 * - Conhece detalhes técnicos (EF Core, MySQL, etc.)
 * 
 * SOLID:
 * - Single Responsibility: Apenas acesso a dados
 * - Dependency Inversion: Implementa interface definida em Application
 */

using Microsoft.EntityFrameworkCore;
using Pacientes.Application.Interfaces;
using Pacientes.Domain.Entities;
using Pacientes.Infrastructure.Data;

namespace Pacientes.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do repositório usando Entity Framework Core e MySQL.
/// </summary>
public class PatientRepository : IPatientRepository
{
    // DbContext injetado via Dependency Injection
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todos os pacientes.
    /// AsNoTracking() melhora performance pois não rastreia mudanças (somente leitura).
    /// </summary>
    public async Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // LINQ é traduzido para: SELECT * FROM Patients
        // AsNoTracking() = não rastreia mudanças (mais rápido para leitura)
        return await _context.Patients.AsNoTracking().ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Busca paciente por ID usando a chave primária (mais eficiente).
    /// </summary>
    public async Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // FindAsync é otimizado para busca por chave primária
        // Traduzido para: SELECT * FROM Patients WHERE Id = @id LIMIT 1
        return await _context.Patients.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Adiciona um novo paciente ao banco.
    /// SaveChangesAsync persiste as mudanças.
    /// </summary>
    public async Task AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        // Adiciona à memória (não persiste ainda)
        await _context.Patients.AddAsync(patient, cancellationToken);
        
        // Persiste no banco: INSERT INTO Patients (...)
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Atualiza um paciente existente.
    /// </summary>
    public async Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        // Marca a entidade como modificada
        _context.Patients.Update(patient);
        
        // Persiste: UPDATE Patients SET ... WHERE Id = @id
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Remove um paciente do banco.
    /// </summary>
    public async Task DeleteAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        // Marca para remoção
        _context.Patients.Remove(patient);
        
        // Persiste: DELETE FROM Patients WHERE Id = @id
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica se um paciente existe por ID (sem carregar o objeto completo).
    /// Mais eficiente que GetByIdAsync quando só precisa verificar existência.
    /// </summary>
    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Traduzido para: SELECT 1 FROM Patients WHERE Id = @id LIMIT 1
        // Retorna apenas true/false, não carrega o objeto completo
        return await _context.Patients.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Patients.AnyAsync(x => x.Email == email, cancellationToken);
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken)
    {
        await _context.Patients.ExecuteDeleteAsync(cancellationToken);
    }

    /// <summary>
    /// Verifica se existe outro paciente (diferente do ID fornecido) com o email especificado.
    /// Usado na atualização para garantir que o email não está sendo usado por outro paciente.
    /// Retorna true se existe outro paciente com esse email, false caso contrário.
    /// 
    /// Exemplo de uso:
    /// - Paciente A (ID: abc-123, Email: "email1@test.com")
    /// - Paciente B (ID: def-456, Email: "email2@test.com")
    /// - Tentando atualizar Paciente A com email "email2@test.com"
    /// - Este método retorna TRUE (existe outro paciente com esse email)
    /// </summary>
    public async Task<bool> EExistingemailotherthanmine(string email, Guid id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
            
        // Normaliza o email: remove espaços em branco antes e depois
        var normalizedEmail = email.Trim();
        
        // Query SQL gerada: SELECT 1 FROM Patients WHERE Email = @email AND Id != @id LIMIT 1
        // Verifica se existe algum paciente com esse email E que tenha ID diferente do fornecido
        var exists = await _context.Patients
            .AnyAsync(x => x.Email == normalizedEmail && x.Id != id, cancellationToken);
            
        return exists;
    }
}
