using Pacientes.Domain.Entities;

namespace Pacientes.Application.Interfaces;

public interface IPatientRepository
{
    Task<IReadOnlyList<Patient>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Patient patient, CancellationToken cancellationToken = default);
    Task UpdateAsync(Patient patient, CancellationToken cancellationToken = default);
    Task DeleteAsync(Patient patient, CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken);
}


