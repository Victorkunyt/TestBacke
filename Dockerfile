# ============================================================================
# DOCKERFILE - Containerização Cloud Native
# ============================================================================
# 
# Docker permite empacotar a aplicação com todas suas dependências
# em um container isolado e portável.
# 
# Benefícios:
# - Portabilidade: "Funciona na minha máquina" → Funciona em qualquer lugar
# - Isolamento: Cada container é independente
# - Escalabilidade: Fácil criar múltiplas instâncias
# - Consistência: Mesmo ambiente em dev, test, produção
# 
# Multi-stage build:
# - Reduz tamanho da imagem final (não inclui SDK, apenas runtime)
# - Otimiza cache de layers do Docker
# - Imagem final mais leve e segura

# ============================================================================
# STAGE 1: Base Runtime Image (imagem leve para executar)
# ============================================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80   # Porta HTTP
EXPOSE 443  # Porta HTTPS

# ============================================================================
# STAGE 2: Build Stage (contém SDK para compilar)
# ============================================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas arquivos .csproj primeiro (otimização de cache)
# Se os .csproj não mudarem, Docker reutiliza o cache desta layer
COPY ["Pacientes.Api/Pacientes.Api.csproj", "Pacientes.Api/"]
COPY ["Pacientes.Application/Pacientes.Application.csproj", "Pacientes.Application/"]
COPY ["Pacientes.Domain/Pacientes.Domain.csproj", "Pacientes.Domain/"]
COPY ["Pacientes.Infrastructure/Pacientes.Infrastructure.csproj", "Pacientes.Infrastructure/"]

# Restaura pacotes NuGet (dependências)
RUN dotnet restore "Pacientes.Api/Pacientes.Api.csproj"

# Copia todo o código fonte
COPY . .

# Compila o projeto em modo Release (otimizado)
WORKDIR "/src/Pacientes.Api"
RUN dotnet build "Pacientes.Api.csproj" -c Release -o /app/build

# ============================================================================
# STAGE 3: Publish Stage (gera arquivos otimizados para produção)
# ============================================================================
FROM build AS publish
RUN dotnet publish "Pacientes.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ============================================================================
# STAGE 4: Final Image (imagem final leve, apenas runtime)
# ============================================================================
FROM base AS final
WORKDIR /app

# Copia apenas os arquivos publicados (não inclui código fonte, SDK, etc.)
COPY --from=publish /app/publish .

# Comando executado quando o container iniciar
ENTRYPOINT ["dotnet", "Pacientes.Api.dll"]

# ============================================================================
# COMO USAR:
# ============================================================================
# 
# 1. Construir a imagem:
#    docker build -t pacientes-api .
# 
# 2. Rodar o container:
#    docker run -d -p 8080:80 --name pacientes-api-container pacientes-api
# 
# 3. Acessar:
#    http://localhost:8080/swagger
# 
# 4. Ver logs:
#    docker logs pacientes-api-container
# 
# 5. Parar:
#    docker stop pacientes-api-container
#    docker rm pacientes-api-container


