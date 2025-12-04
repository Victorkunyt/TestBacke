using Pacientes.Application.DTOs;
using Pacientes.Application.Interfaces;
using Pacientes.Domain.Entities;

namespace Pacientes.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;

    public PatientService(IPatientRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<PatientResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var patients = await _repository.GetAllAsync(cancellationToken);
        return patients.Select(PatientResponseDto.FromEntity).ToList();
    }

    public async Task<PatientResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken);
        return patient is null ? null : PatientResponseDto.FromEntity(patient);
    }

    public async Task<PatientResponseDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken = default)
    {
        var patient = new Patient
        {
            Name = dto.Name,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Document = dto.Document
        };

        await _repository.AddAsync(patient, cancellationToken);

        return PatientResponseDto.FromEntity(patient);
    }

    public async Task<PatientResponseDto?> UpdateAsync(Guid id, PatientUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return null;
        }

        patient.Name = dto.Name;
        patient.Email = dto.Email;
        patient.DateOfBirth = dto.DateOfBirth;
        patient.Document = dto.Document;
        patient.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(patient, cancellationToken);

        return PatientResponseDto.FromEntity(patient);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var patient = await _repository.GetByIdAsync(id, cancellationToken);
        if (patient is null)
        {
            return false;
        }

        await _repository.DeleteAsync(patient, cancellationToken);
        return true;
    }
}


