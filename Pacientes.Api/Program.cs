/*
 * ============================================================================
 * PROGRAM.CS - Configuração da API REST
 * ============================================================================
 * 
 * Este arquivo configura:
 * 1. APIs REST e endpoints HTTP
 * 2. Entity Framework Core com MySQL
 * 3. Injeção de Dependência (SOLID - Dependency Inversion)
 * 4. Validação de dados
 * 5. Swagger para documentação automática da API
 * 
 * Clean Architecture: Esta é a camada de apresentação (Presentation Layer)
 * que depende das camadas Application e Infrastructure.
 */

using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Pacientes.Application.Interfaces;
using Pacientes.Application.Services;
using Pacientes.Application.Validation;
using Pacientes.Infrastructure.Data;
using Pacientes.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// CONFIGURAÇÃO DE SERVIÇOS (Dependency Injection Container)
// ============================================================================

// Registra controllers para processar requisições HTTP REST
// Exemplo: GET /api/patients, POST /api/patients, etc.
builder.Services.AddControllers();

// Swagger/OpenAPI - Documentação automática da API
// Acesse em: https://localhost:5001/swagger (em desenvolvimento)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================================================================
// ENTITY FRAMEWORK CORE - Configuração do ORM e Banco de Dados MySQL
// ============================================================================
// 
// Entity Framework Core é um ORM (Object-Relational Mapping) que permite
// trabalhar com bancos de dados usando objetos C# ao invés de SQL direto.
// 
// Vantagens:
// - Type-safe: Erros detectados em tempo de compilação
// - LINQ: Consultas usando C# (ex: _context.Patients.Where(p => p.Name == "João"))
// - Migrations: Versionamento automático do banco de dados
// - Suporte a múltiplos bancos: MySQL, SQL Server, PostgreSQL, etc.
//
// Para trocar de banco:
// - SQL Server: UseSqlServer(connectionString)
// - PostgreSQL: UseNpgsql(connectionString)
// - MySQL: UseMySql(connectionString) ← Atual

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

// Criar o banco de dados automaticamente se não existir
// Isso facilita o desenvolvimento, mas em produção o banco já deve existir
try
{
    var builderConnection = new MySqlConnectionStringBuilder(connectionString);
    var databaseName = builderConnection.Database;
    builderConnection.Database = ""; // Remove o nome do banco para conectar ao servidor MySQL
    
    using var connection = new MySqlConnection(builderConnection.ConnectionString);
    connection.Open();
    using var command = connection.CreateCommand();
    // Cria o banco com charset UTF8MB4 (suporta emojis e caracteres especiais)
    command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{databaseName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
    command.ExecuteNonQuery();
}
catch (Exception ex)
{
    // Log do erro, mas continua a execução (banco pode já existir)
    Console.WriteLine($"Aviso: Não foi possível criar o banco de dados automaticamente: {ex.Message}");
    Console.WriteLine("Certifique-se de que o banco 'PacientesDb' existe antes de executar as migrations.");
}

// Registra o DbContext no container de DI (SOLID - Dependency Inversion)
// Scoped: Uma instância por requisição HTTP (recomendado para DbContext)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Configuração do MySQL usando Pomelo.EntityFrameworkCore.MySql
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)), 
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,                    // Tenta até 5 vezes em caso de falha temporária
            maxRetryDelay: TimeSpan.FromSeconds(30),  // Espera até 30 segundos entre tentativas
            errorNumbersToAdd: null));          // Códigos de erro MySQL para retry
    // Isso melhora a resiliência em ambientes cloud onde conexões podem falhar temporariamente
});

// ============================================================================
// FLUENTVALIDATION - Validação automática de DTOs
// ============================================================================
// Valida automaticamente os dados recebidos nas requisições HTTP
// Retorna 400 Bad Request se os dados forem inválidos
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<PatientCreateDtoValidator>();

// ============================================================================
// INJEÇÃO DE DEPENDÊNCIA (SOLID - Dependency Inversion Principle)
// ============================================================================
// 
// Registra as implementações concretas para as interfaces (abstrações).
// Isso permite:
// - Testabilidade: Fácil mockar dependências em testes
// - Flexibilidade: Trocar implementações sem alterar código que usa
// - Desacoplamento: Classes dependem de interfaces, não de implementações
//
// Scoped: Uma instância por requisição HTTP (ideal para repositórios e serviços)
// Transient: Nova instância sempre que solicitado
// Singleton: Uma única instância para toda a aplicação

// Clean Architecture: Application Layer
builder.Services.AddScoped<IPatientService, PatientService>();

// Clean Architecture: Infrastructure Layer
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

var app = builder.Build();

// ============================================================================
// CONFIGURAÇÃO DO PIPELINE HTTP (Middleware)
// ============================================================================
// 
// O pipeline processa requisições HTTP na ordem definida abaixo.
// Cada middleware pode modificar a requisição/resposta ou passar para o próximo.

// Swagger apenas em desenvolvimento (não expor em produção por segurança)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Gera o JSON do OpenAPI
    app.UseSwaggerUI();    // Interface web para testar a API
}

// Redireciona HTTP para HTTPS (segurança)
app.UseHttpsRedirection();

// Autenticação e autorização (se configurado)
app.UseAuthorization();

// Mapeia os controllers para rotas HTTP
// Exemplo: PatientsController → /api/patients
app.MapControllers();

// Inicia o servidor HTTP e fica escutando requisições
app.Run();


