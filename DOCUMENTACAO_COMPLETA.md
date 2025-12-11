# 📚 Documentação Completa do Projeto - APIs, Clean Architecture e Cloud Native

## 📋 Índice

1. [APIs e Microserviços](#1-apis-e-microserviços)
2. [Entity Framework Core](#2-entity-framework-core)
3. [Bancos SQL](#3-bancos-sql)
4. [Clean Code / SOLID / Clean Architecture](#4-clean-code--solid--clean-architecture)
5. [Cloud Native (Docker)](#5-cloud-native-docker-containers)

---

## 1. APIs e Microserviços

### O que são APIs REST?

**API (Application Programming Interface)** é uma interface que permite comunicação entre diferentes sistemas. Uma **API REST** segue os princípios REST (Representational State Transfer):

- **Stateless**: Cada requisição contém todas as informações necessárias
- **Recursos identificados por URLs**: `/api/patients/{id}`
- **Métodos HTTP padronizados**: GET, POST, PUT, DELETE
- **Estrutura de dados**: JSON (JavaScript Object Notation)

### O que são Microserviços?

**Microserviços** são uma arquitetura onde aplicações são divididas em serviços pequenos, independentes e desacoplados:

- ✅ **Independência**: Cada serviço pode ser desenvolvido, testado e deployado separadamente
- ✅ **Escalabilidade**: Escalar apenas o serviço necessário
- ✅ **Tecnologia**: Cada serviço pode usar tecnologias diferentes
- ✅ **Resiliência**: Falha em um serviço não derruba todo o sistema

### Neste Projeto

Este projeto é uma **API REST** que pode ser parte de um sistema de microserviços:

```csharp
// Pacientes.Api/Controllers/PatientsController.cs
// Controller REST que expõe endpoints HTTP
[ApiController]
[Route("api/[controller]")]  // Rota: /api/patients
public class PatientsController : ControllerBase
{
    // Endpoints:
    // GET    /api/patients        - Lista todos
    // GET    /api/patients/{id}   - Busca por ID
    // POST   /api/patients        - Cria novo
    // PUT    /api/patients/{id}   - Atualiza
    // DELETE /api/patients/{id}   - Remove
}
```

**Características desta API:**

- ✅ Usa ASP.NET Core (framework moderno para APIs)
- ✅ Swagger/OpenAPI para documentação automática
- ✅ Validação automática com FluentValidation
- ✅ Suporte a CancellationToken (cancelamento de requisições)
- ✅ Retorna códigos HTTP apropriados (200, 201, 404, etc.)

---

## 2. Entity Framework Core

### O que é Entity Framework Core?

**EF Core** é um **ORM (Object-Relational Mapping)** que permite trabalhar com bancos de dados usando objetos C# ao invés de SQL direto.

### Vantagens:

1. **Produtividade**: Menos código SQL manual
2. **Type-Safe**: Erros detectados em tempo de compilação
3. **Migrations**: Versionamento do banco de dados
4. **LINQ**: Consultas usando C# ao invés de SQL

### Como funciona neste projeto:

```csharp
// 1. DbContext - Representa o banco de dados
public class ApplicationDbContext : DbContext
{
    public DbSet<Patient> Patients { get; set; }  // Tabela Patients
}

// 2. Entity - Representa uma tabela
public class Patient
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    // ...
}

// 3. Repository - Usa EF Core para operações
public async Task<Patient?> GetByIdAsync(Guid id)
{
    return await _context.Patients.FindAsync(id);
}
```

### Migrations

**Migrations** são scripts que criam/alteram a estrutura do banco:

```bash
# Criar migration
dotnet ef migrations add InitialCreate

# Aplicar no banco
dotnet ef database update
```

**O que acontece:**

1. EF Core compara o modelo atual com o banco
2. Gera SQL para criar/alterar tabelas
3. Aplica as mudanças no banco de dados

---

## 3. Bancos SQL

### SQL Server, PostgreSQL e MySQL

Este projeto usa **MySQL**, mas pode ser facilmente adaptado para outros bancos:

### MySQL (Atual)

```csharp
// Program.cs
options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)));
```

**Características:**

- ✅ Open-source e gratuito
- ✅ Muito usado em aplicações web
- ✅ Suporte a transações ACID
- ✅ Bom desempenho para leitura

### SQL Server

```csharp
// Para usar SQL Server, troque para:
options.UseSqlServer(connectionString);
// Connection string: "Server=localhost;Database=PacientesDb;User=sa;Password=senha;"
```

**Características:**

- ✅ Microsoft, integração com .NET
- ✅ Ferramentas avançadas (SSMS)
- ✅ Suporte a JSON nativo
- ⚠️ Licenciamento necessário para produção

### PostgreSQL

```csharp
// Para usar PostgreSQL, troque para:
options.UseNpgsql(connectionString);
// Connection string: "Host=localhost;Database=PacientesDb;Username=postgres;Password=senha;"
```

**Características:**

- ✅ Open-source e muito robusto
- ✅ Suporte avançado a JSON, arrays, etc.
- ✅ Excelente para aplicações complexas
- ✅ Padrão ACID completo

### Por que usar Connection String?

A **connection string** centraliza as configurações de conexão:

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=PacientesDb;User=root;Password=;Port=3306;"
  }
}
```

**Vantagens:**

- ✅ Fácil mudança entre ambientes (dev, prod)
- ✅ Segurança (não hardcoded no código)
- ✅ Suporte a diferentes bancos sem alterar código

---

## 4. Clean Code / SOLID / Clean Architecture

### Clean Code (Código Limpo)

**Princípios:**

- ✅ **Nomes descritivos**: `GetPatientByIdAsync` ao invés de `Get`
- ✅ **Funções pequenas**: Uma responsabilidade por método
- ✅ **Comentários quando necessário**: Explicar o "porquê", não o "o quê"
- ✅ **Sem duplicação**: DRY (Don't Repeat Yourself)

### SOLID - 5 Princípios

#### S - Single Responsibility Principle (Responsabilidade Única)

```csharp
// ✅ BOM: Cada classe tem uma responsabilidade
public class PatientService      // Lógica de negócio
public class PatientRepository   // Acesso a dados
public class PatientsController  // Endpoints HTTP

// ❌ RUIM: Tudo em uma classe
public class PatientController   // HTTP + Lógica + Banco de dados
```

#### O - Open/Closed Principle (Aberto/Fechado)

```csharp
// ✅ BOM: Abrir para extensão, fechar para modificação
public interface IPatientRepository  // Interface permite diferentes implementações
{
    Task<Patient?> GetByIdAsync(Guid id);
}

// Pode criar: PatientRepository, PatientMongoRepository, PatientInMemoryRepository
// Sem modificar o código existente
```

#### L - Liskov Substitution Principle (Substituição de Liskov)

```csharp
// ✅ BOM: Qualquer implementação de IPatientRepository deve funcionar
IPatientRepository repository = new PatientRepository();  // MySQL
// ou
IPatientRepository repository = new PatientInMemoryRepository();  // Memória
// O código que usa não precisa saber qual é
```

#### I - Interface Segregation Principle (Segregação de Interface)

```csharp
// ✅ BOM: Interfaces específicas
public interface IPatientRepository  // Apenas operações de Patient
public interface IAppointmentRepository  // Apenas operações de Appointment

// ❌ RUIM: Interface gigante com tudo
public interface IRepository  // Patient, Appointment, User, Order, etc.
```

#### D - Dependency Inversion Principle (Inversão de Dependência)

```csharp
// ✅ BOM: Depender de abstrações (interfaces), não de implementações
public class PatientService
{
    private readonly IPatientRepository _repository;  // Interface, não classe concreta
  
    public PatientService(IPatientRepository repository)  // Injeção de dependência
    {
        _repository = repository;
    }
}

// Program.cs - Registra a implementação
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
```

### Clean Architecture

**Clean Architecture** organiza o código em camadas com dependências unidirecionais:

```
┌─────────────────────────────────────┐
│   Pacientes.Api (Presentation)     │  ← Controllers, Endpoints HTTP
├─────────────────────────────────────┤
│   Pacientes.Application (Use Cases)│  ← Services, DTOs, Interfaces
├─────────────────────────────────────┤
│   Pacientes.Domain (Entities)      │  ← Entidades de negócio
├─────────────────────────────────────┤
│   Pacientes.Infrastructure (Data)  │  ← EF Core, Repositories, Banco
└─────────────────────────────────────┘
```

**Regras:**

1. ✅ **Domain** não depende de nada (camada mais interna)
2. ✅ **Application** depende apenas de Domain
3. ✅ **Infrastructure** depende de Domain e Application
4. ✅ **Api** depende de todas (camada mais externa)

**Vantagens:**

- ✅ **Testabilidade**: Fácil mockar dependências
- ✅ **Manutenibilidade**: Mudanças isoladas por camada
- ✅ **Flexibilidade**: Trocar banco de dados sem afetar lógica de negócio
- ✅ **Independência**: Domain não sabe sobre HTTP, banco, etc.

**Exemplo neste projeto:**

```csharp
// Domain (não conhece banco, HTTP, etc.)
public class Patient { ... }

// Application (usa Domain, define contratos)
public interface IPatientRepository { ... }
public class PatientService { ... }

// Infrastructure (implementa Application)
public class PatientRepository : IPatientRepository { ... }

// Api (usa Application)
public class PatientsController
{
    private readonly IPatientService _service;  // Depende da abstração
}
```

---

## 5. Cloud Native (Docker, Containers)

### O que é Cloud Native?

**Cloud Native** são aplicações projetadas para rodar em nuvem, usando containers, microserviços e DevOps.

### Docker e Containers

**Container** é um pacote isolado que contém:

- ✅ Aplicação
- ✅ Dependências
- ✅ Runtime
- ✅ Configurações

**Vantagens:**

- ✅ **Portabilidade**: "Funciona na minha máquina" → Funciona em qualquer lugar
- ✅ **Isolamento**: Cada container é independente
- ✅ **Escalabilidade**: Fácil criar múltiplas instâncias
- ✅ **Consistência**: Mesmo ambiente em dev, test, prod

### Dockerfile deste Projeto

```dockerfile
# Dockerfile - Instruções para construir a imagem Docker

# 1. Imagem base para runtime (leve, apenas para executar)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80 443  # Portas HTTP e HTTPS

# 2. Imagem para build (contém SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 3. Copia arquivos .csproj primeiro (cache de layers)
COPY ["Pacientes.Api/Pacientes.Api.csproj", "Pacientes.Api/"]
COPY ["Pacientes.Application/Pacientes.Application.csproj", "Pacientes.Application/"]
# ...

# 4. Restaura pacotes NuGet
RUN dotnet restore "Pacientes.Api/Pacientes.Api.csproj"

# 5. Copia todo o código
COPY . .

# 6. Compila o projeto
WORKDIR "/src/Pacientes.Api"
RUN dotnet build "Pacientes.Api.csproj" -c Release -o /app/build

# 7. Publica (gera arquivos otimizados)
FROM build AS publish
RUN dotnet publish "Pacientes.Api.csproj" -c Release -o /app/publish

# 8. Imagem final (apenas runtime)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pacientes.Api.dll"]
```

### Como usar:

```bash
# 1. Construir a imagem
docker build -t pacientes-api .

# 2. Rodar o container
docker run -d -p 8080:80 --name pacientes-api-container pacientes-api

# 3. Acessar
# http://localhost:8080/swagger
```

### Docker Compose (Orquestração)

Para projetos mais complexos, use `docker-compose.yml`:

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "8080:80"
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=PacientesDb;User=root;Password=senha;
    depends_on:
      - db
  
  db:
    image: mysql:8.0
    environment:
      - MYSQL_ROOT_PASSWORD=senha
      - MYSQL_DATABASE=PacientesDb
    ports:
      - "3306:3306"
```

### Benefícios Cloud Native:

1. **Escalabilidade Horizontal**: Criar múltiplos containers
2. **Resiliência**: Se um container falha, outros continuam
3. **Deploy Contínuo**: CI/CD automatizado
4. **Monitoramento**: Logs e métricas centralizados
5. **Orquestração**: Kubernetes para gerenciar containers

---

## 🎯 Resumo

Este projeto demonstra:

✅ **API REST** com ASP.NET Core
✅ **Entity Framework Core** para acesso a dados
✅ **MySQL** (facilmente trocável para SQL Server/PostgreSQL)
✅ **Clean Architecture** com separação de responsabilidades
✅ **SOLID** através de interfaces e injeção de dependência
✅ **Docker** para containerização e deploy

**Próximos passos sugeridos:**

- Adicionar autenticação/autorização (JWT)
- Implementar testes unitários e de integração
- Adicionar logging estruturado (Serilog)
- Configurar CI/CD (GitHub Actions, Azure DevOps)
- Adicionar cache (Redis)
- Implementar API Gateway para microserviços
