/*
 * ============================================================================
 * PATIENTSCONTROLLER - API REST Controller
 * ============================================================================
 * 
 * Este controller expõe endpoints HTTP REST para operações CRUD de pacientes.
 * 
 * APIs REST:
 * - GET    /api/patients        → Lista todos os pacientes
 * - GET    /api/patients/{id}   → Busca paciente por ID
 * - POST   /api/patients        → Cria novo paciente
 * - PUT    /api/patients/{id}   → Atualiza paciente existente
 * - DELETE /api/patients/{id}   → Remove paciente
 * 
 * Clean Architecture: Esta é a camada de apresentação (Presentation Layer).
 * O controller NÃO contém lógica de negócio, apenas delega para a camada Application.
 * 
 * SOLID:
 * - Single Responsibility: Apenas coordena requisições HTTP
 * - Dependency Inversion: Depende de IPatientService (interface), não de implementação
 */

using Microsoft.AspNetCore.Mvc;
using Pacientes.Application.DTOs;
using Pacientes.Application.Interfaces;

namespace Pacientes.Api.Controllers;

// [ApiController] habilita recursos automáticos:
// - Validação automática de ModelState
// - Binding automático de parâmetros
// - Respostas HTTP apropriadas (400, 404, etc.)
[ApiController]

// [Route] define o padrão de rota base
// [controller] é substituído pelo nome do controller sem "Controller"
// Resultado: /api/patients
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    // Injeção de Dependência (SOLID - Dependency Inversion)
    // Depende da interface IPatientService, não da implementação concreta
    private readonly IPatientService _patientService;

    // Constructor Injection: Framework injeta automaticamente a implementação
    public PatientsController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    // GET /api/patients
    // Retorna lista de todos os pacientes
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        // CancellationToken permite cancelar operações longas se o cliente desconectar
        var result = await _patientService.GetAllAsync(cancellationToken);
        return Ok(result);  // HTTP 200 OK com lista de pacientes
    }

    // GET /api/patients/{id}
    // Retorna um paciente específico por ID
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var patient = await _patientService.GetByIdAsync(id, cancellationToken);
        if (patient is null)
            return NotFound();  // HTTP 404 Not Found

        return Ok(patient);  // HTTP 200 OK com o paciente
    }

    // POST /api/patients
    // Cria um novo paciente
    // Body: JSON com Name, Email, DateOfBirth, Document
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PatientCreateDto dto, CancellationToken cancellationToken)
    {
        // FluentValidation valida automaticamente e popula ModelState
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);  // HTTP 400 Bad Request com erros

        var created = await _patientService.CreateAsync(dto, cancellationToken);
        // HTTP 201 Created com Location header apontando para o recurso criado
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/patients/{id}
    // Atualiza um paciente existente
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PatientUpdateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);  // HTTP 400 Bad Request

        var updated = await _patientService.UpdateAsync(id, dto, cancellationToken);
        if (updated is null)
            return NotFound();  // HTTP 404 Not Found

        return Ok(updated);  // HTTP 200 OK com paciente atualizado
    }

    // DELETE /api/patients/{id}
    // Remove um paciente
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _patientService.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();  // HTTP 404 Not Found

        return NoContent();  // HTTP 204 No Content (sucesso sem corpo de resposta)
    }

    // DELETAR TODOS OS PACIENTES
    [HttpDelete]

    public async Task<IActionResult> DeleteAll(CancellationToken cancellationToken)
    {
        var deleted = await _patientService.DeleteAllAsync(cancellationToken);
        if (!deleted)
            return NotFound();  // HTTP 404 Not Found

        return NoContent();  // HTTP 204 No Content (sucesso sem corpo de resposta)
    }
}


