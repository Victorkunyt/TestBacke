/*
 * ============================================================================
 * PATIENT - Entidade de Domínio (Domain Layer)
 * ============================================================================
 * 
 * Domain-Driven Design (DDD):
 * - Entidades representam conceitos do domínio da aplicação
 * - Contêm lógica de negócio e regras de validação
 * - São independentes de frameworks e tecnologias
 * 
 * Clean Architecture:
 * - Esta é a camada mais interna (Domain)
 * - NÃO depende de nada (zero dependências externas)
 * - Outras camadas dependem dela, mas ela não conhece outras camadas
 * - Representa o "coração" da aplicação
 * 
 * Características:
 * - Não conhece HTTP, banco de dados, DTOs, etc.
 * - Pode ser reutilizada em diferentes contextos (API, console app, etc.)
 * - Fácil de testar (sem dependências externas)
 */

namespace Pacientes.Domain.Entities;

/// <summary>
/// Entidade de domínio que representa um paciente.
/// Esta é a representação central do conceito de "Paciente" no sistema.
/// </summary>
public class Patient
{
    // Chave primária: Identificador único (GUID)
    // Guid.NewGuid() gera um ID único automaticamente
    public Guid Id { get; set; } = Guid.NewGuid();

    // Propriedades do paciente
    // default! indica que será inicializada antes do uso (null-forgiving operator)
    public string Name { get; set; } = default!;

    public string Email { get; set; } = default!;

    public DateTime DateOfBirth { get; set; }

    // Documento de identificação (CPF, RG, etc.)
    public string Document { get; set; } = default!;

    // Timestamps para auditoria
    // CreatedAt: Quando o registro foi criado (sempre preenchido)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // UpdatedAt: Quando foi atualizado pela última vez (nullable)
    // null = nunca foi atualizado, tem valor = última atualização
    public DateTime? UpdatedAt { get; set; }
}


