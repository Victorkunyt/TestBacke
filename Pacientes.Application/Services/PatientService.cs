/*
 * ============================================================================
 * PATIENTSERVICE - Camada de Aplicação (Application Layer)
 * ============================================================================
 * 
 * Service contém a lógica de negócio da aplicação.
 * 
 * Responsabilidades:
 * - Orquestrar operações de negócio
 * - Converter entre entidades de domínio e DTOs
 * - Validar regras de negócio (além das validações de entrada)
 * - Coordenar chamadas ao repositório
 * 
 * Clean Architecture:
 * - Esta camada não conhece detalhes de HTTP, banco de dados, etc.
 * - Depende apenas de Domain (entidades) e Interfaces (contratos)
 * - Pode ser testada facilmente sem depender de infraestrutura
 * 
 * SOLID:
 * - Single Responsibility: Lógica de negócio de pacientes
 * - Dependency Inversion: Depende de IPatientRepository (interface)
 */

using Pacientes.Application.DTOs;
using Pacientes.Application.Interfaces;
using Pacientes.Domain.Entities;

namespace Pacientes.Application.Services;

/// <summary>
/// Serviço de aplicação para operações com pacientes.
/// Contém a lógica de negócio e coordena o acesso aos dados.
/// </summary>
public class PatientService : IPatientService
{
    // Injeção de Dependência (SOLID - Dependency Inversion)
    // Depende da abstração IPatientRepository, não da implementação concreta
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retorna todos os pacientes.
    /// Converte entidades de domínio para DTOs de resposta.
    /// </summary>
    public async Task<IReadOnlyList<PatientResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Busca entidades do repositório (camada Infrastructure)
        var patients = await _repository.GetAllAsync(cancellationToken);
        
        // Converte entidades de domínio para DTOs (Data Transfer Objects)
        // DTOs são objetos simples que contêm apenas os dados necessários para a API
        return patients.Select(PatientResponseDto.FromEntity).ToList();
    }

    /// <summary>
    /// Busca um paciente por ID.
    /// Retorna null se não encontrado.
    /// </summary>
    public async Task<PatientResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken);
        return patient is null ? null : PatientResponseDto.FromEntity(patient);
    }

    /// <summary>
    /// Cria um novo paciente.
    /// Converte DTO de entrada para entidade de domínio.
    /// </summary>
    public async Task<PatientResponseDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken = default)
    {
        // Cria nova entidade de domínio a partir do DTO
        // A entidade Patient está na camada Domain (não conhece DTOs)
        var patient = new Patient
        {
            Name = dto.Name,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Document = dto.Document
            // Id e CreatedAt são definidos automaticamente na entidade
        };

        // Persiste no banco através do repositório
        await _repository.AddAsync(patient, cancellationToken);

        // Retorna DTO de resposta (não expõe a entidade diretamente)
        return PatientResponseDto.FromEntity(patient);
    }

    /// <summary>
    /// Atualiza um paciente existente.
    /// Retorna null se o paciente não for encontrado.
    /// </summary>
    public async Task<PatientResponseDto?> UpdateAsync(Guid id, PatientUpdateDto dto, CancellationToken cancellationToken = default)
    {
        // Busca o paciente existente
        var patient = await _repository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return null;  // Paciente não encontrado
        }

        // Atualiza propriedades (lógica de negócio)
        patient.Name = dto.Name;
        patient.Email = dto.Email;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Document = dto.Document;
        patient.UpdatedAt = DateTime.UtcNow;  // Timestamp de atualização

        // Persiste mudanças
        await _repository.UpdateAsync(patient, cancellationToken);

        return PatientResponseDto.FromEntity(patient);
    }

    /// <summary>
    /// Remove um paciente.
    /// Retorna false se o paciente não for encontrado.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return false;  // Paciente não encontrado
        }

        // Remove do banco
        await _repository.DeleteAsync(patient, cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAllAsync(cancellationToken);
        return true;
    }
}


