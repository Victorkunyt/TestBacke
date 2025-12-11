using Pacientes.Application.DTOs;

namespace Pacientes.Application.Interfaces;

public interface IPatientService
{
    Task<IReadOnlyList<PatientResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PatientResponseDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PatientResponseDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken = default);
    Task<PatientResponseDto?> UpdateAsync(Guid id, PatientUpdateDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DeleteAllAsync(CancellationToken cancellationToken = default);
}


