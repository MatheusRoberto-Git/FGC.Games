# 🎮 FGC Games API

Microsserviço de Jogos da plataforma **FIAP Cloud Games (FCG)**.

## 📋 Descrição

Este microsserviço é responsável pelo gerenciamento do catálogo de jogos, incluindo listagem, busca, categorização e controle de preços. Futuramente integrará com Elasticsearch para buscas avançadas e recomendações.

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, separando responsabilidades em camadas:

```
┌─────────────────────────────────────────────────────────────┐
│                    FGC.Games.Presentation                    │
│              (Controllers, Models, Program.cs)               │
├─────────────────────────────────────────────────────────────┤
│                    FGC.Games.Application                     │
│                    (DTOs, Use Cases)                         │
├─────────────────────────────────────────────────────────────┤
│                   FGC.Games.Infrastructure                   │
│              (Repositories, DbContext)                       │
├─────────────────────────────────────────────────────────────┤
│                      FGC.Games.Domain                        │
│             (Entities, Enums, Interfaces)                    │
└─────────────────────────────────────────────────────────────┘
```

### Estrutura de Pastas

```
FGC.Games/
├── FGC.Games.Domain/
│   ├── Common/
│   │   ├── Entities/
│   │   └── Events/
│   ├── Entities/
│   ├── Enums/
│   ├── Events/
│   └── Interfaces/
├── FGC.Games.Application/
│   ├── DTOs/
│   └── UseCases/
├── FGC.Games.Infrastructure/
│   ├── Data/
│   │   ├── Configurations/
│   │   └── Context/
│   └── Repositories/
├── FGC.Games.Presentation/
│   ├── Controllers/
│   ├── Models/
│   │   ├── Requests/
│   │   └── Responses/
│   └── Properties/
├── .github/
│   └── workflows/
│       ├── ci.yml
│       └── cd.yml
├── Dockerfile
└── README.md
```

---

## 🚀 Endpoints

### 🎮 Games Controller (`/api/games`)

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/games` | Listar todos os jogos | - |
| GET | `/api/games/{id}` | Buscar jogo por ID | - |
| GET | `/api/games/search` | Pesquisar jogos | - |
| POST | `/api/games` | Criar novo jogo | Admin |
| PUT | `/api/games/{id}/price` | Atualizar preço | Admin |
| PUT | `/api/games/{id}/deactivate` | Desativar jogo | Admin |
| PUT | `/api/games/{id}/activate` | Ativar jogo | Admin |

### Parâmetros de Busca (`/api/games/search`)

| Parâmetro | Tipo | Descrição |
|-----------|------|-----------|
| `searchTerm` | string | Termo para buscar no título |
| `category` | int | Filtrar por categoria |
| `minPrice` | decimal | Preço mínimo |
| `maxPrice` | decimal | Preço máximo |

---

## 📊 Modelos de Dados

### Game Entity

| Campo | Tipo | Descrição |
|-------|------|-----------|
| Id | Guid | Identificador único |
| Title | string | Título do jogo (max 200) |
| Description | string | Descrição (max 2000) |
| Price | decimal | Preço (0 - 999999.99) |
| Category | GameCategory | Categoria do jogo |
| Developer | string | Desenvolvedor |
| Publisher | string | Publicadora |
| ReleaseDate | DateTime | Data de lançamento |
| CreatedAt | DateTime | Data de criação |
| UpdatedAt | DateTime? | Última atualização |
| IsActive | bool | Status ativo/inativo |
| Rating | double | Avaliação (0-5) |
| TotalSales | int | Total de vendas |

### 🎯 Categorias de Jogos

| Valor | Categoria | Descrição |
|-------|-----------|-----------|
| 0 | Action | Jogos de ação |
| 1 | Adventure | Aventura |
| 2 | RPG | Role-Playing Games |
| 3 | Strategy | Estratégia |
| 4 | Sports | Esportes |
| 5 | Racing | Corrida |
| 6 | Simulation | Simulação |
| 7 | Puzzle | Quebra-cabeça |
| 8 | Horror | Terror |
| 9 | FPS | First-Person Shooter |
| 10 | MMORPG | MMO RPG |
| 11 | Indie | Jogos independentes |
| 12 | Fighting | Luta |
| 13 | Platformer | Plataforma |
| 14 | Sandbox | Mundo aberto |

---

## 🔒 Autenticação

Este microsserviço **valida tokens JWT** emitidos pelo **FGC Users API**.

### Endpoints Públicos (sem autenticação)
- `GET /api/games` - Listar jogos
- `GET /api/games/{id}` - Buscar por ID
- `GET /api/games/search` - Pesquisar

### Endpoints Protegidos (requer Admin)
- `POST /api/games` - Criar jogo
- `PUT /api/games/{id}/price` - Atualizar preço
- `PUT /api/games/{id}/deactivate` - Desativar
- `PUT /api/games/{id}/activate` - Ativar

### Configuração JWT

```json
{
  "Jwt": {
    "SecretKey": "FGC_SuperSecretKey_2024_FIAP_TechChallenge_MinimumLengthRequired32Chars",
    "Issuer": "FGC.Users.API",
    "Audience": "FGC.Client",
    "ExpireMinutes": 120
  }
}
```

> ⚠️ **Importante**: A `SecretKey` deve ser **idêntica** à configurada no Users API.

---

## 🔧 Configuração Local

### Pré-requisitos

- .NET 8.0 SDK
- SQL Server (local ou Azure)
- Visual Studio 2022 ou VS Code

### Executar

```bash
cd FGC.Games.Presentation
dotnet restore
dotnet run
```

A API estará disponível em: `http://localhost:5002`

### Migrations

```bash
# Criar migration
dotnet ef migrations add InitialCreate -p FGC.Games.Infrastructure -s FGC.Games.Presentation

# Aplicar migration
dotnet ef database update -p FGC.Games.Infrastructure -s FGC.Games.Presentation
```

---

## 🐳 Docker

### Build

```bash
docker build -t fgc-games-api .
```

### Run

```bash
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="sua_connection_string" \
  -e Jwt__SecretKey="sua_secret_key" \
  fgc-games-api
```

---

## 📦 Variáveis de Ambiente

| Variável | Descrição | Obrigatório |
|----------|-----------|-------------|
| `ConnectionStrings__DefaultConnection` | Connection string do SQL Server | ✅ |
| `Jwt__SecretKey` | Chave secreta do JWT (min 32 chars) | ✅ |
| `Jwt__Issuer` | Emissor do token | ✅ |
| `Jwt__Audience` | Audiência do token | ✅ |
| `Jwt__ExpireMinutes` | Tempo de expiração em minutos | ✅ |
| `ASPNETCORE_ENVIRONMENT` | Ambiente (Development/Production) | ✅ |

---

## 🔗 Comunicação entre Microsserviços

```
┌─────────────────────┐
│    FGC Users API    │
│   (Autenticação)    │
└─────────┬───────────┘
          │
          │ Token JWT
          ▼
┌─────────────────────┐         ┌─────────────────────┐
│   FGC Games API     │◄───────►│  FGC Payments API   │
│    (Catálogo)       │ GameId  │    (Transações)     │
│   :8080 / :5002     │         │   :8080 / :5003     │
└─────────────────────┘         └─────────────────────┘
          │
          │ Futuramente
          ▼
┌─────────────────────┐
│    Elasticsearch    │
│  (Busca Avançada)   │
└─────────────────────┘
```

### Este microsserviço:
- ✅ **Valida** tokens JWT do Users API
- ✅ **Fornece** dados de jogos para o Payments API
- ✅ **Gerencia** catálogo de jogos

### Dependências:
- 🔵 **Azure SQL Database** - Armazenamento de dados
- 🔵 **Azure Container Instance** - Hospedagem
- 🟡 **Elasticsearch** - Busca avançada

---

## 🚀 CI/CD

### Pipeline CI (Pull Requests)

```yaml
- Checkout do código
- Setup .NET 8.0
- Restore de dependências
- Build da solução
- Execução de testes
```

### Pipeline CD (Push para master)

```yaml
- Checkout do código
- Build e testes
- Login no Azure
- Build da imagem Docker
- Push para Azure Container Registry
- Deploy no Azure Container Instance
- Health check
```

---

## 📍 URLs de Produção

| Ambiente | URL |
|----------|-----|
| **Swagger** | http://fgc-games-api.eastus2.azurecontainer.io:8080 |
| **Health Check** | http://fgc-games-api.eastus2.azurecontainer.io:8080/health |
| **Info** | http://fgc-games-api.eastus2.azurecontainer.io:8080/info |

---

## 🧪 Exemplos de Uso

### Listar Jogos

```bash
curl -X GET http://localhost:5002/api/games
```

### Buscar por Categoria (RPG)

```bash
curl -X GET "http://localhost:5002/api/games/search?category=2"
```

### Criar Jogo (requer token Admin)

```bash
curl -X POST http://localhost:5002/api/games \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  -d '{
    "title": "God of War Ragnarok",
    "description": "Aventura épica com Kratos e Atreus pelos nove reinos",
    "price": 249.90,
    "category": 0,
    "developer": "Santa Monica Studio",
    "publisher": "Sony Interactive Entertainment",
    "releaseDate": "2022-11-09"
  }'
```

### Resposta

```json
{
  "success": true,
  "message": "Jogo criado com sucesso",
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "God of War Ragnarok",
    "description": "Aventura épica com Kratos e Atreus pelos nove reinos",
    "price": 249.90,
    "category": "Action",
    "developer": "Santa Monica Studio",
    "publisher": "Sony Interactive Entertainment",
    "releaseDate": "2022-11-09T00:00:00",
    "createdAt": "2024-12-08T10:30:00Z",
    "isActive": true,
    "rating": 0,
    "totalSales": 0
  }
}
```

### Atualizar Preço

```bash
curl -X PUT http://localhost:5002/api/games/{id}/price \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJhbGciOiJIUzI1NiIs..." \
  -d '{
    "newPrice": 199.90
  }'
```

---

## 🔮 Funcionalidades Futuras

### Elasticsearch Integration
- Indexação de jogos para busca full-text
- Busca por similaridade
- Sugestões baseadas no histórico do usuário
- Agregações para métricas (jogos mais populares)

### Recomendações
- Jogos similares por categoria
- Baseado em compras anteriores
- Trending / Mais vendidos

---

# 📐 Arquitetura FIAP Cloud Games (FCG) - Fase 3

## 🏛️ Visão Geral da Arquitetura

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                      CLIENTES                                            │
│                                                                                          │
│    ┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐      │
│    │   Web App    │     │  Mobile App  │     │   Swagger    │     │   Postman    │      │
│    └──────┬───────┘     └──────┬───────┘     └──────┬───────┘     └──────┬───────┘      │
│           │                    │                    │                    │               │
└───────────┼────────────────────┼────────────────────┼────────────────────┼───────────────┘
            │                    │                    │                    │
            └────────────────────┴────────────────────┴────────────────────┘
                                          │
                                          ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                              AZURE CLOUD INFRASTRUCTURE                                  │
│                                                                                          │
│  ┌───────────────────────────────────────────────────────────────────────────────────┐  │
│  │                           AZURE CONTAINER INSTANCES                                │  │
│  │                                                                                    │  │
│  │   ┌─────────────────────┐  ┌─────────────────────┐  ┌─────────────────────┐       │  │
│  │   │  🔐 FGC Users API   │  │  🎮 FGC Games API   │  │  💳 FGC Payments API│       │  │
│  │   │                     │  │                     │  │                     │       │  │
│  │   │  ┌───────────────┐  │  │  ┌───────────────┐  │  │  ┌───────────────┐  │       │  │
│  │   │  │ Presentation  │  │  │  │ Presentation  │  │  │  │ Presentation  │  │       │  │
│  │   │  ├───────────────┤  │  │  ├───────────────┤  │  │  ├───────────────┤  │       │  │
│  │   │  │ Application   │  │  │  │ Application   │  │  │  │ Application   │  │       │  │
│  │   │  ├───────────────┤  │  │  ├───────────────┤  │  │  ├───────────────┤  │       │  │
│  │   │  │Infrastructure │  │  │  │Infrastructure │  │  │  │Infrastructure │  │       │  │
│  │   │  ├───────────────┤  │  │  ├───────────────┤  │  │  ├───────────────┤  │       │  │
│  │   │  │    Domain     │  │  │  │    Domain     │  │  │  │    Domain     │  │       │  │
│  │   │  └───────────────┘  │  │  └───────────────┘  │  │  └───────────────┘  │       │  │
│  │   │                     │  │                     │  │                     │       │  │
│  │   │  📍 :8080           │  │  📍 :8080           │  │  📍 :8080           │       │  │
│  │   │  fgc-users-api      │  │  fgc-games-api      │  │  fgc-payments-api   │       │  │
│  │   └──────────┬──────────┘  └──────────┬──────────┘  └──────────┬──────────┘       │  │
│  │              │                        │                        │                  │  │
│  └──────────────┼────────────────────────┼────────────────────────┼──────────────────┘  │
│                 │                        │                        │                     │
│                 └────────────────────────┼────────────────────────┘                     │
│                                          │                                              │
│                                          ▼                                              │
│  ┌───────────────────────────────────────────────────────────────────────────────────┐  │
│  │                              AZURE SQL DATABASE                                    │  │
│  │                                                                                    │  │
│  │   ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐               │  │
│  │   │  📋 Users       │    │  📋 Games       │    │  📋 Payments    │               │  │
│  │   │                 │    │                 │    │                 │               │  │
│  │   │  - Id           │    │  - Id           │    │  - Id           │               │  │
│  │   │  - Name         │    │  - Title        │    │  - UserId       │               │  │
│  │   │  - Email        │    │  - Description  │    │  - GameId       │               │  │
│  │   │  - Password     │    │  - Price        │    │  - Amount       │               │  │
│  │   │  - Role         │    │  - Category     │    │  - Status       │               │  │
│  │   │  - IsActive     │    │  - Developer    │    │  - Method       │               │  │
│  │   │  - CreatedAt    │    │  - Publisher    │    │  - TransactionId│               │  │
│  │   │  - LastLoginAt  │    │  - ReleaseDate  │    │  - CreatedAt    │               │  │
│  │   │                 │    │  - IsActive     │    │  - ProcessedAt  │               │  │
│  │   │                 │    │  - Rating       │    │  - CompletedAt  │               │  │
│  │   │                 │    │  - TotalSales   │    │  - FailureReason│               │  │
│  │   └─────────────────┘    └─────────────────┘    └─────────────────┘               │  │
│  │                                                                                    │  │
│  │   📍 fgc-sql-server.database.windows.net                                          │  │
│  │   📁 fgc-database                                                                 │  │
│  └───────────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                          │
│  ┌───────────────────────────────────────────────────────────────────────────────────┐  │
│  │                          AZURE CONTAINER REGISTRY                                  │  │
│  │                                                                                    │  │
│  │   🐳 fgcregistry.azurecr.io                                                       │  │
│  │                                                                                    │  │
│  │   ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                   │  │
│  │   │ fgc-users-api   │  │ fgc-games-api   │  │fgc-payments-api │                   │  │
│  │   │    :latest      │  │    :latest      │  │    :latest      │                   │  │
│  │   └─────────────────┘  └─────────────────┘  └─────────────────┘                   │  │
│  └───────────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                          │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Fluxo de Comunicação entre Microsserviços

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                          │
│                              FLUXO DE AUTENTICAÇÃO JWT                                   │
│                                                                                          │
│    ┌──────────┐         POST /api/auth/login              ┌──────────────────┐          │
│    │          │ ─────────────────────────────────────────►│                  │          │
│    │  Client  │         { email, password }               │  FGC Users API   │          │
│    │          │◄───────────────────────────────────────── │                  │          │
│    └────┬─────┘         { token: "eyJ..." }               └──────────────────┘          │
│         │                                                                                │
│         │                                                                                │
│         │  Authorization: Bearer eyJ...                                                  │
│         │                                                                                │
│         ├─────────────────────────────────────────────────────────────────┐              │
│         │                                                                 │              │
│         ▼                                                                 ▼              │
│    ┌──────────────────┐                                    ┌──────────────────┐         │
│    │                  │         Valida JWT                 │                  │         │
│    │  FGC Games API   │         (mesma SecretKey)          │FGC Payments API  │         │
│    │                  │                                    │                  │         │
│    └──────────────────┘                                    └──────────────────┘         │
│                                                                                          │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 🛒 Fluxo de Compra de Jogo

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                          │
│                               FLUXO DE COMPRA DE JOGO                                    │
│                                                                                          │
│    ┌──────────┐                                                                          │
│    │  Client  │                                                                          │
│    └────┬─────┘                                                                          │
│         │                                                                                │
│         │ 1️⃣ POST /api/auth/login                                                        │
│         ▼                                                                                │
│    ┌──────────────────┐                                                                  │
│    │  FGC Users API   │  ──►  Valida credenciais                                        │
│    │                  │  ──►  Retorna JWT Token                                         │
│    └────────┬─────────┘                                                                  │
│             │                                                                            │
│             │ { token }                                                                  │
│             ▼                                                                            │
│    ┌──────────┐                                                                          │
│    │  Client  │                                                                          │
│    └────┬─────┘                                                                          │
│         │                                                                                │
│         │ 2️⃣ GET /api/games (com Bearer Token)                                           │
│         ▼                                                                                │
│    ┌──────────────────┐                                                                  │
│    │  FGC Games API   │  ──►  Valida JWT                                                │
│    │                  │  ──►  Retorna lista de jogos                                    │
│    └────────┬─────────┘                                                                  │
│             │                                                                            │
│             │ { games[] }                                                                │
│             ▼                                                                            │
│    ┌──────────┐                                                                          │
│    │  Client  │  ──►  Usuário escolhe um jogo                                           │
│    └────┬─────┘                                                                          │
│         │                                                                                │
│         │ 3️⃣ POST /api/payments (com Bearer Token)                                       │
│         │    { userId, gameId, amount, paymentMethod }                                   │
│         ▼                                                                                │
│    ┌──────────────────┐                                                                  │
│    │FGC Payments API  │  ──►  Valida JWT                                                │
│    │                  │  ──►  Cria pagamento (status: Pending)                          │
│    └────────┬─────────┘                                                                  │
│             │                                                                            │
│             │ { payment: { id, status: "Pending" } }                                     │
│             ▼                                                                            │
│    ┌──────────┐                                                                          │
│    │  Client  │                                                                          │
│    └────┬─────┘                                                                          │
│         │                                                                                │
│         │ 4️⃣ POST /api/payments/{id}/process                                             │
│         ▼                                                                                │
│    ┌──────────────────┐                                                                  │
│    │FGC Payments API  │  ──►  Processa pagamento                                        │
│    │                  │  ──►  Atualiza status (Completed/Failed)                        │
│    └────────┬─────────┘                                                                  │
│             │                                                                            │
│             │ { payment: { status: "Completed" } }                                       │
│             ▼                                                                            │
│    ┌──────────┐                                                                          │
│    │  Client  │  ──►  ✅ Compra finalizada!                                              │
│    └──────────┘                                                                          │
│                                                                                          │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 🔧 Pipeline CI/CD

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                                                                          │
│                                    CI/CD PIPELINE                                        │
│                                                                                          │
│    ┌──────────────────────────────────────────────────────────────────────────────────┐ │
│    │                              GITHUB REPOSITORIES                                  │ │
│    │                                                                                   │ │
│    │   ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                  │ │
│    │   │ fgc-users-api   │  │ fgc-games-api   │  │fgc-payments-api │                  │ │
│    │   └────────┬────────┘  └────────┬────────┘  └────────┬────────┘                  │ │
│    │            │                    │                    │                           │ │
│    └────────────┼────────────────────┼────────────────────┼───────────────────────────┘ │
│                 │                    │                    │                             │
│                 └────────────────────┼────────────────────┘                             │
│                                      │                                                  │
│                                      ▼                                                  │
│    ┌──────────────────────────────────────────────────────────────────────────────────┐ │
│    │                            GITHUB ACTIONS                                         │ │
│    │                                                                                   │ │
│    │   ┌─────────────────────────────────────────────────────────────────────────┐    │ │
│    │   │                        CI (Pull Requests)                                │    │ │
│    │   │                                                                          │    │ │
│    │   │   📥 Checkout  ──►  🔧 Setup .NET  ──►  📦 Restore  ──►  🏗️ Build  ──►  🧪 Test │ │
│    │   └─────────────────────────────────────────────────────────────────────────┘    │ │
│    │                                                                                   │ │
│    │   ┌─────────────────────────────────────────────────────────────────────────┐    │ │
│    │   │                        CD (Push to master)                               │    │ │
│    │   │                                                                          │    │ │
│    │   │   📥 Checkout  ──►  🏗️ Build  ──►  🧪 Test  ──►  🔐 Azure Login          │    │ │
│    │   │        │                                              │                  │    │ │
│    │   │        ▼                                              ▼                  │    │ │
│    │   │   🐳 Docker Build  ──►  📤 Push ACR  ──►  🚀 Deploy ACI  ──►  🏥 Health  │    │ │
│    │   └─────────────────────────────────────────────────────────────────────────┘    │ │
│    └──────────────────────────────────────────────────────────────────────────────────┘ │
│                                      │                                                  │
│                                      ▼                                                  │
│    ┌──────────────────────────────────────────────────────────────────────────────────┐ │
│    │                              AZURE RESOURCES                                      │ │
│    │                                                                                   │ │
│    │   ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                  │ │
│    │   │ Container       │  │ Container       │  │   SQL Server    │                  │ │
│    │   │ Registry (ACR)  │  │ Instances (ACI) │  │   Database      │                  │ │
│    │   └─────────────────┘  └─────────────────┘  └─────────────────┘                  │ │
│    └──────────────────────────────────────────────────────────────────────────────────┘ │
│                                                                                          │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 📊 Recursos Azure

| Recurso | Nome | Tipo | Região |
|---------|------|------|--------|
| Resource Group | `rg-fgc-api` | Resource Group | East US 2 |
| SQL Server | `fgc-sql-server` | Azure SQL Server | East US 2 |
| Database | `fgc-database` | Azure SQL Database | East US 2 |
| Container Registry | `fgcregistry` | Azure Container Registry | East US 2 |
| Container Instance | `fgc-users-container` | Azure Container Instance | East US 2 |
| Container Instance | `fgc-games-container` | Azure Container Instance | East US 2 |
| Container Instance | `fgc-payments-container` | Azure Container Instance | East US 2 |

---

## 🌐 URLs de Produção

| Microsserviço | URL | Swagger |
|---------------|-----|---------|
| **Users API** | http://fgc-users-api.eastus2.azurecontainer.io:8080 | ✅ |
| **Games API** | http://fgc-games-api.eastus2.azurecontainer.io:8080 | ✅ |
| **Payments API** | http://fgc-payments-api.eastus2.azurecontainer.io:8080 | ✅ |

---

## 🔐 Segurança

### JWT Token Flow

```
┌────────────────────────────────────────────────────────────────┐
│                        JWT TOKEN                                │
│                                                                 │
│  Header:     { "alg": "HS256", "typ": "JWT" }                  │
│                                                                 │
│  Payload:    {                                                  │
│                "sub": "user-id",                               │
│                "email": "user@email.com",                      │
│                "role": "Admin",                                │
│                "exp": 1702044800                               │
│              }                                                  │
│                                                                 │
│  Signature:  HMACSHA256(                                       │
│                base64UrlEncode(header) + "." +                 │
│                base64UrlEncode(payload),                       │
│                secret_key                                       │
│              )                                                  │
│                                                                 │
└────────────────────────────────────────────────────────────────┘
```

### Mesma Secret Key em todos os microsserviços:

```
FGC_SuperSecretKey_2024_FIAP_TechChallenge_MinimumLengthRequired32Chars
```

---

## 📁 Estrutura dos Repositórios

```
GitHub Organization/User
│
├── 📁 fgc-users-api/
│   ├── 📁 .github/workflows/
│   │   ├── ci.yml
│   │   └── cd.yml
│   ├── 📁 FGC.Users.Domain/
│   ├── 📁 FGC.Users.Application/
│   ├── 📁 FGC.Users.Infrastructure/
│   ├── 📁 FGC.Users.Presentation/
│   ├── 🐳 Dockerfile
│   ├── 📄 FGC.Users.sln
│   └── 📖 README.md
│
├── 📁 fgc-games-api/
│   ├── 📁 .github/workflows/
│   │   ├── ci.yml
│   │   └── cd.yml
│   ├── 📁 FGC.Games.Domain/
│   ├── 📁 FGC.Games.Application/
│   ├── 📁 FGC.Games.Infrastructure/
│   ├── 📁 FGC.Games.Presentation/
│   ├── 🐳 Dockerfile
│   ├── 📄 FGC.Games.sln
│   └── 📖 README.md
│
└── 📁 fgc-payments-api/
    ├── 📁 .github/workflows/
    │   ├── ci.yml
    │   └── cd.yml
    ├── 📁 FGC.Payments.Domain/
    ├── 📁 FGC.Payments.Application/
    ├── 📁 FGC.Payments.Infrastructure/
    ├── 📁 FGC.Payments.Presentation/
    ├── 🐳 Dockerfile
    ├── 📄 FGC.Payments.sln
    └── 📖 README.md
```

---

## 📄 Licença

FIAP - Pós-Graduação em Arquitetura de Software .NET

**Tech Challenge - Fase 3**