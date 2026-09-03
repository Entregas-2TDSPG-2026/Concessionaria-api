# Automóveis Vendas API — CP2 · CP3 · CP4

## Integrantes

| Nome | RM |
|---|---|
| Arthur Brito | RM562085 |
| Felipe Flosi | RM563197 |
| Pedro Brum | RM561780 |

---

## Domínio

Sistema de gerenciamento de uma concessionária de veículos: cadastro de clientes, carros e motos,
registro de vendas (cliente + carro OU cliente + moto) e pagamentos associados a cada venda.

## SGBD

**SQLite** — sem instalação de servidor; o arquivo `concessionaria.db` é criado/atualizado
automaticamente (via `Database.Migrate()`) na pasta `automoveisVendasApi/` ao subir a API.

---

## Arquitetura (Clean Architecture)



A API **não** injeta `DbContext` diretamente nos controllers: todo acesso a dados passa pelo
repositório genérico `IRepository<T>` (Application/Infrastructure) ou por repositórios específicos
de agregado (`ICarroRepository`, `IVendaRepository`, etc.) quando há consultas além do CRUD mínimo.

---

## Como executar a API

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

### Passo 1 — Restaurar pacotes

```bash
dotnet restore
```

### Passo 2 — Aplicar as migrations (opcional — a API também aplica automaticamente no startup)

```bash
dotnet ef database update --project AutomoveisVendasApi.Infrastructure --startup-project automoveisVendasApi
```

### Passo 3 — Rodar a API

```bash
cd automoveisVendasApi
dotnet run
```

- **Swagger UI:** `http://localhost:5000/swagger`
- **Health check:** `http://localhost:5000/health`

---

## Endpoints principais

| Recurso | Método | Rota | Descrição |
|---|---|---|---|
| Health | GET | `/health` | Relatório de saúde (self + banco) |
| Clientes | GET | `/api/clientes` | Lista clientes |
| Clientes | GET | `/api/clientes/{id}` | Busca cliente por id |
| Clientes | POST | `/api/clientes` | Cadastra cliente |
| Carros | GET | `/api/carros` | Lista carros |
| Carros | GET | `/api/carros/disponiveis` | Lista carros não vendidos |
| Carros | GET | `/api/carros/{id}` | Busca carro por id |
| Carros | POST | `/api/carros` | Cadastra carro |
| Motos | GET | `/api/motos` | Lista motos |
| Motos | GET | `/api/motos/disponiveis` | Lista motos não vendidas |
| Motos | GET | `/api/motos/{id}` | Busca moto por id |
| Motos | POST | `/api/motos` | Cadastra moto |
| Vendas | GET | `/api/vendas` | Lista vendas com detalhes |
| Vendas | GET | `/api/vendas/{id}` | Busca venda por id |
| Vendas | POST | `/api/vendas` | Registra venda (carro OU moto) |
| Pagamentos | GET | `/api/pagamentos/venda/{vendaId}` | Lista pagamentos de uma venda |
| Pagamentos | POST | `/api/pagamentos` | Registra pagamento de uma venda |

Todos os endpoints estão documentados no Swagger, com comentários XML e os tipos de resposta
(`ProducesResponseType`) para sucesso e para os erros mais comuns.

---

## Repositório genérico

- **Contrato:** `IRepository<T>` (`AutomoveisVendasApi.Application/Interfaces/IRepository.cs`) —
  `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync`.
- **Implementação:** `Repository<T>` (`AutomoveisVendasApi.Infrastructure/Repositories/Repository.cs`),
  usando `DbContext`/`Set<T>()`.
- **Registro na DI:** `services.AddScoped(typeof(IRepository<>), typeof(Repository<>));` (`Program.cs`).
- **Uso demonstrado:**
  - `ClientesController` usa `IRepository<Cliente>` diretamente para todo o CRUD exposto.
  - `PagamentosController` usa `IRepository<Venda>` para validar a existência da venda.
  - `VendaService` (Application) usa `IRepository<Cliente>`, `IRepository<Carro>`, `IRepository<Moto>`
    e `IRepository<Venda>` para orquestrar a criação de uma venda.
  - `CarrosController`, `MotosController` e `VendasController` usam repositórios **específicos**
    (`ICarroRepository`, `IMotoRepository`, `IVendaRepository`) porque precisam de consultas além do
    CRUD mínimo (`GetDisponiveisAsync`, `GetByPlacaAsync`, `GetWithDetailsAsync`). Esses repositórios
    específicos herdam de `Repository<T>` e, portanto, também implementam `IRepository<T>`.

---

## Tratamento global de exceções (GlobalExceptionHandler)

Implementado em `automoveisVendasApi/Exceptions/GlobalExceptionHandler.cs` (`IExceptionHandler`),
registrado no `Program.cs` com `AddExceptionHandler<GlobalExceptionHandler>()` + `AddProblemDetails()`
e `app.UseExceptionHandler()` (antes de `MapControllers`/`MapApplicationHealthChecks`).

Toda exceção não tratada é logada (`ILogger`, nível `Error`, com o `TraceIdentifier` da requisição) e
convertida em uma resposta `application/problem+json` (RFC 7807). Em produção, `Detail` traz uma
mensagem genérica — o detalhe completo fica apenas no log.

| Exceção | Status HTTP |
|---|---|
| `ResourceNotFoundException` (Domain) | 404 Not Found |
| `ConflictException` (Domain) | 409 Conflict |
| `DomainException` (regra de negócio genérica) | 400 Bad Request |
| `ArgumentException` | 400 Bad Request |
| `KeyNotFoundException` | 404 Not Found |
| Qualquer outra exceção | 500 Internal Server Error (mensagem genérica fora de Development) |

---

## Health Checks (`GET /health`)

Único endpoint de health check, registrado em `automoveisVendasApi/Extensions/HealthCheckExtensions.cs`.

**Checks registrados:**
1. `self` — `SelfHealthCheck` (`IHealthCheck` customizado), confirma que o processo está no ar.
2. `database` — `AddDbContextCheck<ApplicationDbContext>`, alinhado ao SQLite/EF Core do CP2.

**Resposta JSON** com status geral, duração total e o resultado individual de cada check
(nome, status, duração e, apenas em Development, a mensagem da exceção).

**Status HTTP:** `Healthy`/`Degraded` → 200 (ainda serve tráfego); `Unhealthy` → 503.

Exemplo (API e banco OK):

```json
{
  "status": "Healthy",
  "totalDurationMs": 12.4,
  "checks": [
    { "name": "self", "status": "Healthy", "durationMs": 0.02, "description": "A API está em execução.", "error": null },
    { "name": "database", "status": "Healthy", "durationMs": 11.9, "description": null, "error": null }
  ]
}
```

Para simular falha do banco em ambiente local, altere temporariamente a `ConnectionStrings:DefaultConnection`
em `automoveisVendasApi/appsettings.Development.json` para um caminho inválido (NÃO commitar essa alteração)
e reinicie a API — `/health` deve retornar HTTP 503 com o check `database` como `Unhealthy`.

---

## Observabilidade (logs)

- `ILogger<T>` nativo do ASP.NET Core, sem dependência de biblioteca externa.
- Fluxo de escrita instrumentado: `POST /api/vendas` (`VendasController`) loga início e sucesso da
  operação com propriedades nomeadas e o `HttpContext.TraceIdentifier` da requisição, permitindo
  correlacionar as duas linhas de log com a mesma requisição. Os controllers de Clientes, Carros,
  Motos e Pagamentos também logam o sucesso da criação com o mesmo padrão.
- O `GlobalExceptionHandler` loga toda exceção em nível `Error`, incluindo o mesmo `TraceIdentifier`.

Exemplo de log de um `POST /api/vendas` bem-sucedido:
---

## Testes automatizados (xUnit)

Dois projetos de teste na solution:

- **`AutomoveisVendasApi.Domain.Tests`** — referencia **somente** `Domain`. Testa as regras de
  negócio de `Venda` (`Venda.CriarVendaClienteCarro`, `Venda.CriarVendaClienteMoto`, `Venda.Finalizar`)
  sem nenhum mock, no padrão AAA, com `[Fact]` (caminho feliz) e `[Theory]`/`[InlineData]` (caminhos
  de erro que lançam `DomainException`).
- **`AutomoveisVendasApi.Application.Tests`** — referencia `Application`, usa **Moq** para mockar
  `IRepository<Cliente>`, `IRepository<Carro>`, `IRepository<Moto>` e `IRepository<Venda>` e testa
  `VendaService`. Cobre cenários de dependência ausente/erro de negócio (cliente inexistente, carro
  inexistente, carro já vendido, carro e moto informados juntos) verificando que `AddAsync` **nunca**
  é chamado (`Times.Never`), além do caminho feliz, que verifica persistência única (`Times.Once`).

### Rodando os testes

```bash
dotnet test
```

Todos os testes devem passar (`dotnet test` a partir da raiz da solution).

---

## Mapeamento EF Core (Fluent API) — herdado do CP2

| Entidade | Destaques |
|----------|-----------|
| Cliente | Email único (`HasIndex(...).IsUnique()`) |
| Carro | Placa única; FK opcional em Venda; auto-increment |
| Moto | FK opcional em Venda; auto-increment |
| Venda | FK obrigatória → Cliente; FK opcional → Carro ou Moto; índices nas FKs |
| Pagamento | FK obrigatória → Venda; cascade delete; auto-increment |

## Connection string

Configurada em `automoveisVendasApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=concessionaria.db"
  }
}
```

---

## Checklist de entrega (CP3 + CP4)

- [x] Controllers + DTOs de request/response (nenhuma entidade de domínio exposta diretamente)
- [x] `IRepository<T>` + `Repository<T>` registrados na DI e usados em Controllers e em `VendaService`
- [x] Swagger com `SwaggerDoc`, `IncludeXmlComments` e `ProducesResponseType` em todas as actions
- [x] `GlobalExceptionHandler` (`IExceptionHandler`) + `ProblemDetails` (`application/problem+json`)
- [x] `GET /health` com checks `self` e `database`, JSON detalhado, 200/503 coerentes
- [x] Logs estruturados com `TraceIdentifier` no fluxo de criação de venda e no `GlobalExceptionHandler`
- [x] `AutomoveisVendasApi.Domain.Tests` (Fact + Theory, sem mock, sem referenciar Infrastructure/API)
- [x] `AutomoveisVendasApi.Application.Tests` (mock de repositório, `Times.Never`/`Times.Once`)
- [x] Migrations, DbContext e seed do CP2 mantidos intactos
