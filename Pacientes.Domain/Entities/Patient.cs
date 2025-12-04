namespace Pacientes.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = default!;

    public string Email { get; set; } = default!;

    public DateTime DateOfBirth { get; set; }

    public string Document { get; set; } = default!; // CPF, RG, etc.

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }
}


