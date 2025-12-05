# FGC Games API

Microsserviço de Jogos da plataforma FIAP Cloud Games (FCG).

## 📋 Descrição

Este microsserviço é responsável pelo gerenciamento de jogos na plataforma FCG.

## 🏗️ Arquitetura

```
FGC.Games.Domain/          → Entidades, Enums, Eventos, Interfaces
FGC.Games.Application/     → DTOs, Use Cases
FGC.Games.Infrastructure/  → Repositórios, DbContext, Configurações
FGC.Games.Presentation/    → Controllers, Models, Program.cs
```

## 🚀 Endpoints

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/games` | Listar todos os jogos | - |
| GET | `/api/games/{id}` | Buscar jogo por ID | - |
| GET | `/api/games/search` | Pesquisar jogos | - |
| POST | `/api/games` | Criar novo jogo | Admin |
| PUT | `/api/games/{id}/price` | Atualizar preço | Admin |
| PUT | `/api/games/{id}/deactivate` | Desativar jogo | Admin |
| PUT | `/api/games/{id}/activate` | Ativar jogo | Admin |

## 🎮 Categorias de Jogos

| Valor | Categoria |
|-------|-----------|
| 0 | Action |
| 1 | Adventure |
| 2 | RPG |
| 3 | Strategy |
| 4 | Sports |
| 5 | Racing |
| 6 | Simulation |
| 7 | Puzzle |
| 8 | Horror |
| 9 | FPS |
| 10 | MMORPG |
| 11 | Indie |
| 12 | Fighting |
| 13 | Platformer |
| 14 | Sandbox |

## 🔧 Configuração Local

### Pré-requisitos
- .NET 8.0 SDK
- SQL Server (local ou Azure)

### Executar
```bash
cd FGC.Games.Presentation
dotnet restore
dotnet run
```

### Migrations
```bash
dotnet ef migrations add InitialCreate -p FGC.Games.Infrastructure -s FGC.Games.Presentation
dotnet ef database update -p FGC.Games.Infrastructure -s FGC.Games.Presentation
```

## 🐳 Docker

```bash
docker build -t fgc-games-api .
docker run -p 8080:8080 fgc-games-api
```

## 📦 Variáveis de Ambiente

| Variável | Descrição |
|----------|-----------|
| `ConnectionStrings__DefaultConnection` | Connection string do SQL Server |
| `Jwt__SecretKey` | Chave secreta do JWT (min 32 chars) |
| `Jwt__Issuer` | Emissor do token |
| `Jwt__Audience` | Audiência do token |
| `Jwt__ExpireMinutes` | Tempo de expiração em minutos |

## 🔗 Integração

Este microsserviço se comunica com:
- **FGC Users API** → Validação de tokens JWT
- **FGC Payments API** → Referência de jogos em pagamentos

## 📄 Licença

FIAP - Pós-Graduação em Arquitetura de Software .NET