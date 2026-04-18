# Automóveis Vendas API — CP2

## Integrantes

| Nome | RM |

| Arthur Brito | RM562085 |

| Felipe Flosi | RM563197 |

| Pedro Brum | RM561780 |

---

## Domínio

Sistema de gerenciamento de uma concessionária de veículos com cadastro de clientes, carros, motos, vendas e pagamentos.

## SGBD

**SQLite** — sem instalação de servidor, o arquivo `concessionaria.db` é criado automaticamente na pasta `automoveisVendasApi/`.

---

## Arquitetura

```
c--concessionaria-api/
├── AutomoveisVendasApi.Domain          → Entidades de domínio
├── AutomoveisVendasApi.Application     → Interfaces de repositório + DTOs
├── AutomoveisVendasApi.Infrastructure  → DbContext, Mappings (Fluent API), Repositórios, Migrations
└── automoveisVendasApi                 → API — Program.cs, DI, Endpoints, Swagger
```

---

## Diagrama de Classes

```
┌─────────────────────┐          ┌──────────────────────────┐
│       Cliente        │          │          Carro            │
├─────────────────────┤          ├──────────────────────────┤
│ + ClienteId : int   │          │ + CarroId  : int          │
│ + Nome      : string│          │ + Modelo   : string       │
│ + Email     : string│          │ + Marca    : string       │
│ + Telefone  : string│          │ + Ano      : int          │
├─────────────────────┤          │ + Valor    : decimal      │
│ + Cadastrar()       │          │ + Placa    : string       │
└────────┬────────────┘          │ + Vendido  : bool         │
         │ 1                     └─────────────┬─────────────┘
         │                                     │ 1
         │ N                                   │ 0..N
         ▼                                     ▼
┌─────────────────────────────────────────────────────────┐
│                         Venda                            │
├─────────────────────────────────────────────────────────┤
│ + VendaId    : int                                       │
│ + ClienteId  : int       [FK obrigatória]                │
│ + CarroId    : int?      [FK opcional]                   │
│ + MotoId     : int?      [FK opcional]                   │
│ + DataVenda  : DateTime                                  │
│ + ValorTotal : decimal                                   │
│ + Status     : string                                    │
├─────────────────────────────────────────────────────────┤
│ + CriarVendaClienteCarro()                               │
│ + CriarVendaClienteMoto()                                │
└───────────────────────┬─────────────────────────────────┘
         ▲              │ 1
         │ 0..N         │ N
         │              ▼
┌────────┴────────┐    ┌──────────────────────────┐
│      Moto       │    │        Pagamento           │
├─────────────────┤    ├──────────────────────────┤
│ + MotoId : int  │    │ + PagamentoId  : int      │
│ + Modelo : str  │    │ + VendaId      : int [FK] │
│ + Marca  : str  │    │ + Tipo         : string   │
│ + Ano    : int  │    │ + Valor        : decimal  │
│ + Valor  : dec  │    │ + DataPagamento : DateTime│
│ + Vendida: bool │    ├──────────────────────────┤
└─────────────────┘    │ + CriarPagamento()        │
                       └──────────────────────────┘
```

**Regras de negócio do modelo:**
- Uma `Venda` pertence obrigatoriamente a um `Cliente`
- Uma `Venda` está associada a **um** `Carro` **ou** uma `Moto` (mutuamente exclusivos — nunca os dois)
- Uma `Venda` pode ter **N** `Pagamentos` (ex: entrada + parcelas)
- `Email` do cliente é único no banco
- `Placa` do carro é única no banco

---

## Como rodar

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- EF Core CLI:

```bash
dotnet tool install --global dotnet-ef
```

---

### Passo 1 — Restaurar pacotes

```bash
dotnet restore
```

---

### Passo 2 — Gerar a migration

```bash
dotnet ef migrations add InitialCreate --project AutomoveisVendasApi.Infrastructure --startup-project automoveisVendasApi
```

---

### Passo 3 — Iniciar o banco

```bash
dotnet ef database update --project AutomoveisVendasApi.Infrastructure --startup-project automoveisVendasApi
```

O arquivo `concessionaria.db` será iniciado em `automoveisVendasApi/`.

---

### Passo 4 — Rodar a API

```bash
cd automoveisVendasApi
dotnet run
```

Acesse o Swagger em: **http://localhost:5000/swagger**

---

## Endpoints

| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/health` | Health check |
| GET | `/carros` | Lista todos os carros |
| GET | `/carros/disponiveis` | Lista carros não vendidos |
| GET | `/motos` | Lista todas as motos |
| GET | `/motos/disponiveis` | Lista motos não vendidas |
| GET | `/clientes` | Lista todos os clientes |
| GET | `/vendas` | Lista vendas com cliente, veículo e pagamentos |

---

## Mapeamento EF Core (Fluent API)

| Entidade | Destaques |
|----------|-----------|
| Cliente | Email único (`HasIndex(...).IsUnique()`) |
| Carro | Placa única; FK opcional em Venda; auto-increment |
| Moto | FK opcional em Venda; auto-increment |
| Venda | FK obrigatória → Cliente; FK opcional → Carro ou Moto; índices nas FKs |
| Pagamento | FK obrigatória → Venda; cascade delete; auto-increment |

---

## Connection string

Configurada em `automoveisVendasApi/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=concessionaria.db"
  }
}
```
