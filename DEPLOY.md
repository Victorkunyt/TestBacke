# 🚀 Guia de Deploy - API Pacientes

Este guia explica como fazer deploy da API na nuvem com ambientes de **Desenvolvimento** e **Produção**.

## ⚠️ Importante: Vercel e .NET

O **Vercel não suporta aplicações .NET Core** nativamente. Ele é otimizado para:
- Node.js, Python, Go, Ruby
- Frameworks frontend (Next.js, React, Vue)

Para APIs .NET Core, recomendamos as seguintes plataformas:

---

## 🎯 Opção Recomendada: Railway

**Railway** é similar ao Vercel em facilidade, mas suporta Docker e .NET Core.

### ✅ Vantagens:
- ✅ Integração fácil com GitHub (deploy automático)
- ✅ Suporte nativo a Docker
- ✅ Ambiente Dev e Produção separados
- ✅ Banco de dados MySQL incluso
- ✅ Plano gratuito generoso
- ✅ Interface simples e intuitiva

### 📋 Passo a Passo - Railway

#### 1. Criar conta no Railway
1. Acesse [railway.app](https://railway.app)
2. Faça login com GitHub
3. Clique em **"New Project"**

#### 2. Conectar repositório GitHub
1. Selecione **"Deploy from GitHub repo"**
2. Escolha seu repositório
3. Railway detectará automaticamente o `Dockerfile`

#### 3. Configurar variáveis de ambiente

No Railway, vá em **Variables** e adicione:

```env
# Connection String do MySQL
CONNECTION_STRING=Server=mysql.railway.internal;Database=railway;User=root;Password=SUA_SENHA;Port=3306;

# Ambiente
ASPNETCORE_ENVIRONMENT=Production
```

**💡 Dica:** Railway cria automaticamente um serviço MySQL. Use a connection string fornecida por ele.

#### 4. Criar banco de dados MySQL no Railway
1. No projeto Railway, clique em **"+ New"**
2. Selecione **"Database"** → **"MySQL"**
3. Railway criará automaticamente e fornecerá a connection string
4. Copie a connection string e adicione na variável `CONNECTION_STRING`

#### 5. Executar migrations

Após o primeiro deploy, execute as migrations:

**Opção A - Via Railway CLI:**
```bash
# Instalar Railway CLI
npm i -g @railway/cli

# Login
railway login

# Conectar ao projeto
railway link

# Executar migrations
railway run dotnet ef database update --project Pacientes.Infrastructure --startup-project Pacientes.Api
```

**Opção B - Via SSH no Railway:**
1. No Railway, vá em **Settings** → **Service**
2. Ative **"Generate SSH Config"**
3. Conecte via SSH e execute:
```bash
cd /app
dotnet ef database update --project Pacientes.Infrastructure --startup-project Pacientes.Api
```

#### 6. Configurar ambiente de Desenvolvimento

1. No Railway, crie um **novo serviço** para Dev
2. Conecte ao mesmo repositório
3. Configure uma **branch diferente** (ex: `develop`)
4. Use um banco MySQL separado para Dev
5. Configure variáveis:
```env
ASPNETCORE_ENVIRONMENT=Development
CONNECTION_STRING=... (banco de dev)
```

#### 7. Acessar a API

Após o deploy, Railway fornecerá uma URL como:
- `https://seu-projeto.up.railway.app`

Acesse:
- API: `https://seu-projeto.up.railway.app/api/patients`
- Swagger (se habilitado): `https://seu-projeto.up.railway.app/swagger`

---

## 🌐 Outras Opções de Deploy

### 1. Render

**Similar ao Railway, também fácil de usar:**

1. Acesse [render.com](https://render.com)
2. Conecte GitHub
3. Crie um **Web Service** → **Docker**
4. Configure variáveis de ambiente
5. Adicione serviço **PostgreSQL** ou **MySQL**

**Vantagens:**
- ✅ Plano gratuito
- ✅ Deploy automático do GitHub
- ✅ Suporte a Docker

---

### 2. Fly.io

**Suporta .NET nativamente:**

1. Instale Fly CLI: `curl -L https://fly.io/install.sh | sh`
2. Login: `fly auth login`
3. No diretório do projeto: `fly launch`
4. Configure variáveis: `fly secrets set CONNECTION_STRING="..."`

**Vantagens:**
- ✅ Suporte nativo .NET
- ✅ Muito rápido
- ✅ Plano gratuito

---

### 3. Azure App Service

**Melhor para .NET (Microsoft):**

1. Acesse [portal.azure.com](https://portal.azure.com)
2. Crie **App Service** → **.NET 8**
3. Conecte GitHub para CI/CD
4. Configure **Azure Database for MySQL**

**Vantagens:**
- ✅ Integração perfeita com .NET
- ✅ Escalabilidade automática
- ✅ Muitos recursos Microsoft

---

## 🔧 Configurações Importantes

### Variáveis de Ambiente

A API usa as seguintes variáveis (defina na plataforma escolhida):

| Variável | Descrição | Exemplo |
|----------|-----------|---------|
| `CONNECTION_STRING` | String de conexão MySQL | `Server=host;Database=db;User=user;Password=pass;Port=3306;` |
| `ASPNETCORE_ENVIRONMENT` | Ambiente da aplicação | `Production` ou `Development` |
| `PORT` | Porta da aplicação | `5000` (geralmente definido automaticamente) |

### Connection String Format

```env
Server=HOST;Database=NOME_DB;User=USUARIO;Password=SENHA;Port=3306;
```

### Executar Migrations

Após o primeiro deploy, sempre execute as migrations:

```bash
dotnet ef database update --project Pacientes.Infrastructure --startup-project Pacientes.Api
```

---

## 🐳 Docker Local (Teste antes de deploy)

Antes de fazer deploy, teste localmente com Docker:

```bash
# Build da imagem
docker build -t pacientes-api .

# Rodar container
docker run -d \
  -p 8080:80 \
  -e CONNECTION_STRING="Server=host.docker.internal;Database=PacientesDb;User=root;Password=;Port=3306;" \
  --name pacientes-api \
  pacientes-api

# Ver logs
docker logs pacientes-api

# Parar
docker stop pacientes-api
docker rm pacientes-api
```

---

## 📝 Checklist de Deploy

- [ ] Código commitado no GitHub
- [ ] Dockerfile testado localmente
- [ ] Variáveis de ambiente configuradas
- [ ] Banco de dados criado na plataforma
- [ ] Connection string configurada
- [ ] Migrations executadas
- [ ] API acessível via URL pública
- [ ] Testes realizados (GET, POST, PUT, DELETE)

---

## 🆘 Troubleshooting

### Erro: "Connection string not found"
- Verifique se a variável `CONNECTION_STRING` está configurada
- Ou use `DefaultConnection` no `appsettings.json`

### Erro: "Cannot connect to database"
- Verifique se o banco MySQL está rodando
- Confirme se a connection string está correta
- Verifique firewall/rede da plataforma

### Erro: "Port already in use"
- A plataforma define a porta automaticamente via variável `PORT`
- Não defina porta manualmente

### Swagger não aparece em produção
- Por padrão, Swagger só aparece em `Development`
- Para habilitar em produção, ajuste `Program.cs`:
```csharp
// Remover o if (app.Environment.IsDevelopment())
app.UseSwagger();
app.UseSwaggerUI();
```

---

## 📚 Recursos

- [Railway Docs](https://docs.railway.app)
- [Render Docs](https://render.com/docs)
- [Fly.io Docs](https://fly.io/docs)
- [Azure App Service Docs](https://docs.microsoft.com/azure/app-service)

---

## 💡 Dicas

1. **Sempre teste localmente com Docker antes de fazer deploy**
2. **Use ambientes separados** (Dev/Prod) para evitar problemas
3. **Nunca commite secrets** (connection strings, senhas) no Git
4. **Monitore logs** da plataforma para identificar problemas
5. **Configure CI/CD** para deploy automático em cada push

---

**Pronto! Sua API está configurada para deploy na nuvem! 🎉**

