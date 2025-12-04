FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Pacientes.Api/Pacientes.Api.csproj", "Pacientes.Api/"]
COPY ["Pacientes.Application/Pacientes.Application.csproj", "Pacientes.Application/"]
COPY ["Pacientes.Domain/Pacientes.Domain.csproj", "Pacientes.Domain/"]
COPY ["Pacientes.Infrastructure/Pacientes.Infrastructure.csproj", "Pacientes.Infrastructure/"]

RUN dotnet restore "Pacientes.Api/Pacientes.Api.csproj"

COPY . .
WORKDIR "/src/Pacientes.Api"
RUN dotnet build "Pacientes.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pacientes.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Pacientes.Api.dll"]


