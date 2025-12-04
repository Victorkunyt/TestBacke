using Pacientes.Domain.Entities;

namespace Pacientes.Application.DTOs;

public record PatientCreateDto(
    string Name,
    string Email,
    DateTime DateOfBirth,
    string Document
);

public record PatientUpdateDto(
    string Name,
    string Email,
    DateTime DateOfBirth,
    string Document
);

public record PatientResponseDto(
    Guid Id,
    string Name,
    string Email,
    DateTime DateOfBirth,
    string Document,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    public static PatientResponseDto FromEntity(Patient patient) =>
        new(
            patient.Id,
            patient.Name,
            patient.Email,
            patient.DateOfBirth,
            patient.Document,
            patient.CreatedAt,
            patient.UpdatedAt
        );
}


