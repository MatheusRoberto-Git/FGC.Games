# 🎮 FGC Games API

Microsserviço de Jogos da plataforma **FIAP Cloud Games (FCG)**.

## 📋 Descrição

Este microsserviço é responsável pelo gerenciamento do catálogo de jogos, incluindo listagem, busca, categorização e controle de preços. Integra com Elasticsearch para buscas avançadas.

---

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, separando responsabilidades em camadas:

```
┌─────────────────────────────────────────────────────────────┐
│                    FGC.Games.Presentation                   │
│              (Controllers, Models, Program.cs)              │
├─────────────────────────────────────────────────────────────┤
│                    FGC.Games.Application                    │
│                    (DTOs, Use Cases)                        │
├─────────────────────────────────────────────────────────────┤
│                   FGC.Games.Infrastructure                  │
│              (Repositories, DbContext)                      │
├─────────────────────────────────────────────────────────────┤
│                      FGC.Games.Domain                       │
│             (Entities, Enums, Interfaces)                   │
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
    "SecretKey": "FGC_SuperSecretKey_2024_MinLength32Chars!",
    "Issuer": "FGC.Users.API",
    "Audience": "FGC.Client",
    "ExpireMinutes": 120
  }
}
```

> ⚠️ **Importante**: A `SecretKey` deve ser **idêntica** à configurada no Users API.

---

## 🐳 Docker - Imagem Otimizada (Fase 4)

O Dockerfile foi otimizado na **Fase 4** com as seguintes melhorias:

| Melhoria | Antes | Depois |
|----------|-------|--------|
| **Tamanho da imagem** | ~93 MB | ~64 MB |
| **Imagem base** | aspnet:8.0 | aspnet:8.0-alpine |
| **Usuário** | root | appuser (non-root) |
| **Health check** | Não tinha | Integrado |

### Dockerfile Otimizado

```dockerfile
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src
# ... build steps

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Stage 3: Runtime (Alpine)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
RUN addgroup -g 1000 appgroup && adduser -u 1000 -G appgroup -D appuser
USER appuser
HEALTHCHECK --interval=30s --timeout=10s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1
```

### Build & Run

```bash
# Build
docker build -t fgc-games-api .

# Run
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="sua_connection_string" \
  -e Jwt__SecretKey="sua_secret_key" \
  fgc-games-api
```

---

## ☸️ Kubernetes (AKS) - Fase 4

Na Fase 4, o microsserviço foi migrado para **Azure Kubernetes Service (AKS)**.

### Recursos Kubernetes

| Recurso | Descrição |
|---------|-----------|
| **Deployment** | Gerencia os pods da aplicação |
| **Service (ClusterIP)** | Exposição interna |
| **HPA** | Auto scaling baseado em CPU (1-5 pods) |
| **ConfigMap** | Configurações não sensíveis |
| **Secret** | Dados sensíveis (connection strings, JWT) |

### HPA (Horizontal Pod Autoscaler)

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
metadata:
  name: games-api-hpa
  namespace: fgc
spec:
  scaleTargetRef:
    apiVersion: apps/v1
    kind: Deployment
    name: games-api
  minReplicas: 1
  maxReplicas: 5
  metrics:
  - type: Resource
    resource:
      name: cpu
      target:
        type: Utilization
        averageUtilization: 70
```

### Comandos Úteis

```bash
# Ver pods
kubectl get pods -n fgc

# Ver logs
kubectl logs -n fgc deployment/games-api

# Ver HPA
kubectl get hpa -n fgc

# Escalar manualmente
kubectl scale deployment/games-api --replicas=3 -n fgc
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
| `Elasticsearch__Uri` | URI do Elasticsearch | ⬜ |
| `ApplicationInsights__ConnectionString` | APM monitoring | ⬜ |

---

# 📐 Arquitetura FIAP Cloud Games (FCG) - Fase 4

## 🏛️ Visão Geral da Arquitetura

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                                      CLIENTES                                           │
│    ┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐      │
│    │   Web App    │     │  Mobile App  │     │   Swagger    │     │   Postman    │      │
│    └──────┬───────┘     └──────┬───────┘     └──────┬───────┘     └──────┬───────┘      │
└───────────┼────────────────────┼────────────────────┼────────────────────┼──────────────┘
            └────────────────────┴────────────────────┴────────────────────┘
                                          │
                                          ▼
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                              AZURE CLOUD INFRASTRUCTURE                                 │
│                                                                                         │
│  ┌───────────────────────────────────────────────────────────────────────────────────┐  │
│  │                        AZURE KUBERNETES SERVICE (AKS)                             │  │
│  │                            fgc-aks-cluster                                        │  │
│  │                                                                                   │  │
│  │   ┌─────────────────────┐  ┌─────────────────────┐  ┌─────────────────────┐       │  │
│  │   │  🔐 FGC Users API   │  │  🎮 FGC Games API   │  │ 💳 FGC Payments API│       │  │
│  │   │      (Pod)          │  │      (Pod)          │  │      (Pod)          │       │  │
│  │   │   HPA: 1-5 pods     │  │   HPA: 1-5 pods     │  │   HPA: 1-5 pods     │       │  │
│  │   │   CPU target: 70%   │  │   CPU target: 70%   │  │   CPU target: 70%   │       │  │
│  │   │                     │  │                     │  │                     │       │  │
│  │   │  📍 LoadBalancer    │  │  📍 ClusterIP       │  │  📍 LoadBalancer   │       │  │
│  │   │  68.220.143.16      │  │  (interno)          │  │  128.85.227.213     │       │  │
│  │   └──────────┬──────────┘  └──────────┬──────────┘  └──────────┬──────────┘       │  │
│  └──────────────┼────────────────────────┼────────────────────────┼──────────────────┘  │
│                 │                        │                        │                     │
│                 └────────────────────────┼────────────────────────┘                     │
│                                          │                                              │
│                            ┌─────────────┴─────────────┐                                │
│                            ▼                           ▼                                │
│  ┌─────────────────────────────────────┐  ┌────────────────────────────────────────-─┐  │
│  │        AZURE SQL DATABASE           │  │         AZURE SERVICE BUS                │  │
│  │   📍 fgc-sql-server.database.       │  │   📍 fgc-servicebus                     │  │
│  │      windows.net                    │  │   📬 Queue: payment-notifications       │  │
│  │   📁 fgc-database                   │  │                                         │  │
│  └─────────────────────────────────────┘  └────────────────────────────────────────-─┘ │
│                                                                                        │
│  ┌─────────────────────────────────────┐  ┌─────────────────────────────────────────┐  │
│  │      AZURE CONTAINER REGISTRY       │  │       APPLICATION INSIGHTS (APM)        │  │
│  │   🐳 fgcregistry.azurecr.io         │  │   📊 Métricas de performance            │  │
│  │   • fgc-users-api:latest            │  │   📈 Logs e traces distribuídos         │  │
│  │   • fgc-games-api:latest            │  │   🔍 Monitoramento em tempo real        │  │
│  │   • fgc-payments-api:latest         │  │                                         │   │
│  └─────────────────────────────────────┘  └─────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 🔄 Fluxo de Comunicação Assíncrona (Service Bus)

```
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│                         FLUXO DE MENSAGERIA ASSÍNCRONA                                  │
│                                                                                         │
│    ┌──────────┐      POST /api/payments/{id}/process      ┌──────────────────┐          │
│    │  Client  │ ─────────────────────────────────────────►│  Payments API    │          │
│    └──────────┘                                           └────────┬─────────┘          │
│                                                                    │                    │
│                                                                    │ Publica mensagem   │
│                                                                    ▼                    │
│                                                           ┌──────────────────┐          │
│                                                           │  Azure Service   │          │
│                                                           │      Bus         │          │
│                                                           │  Queue:          │          │
│                                                           │  payment-        │          │
│                                                           │  notifications   │          │
│                                                           └────────┬─────────┘          │
│                                                                    │                    │
│                                                                    │ Consome mensagem   │
│                                                                    ▼                    │
│                                                           ┌──────────────────┐          │
│                                                           │  Azure Function  │          │
│                                                           │   (Consumer)     │          │
│                                                           │  • Notificações  │          │
│                                                           │  • Webhooks      │          │
│                                                           └──────────────────┘          │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 🔗 Comunicação entre Microsserviços

```
                    ┌─────────────────────┐
                    │    FGC Users API    │
                    │   (Autenticação)    │
                    │  68.220.143.16      │
                    └─────────┬───────────┘
                              │
                    Gera Token JWT
                              │
           ┌──────────────────┼──────────────────┐
           │                  │                  │
           ▼                  ▼                  ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│  FGC Games API  │ │FGC Payments API │ │  Outros Clients │
│   (Catálogo)    │ │  (Transações)   │ │   (Frontend)    │
│  ClusterIP      │ │ 128.85.227.213  │ │                 │
└─────────────────┘ └────────┬────────┘ └─────────────────┘
                             │
                             ▼
                   ┌─────────────────┐
                   │  Service Bus    │
                   │  (Mensageria)   │
                   └─────────────────┘
```

### Este microsserviço:
- ✅ **Valida** tokens JWT do Users API
- ✅ **Fornece** dados de jogos para o Payments API
- ✅ **Gerencia** catálogo de jogos

### Dependências:
- 🔵 **Azure SQL Database** - Armazenamento de dados
- 🔵 **Azure Kubernetes Service** - Orquestração de containers
- 🔵 **Elasticsearch** - Busca avançada (Elastic Cloud)
- 🔵 **Application Insights** - Monitoramento APM

---

## 🔧 Pipeline CI/CD

```
┌──────────────────────────────────────────────────────────────────────────────────────-───┐
│                                    CI/CD PIPELINE                                        │
│                                                                                          │
│    ┌──────────────────────────────────────────────────────────────────────────────────┐  │
│    │                              GITHUB REPOSITORIES                                    │
│    │   ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐                  │  │
│    │   │ fgc-users-api   │  │ fgc-games-api   │  │fgc-payments-api │                  │  │
│    │   └────────┬────────┘  └────────┬────────┘  └────────┬────────┘                  │  │
│    └────────────┼────────────────────┼────────────────────┼───────────────────────────┘  │
│                 └────────────────────┼────────────────────┘                              │
│                                      ▼                                                   │
│    ┌──────────────────────────────────────────────────────────────────────────────────┐  │
│    │                            GITHUB ACTIONS                                        │  │
│    │                                                                                  │  │
│    │   ┌────────────────────────────────────────────────────────────────────────-─┐   │  │
│    │   │  CI (Pull Requests)                                                      │   │  │
│    │   │  📥 Checkout ──► 🔧 Setup .NET ──► 📦 Restore ──► 🏗️ Build ──► 🧪 Test │   │  │
│    │   └─────────────────────────────────────────────────────────────────────────-┘   │  │
│    │                                                                                  │  │
│    │   ┌─────────────────────────────────────────────────────────────────────────-┐   │  │
│    │   │  CD (Push to master)                                                     │   │  │
│    │   │  📥 Checkout ──► 🏗️ Build ──► 🧪 Test ──► 🔐 Azure Login                │   │  │
│    │   │       │                                         │                        │   │  │
│    │   │       ▼                                         ▼                        │   │  │
│    │   │  🐳 Docker Build ──► 📤 Push ACR ──► 🚀 Deploy ──► 🏥 Health Check      │   │  │
│    │   └────────────────────────────────────────────────────────────────────────-─┘   │  │
│    └─────────────────────────────────────────────────────────────────────────────────-┘  │
└───────────────────────────────────────────────────────────────────────────────────────-──┘
```

---

## 📊 Recursos Azure - Fase 4

| Recurso | Nome | Tipo | Região |
|---------|------|------|--------|
| Resource Group | `rg-fgc-api` | Resource Group | East US 2 |
| AKS Cluster | `fgc-aks-cluster` | Azure Kubernetes Service | East US 2 |
| SQL Server | `fgc-sql-server` | Azure SQL Server | East US 2 |
| Database | `fgc-database` | Azure SQL Database | East US 2 |
| Container Registry | `fgcregistry` | Azure Container Registry | East US 2 |
| Service Bus | `fgc-servicebus` | Azure Service Bus | East US 2 |
| App Insights | `fgc-appinsights` | Application Insights | East US 2 |

---

## 🌐 URLs de Produção (Kubernetes)

| Microsserviço | URL | Swagger |
|---------------|-----|---------|
| **Users API** | http://68.220.143.16 | ✅ |
| **Games API** | Interno (ClusterIP) | - |
| **Payments API** | http://128.85.227.213 | ✅ |

> **Nota**: Games API está configurado como ClusterIP (acesso interno) por design. É acessado internamente pelos outros microsserviços.

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

## 🔮 Funcionalidades

### Elasticsearch Integration (Implementado - Fase 3)
- ✅ Indexação de jogos para busca full-text
- ✅ Busca por similaridade
- ✅ Azure Function para sincronização automática

---

## 📋 Checklist da Fase 4

| Requisito | Status |
|-----------|--------|
| ✅ Dockerfiles otimizados (Alpine, non-root) | Implementado |
| ✅ Cluster Kubernetes (AKS) | Implementado |
| ✅ Deployments e Services | Implementado |
| ✅ HPA (Auto Scaling 1-5 pods, CPU 70%) | Implementado |
| ✅ Comunicação Assíncrona (Azure Service Bus) | Implementado |
| ✅ APM (Application Insights) | Implementado |

---

## 📄 Licença

FIAP - Pós-Graduação em Arquitetura de Software .NET

**Tech Challenge - Fase 4**