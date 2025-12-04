## API de Pacientes (.NET 8) com Clean Architecture

Este projeto implementa uma API REST de pacientes usando .NET 8, seguindo uma variação simples de **Clean Architecture**, com camadas:

- **Pacientes.Domain**: Entidades de domínio (`Patient`).
- **Pacientes.Application**: DTOs, interfaces (`IPatientRepository`, `IPatientService`), serviços de aplicação e validações (FluentValidation).
- **Pacientes.Infrastructure**: Infraestrutura de dados com Entity Framework Core (InMemory) e repositórios concretos.
- **Pacientes.Api**: Projeto Web API (controllers, DI, pipeline HTTP).

---

### 1. Pré-requisitos

- **.NET 8 SDK** instalado  
  - Site oficial: `https://dotnet.microsoft.com/download`
- **Docker** (opcional, para rodar via container)  
  - Site oficial: `https://www.docker.com/`

---

### 2. Estrutura de pastas

- `Pacientes.Domain`
  - `Pacientes.Domain.csproj`
  - `Entities/Patient.cs`
- `Pacientes.Application`
  - `Pacientes.Application.csproj`
  - `DTOs/PatientDtos.cs`
  - `Interfaces/IPatientRepository.cs`
  - `Interfaces/IPatientService.cs`
  - `Services/PatientService.cs`
  - `Validation/PatientValidators.cs`
- `Pacientes.Infrastructure`
  - `Pacientes.Infrastructure.csproj`
  - `Data/ApplicationDbContext.cs`
  - `Repositories/PatientRepository.cs`
- `Pacientes.Api`
  - `Pacientes.Api.csproj`
  - `Program.cs`
  - `Controllers/PatientsController.cs`
  - `Properties/launchSettings.json`
- `Dockerfile`

---

### 3. Passo a passo para rodar localmente (sem Docker)

1. **Clonar o repositório** (ou usar os arquivos gerados pelo Cursor):
   - Coloque todos os arquivos nas pastas conforme a estrutura acima.

2. **Restaurar pacotes**:

   ```bash
   dotnet restore Pacientes.Api/Pacientes.Api.csproj
   ```

3. **Rodar a API**:

   ```bash
   dotnet run --project Pacientes.Api/Pacientes.Api.csproj
   ```

4. **Acessar Swagger** (em ambiente de desenvolvimento):
   - URL padrão: `https://localhost:5001/swagger` ou `http://localhost:5000/swagger`

---

### 4. Endpoints principais (CRUD de Pacientes)

Controller: `PatientsController` (`/api/patients`)

- **GET** `/api/patients`
  - Retorna lista de pacientes.
- **GET** `/api/patients/{id}`
  - Retorna um paciente por `id` (`Guid`).
- **POST** `/api/patients`
  - Cria um novo paciente.
  - Body (JSON):

    ```json
    {
      "name": "João da Silva",
      "email": "joao@teste.com",
      "dateOfBirth": "1990-01-01",
      "document": "12345678900"
    }
    ```

- **PUT** `/api/patients/{id}`
  - Atualiza um paciente existente.
- **DELETE** `/api/patients/{id}`
  - Remove um paciente.

---

### 5. Validações (FluentValidation)

As validações são aplicadas sobre os DTOs:

- `PatientCreateDtoValidator`
- `PatientUpdateDtoValidator`

Principais regras:

- `Name`: obrigatório, máximo 200 caracteres.
- `Email`: obrigatório, formato válido, máximo 200 caracteres.
- `Document`: obrigatório, máximo 50 caracteres.
- `DateOfBirth`: deve ser menor que a data atual.

Quando uma validação falhar, a API retorna **400 Bad Request** com os detalhes no corpo da resposta.

---

### 6. Injeção de Dependência (DI) e configuração da API

A configuração de DI e da pipeline HTTP está em `Program.cs` do projeto `Pacientes.Api`:

- **Controllers / API**:
  - Registrados com:

    ```csharp
    builder.Services.AddControllers();
    ```

- **DbContext**:
  - `ApplicationDbContext` com banco InMemory:

    ```csharp
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseInMemoryDatabase("PacientesDb");
    });
    ```

- **Repositório**:
  - `IPatientRepository` → `PatientRepository`.
- **Serviço de aplicação**:
  - `IPatientService` → `PatientService`.
- **FluentValidation**:
  - Registrado com:

    ```csharp
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<PatientCreateDtoValidator>();
    ```

> Observação: os pacotes usados são `FluentValidation` e `FluentValidation.AspNetCore` (versão `11.3.1`), configurados nos arquivos `Pacientes.Application.csproj` e `Pacientes.Api.csproj`.

---

### 7. Rodando com Docker

1. **Build da imagem**:

   ```bash
   docker build -t pacientes-api .
   ```

2. **Rodar o container**:

   ```bash
   docker run -d -p 8080:80 --name pacientes-api-container pacientes-api
   ```

3. **Acessar a API no navegador ou via Postman**:
   - Base URL: `http://localhost:8080`
   - Swagger (se habilitado em produção, por padrão está só em Development):
     - `http://localhost:8080/swagger`

4. **Parar e remover o container** (opcional):

   ```bash
   docker stop pacientes-api-container
   docker rm pacientes-api-container
   ```

---

### 8. Como estender a Clean Architecture

Sugestões de próximos passos:

- **Substituir InMemory por banco real** (SQL Server, PostgreSQL, etc.) criando migrations na camada `Infrastructure`.
- Implementar **padrão CQRS** com comandos/queries separados.
- Adicionar **logs** e **tratamento global de erros** (middleware).
- Criar testes unitários para serviços de aplicação e validações.


