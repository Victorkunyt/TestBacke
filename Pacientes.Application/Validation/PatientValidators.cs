using FluentValidation;
using Pacientes.Application.DTOs;

namespace Pacientes.Application.Validation;

public class PatientCreateDtoValidator : AbstractValidator<PatientCreateDto>
{
    public PatientCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.")
            .MaximumLength(200);

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("Documento é obrigatório.")
            .MaximumLength(50);

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Data de nascimento deve ser no passado.");
    }
}

public class PatientUpdateDtoValidator : AbstractValidator<PatientUpdateDto>
{
    public PatientUpdateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório.")
            .EmailAddress().WithMessage("Email inválido.")
            .MaximumLength(200);

        RuleFor(x => x.Document)
            .NotEmpty().WithMessage("Documento é obrigatório.")
            .MaximumLength(50);

        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.Today).WithMessage("Data de nascimento deve ser no passado.");
    }
}


