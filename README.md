#  Automóveis Vendas API — CP2

> Sistema de gerenciamento de concessionária de veículos com cadastro de clientes, carros, motos, vendas e pagamentos.

**Turma:** 2TDSPG

| Nome | RM |
|---|---|
| Arthur Brito | RM562085 |
| Felipe Flosi | RM563197 |
| Pedro Brum | RM561780 |

---

##  Arquitetura

```
automoveisVendasApi.sln
├── AutomoveisVendasApi.Domain         → Entidades
├── AutomoveisVendasApi.Application    → Interfaces de repositório
├── AutomoveisVendasApi.Infrastructure → DbContext, Mappings, Repositórios, Migrations
└── automoveisVendasApi                → API, DI, Endpoints
```





---

##  Como rodar o projeto

### 1. Entrar na pasta

```bash
cd cp2
```

### 2. Restaurar dependências

```bash
dotnet restore
```

### 3. Iniciar o banco

```bash
dotnet ef database update \
  --project AutomoveisVendasApi.Infrastructure \
  --startup-project automoveisVendasApi
```



### 4. Rodar a API

```bash
cd automoveisVendasApi
dotnet run
```

---

##  Acesso

| Interface | URL |
|---|---|
| Swagger UI | http://localhost:5000 |

---

##  Endpoints

| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/health` | Health check |
| `GET` | `/carros` | Lista todos os carros |
| `GET` | `/carros/disponiveis` | Lista carros não vendidos |
| `GET` | `/motos` | Lista todas as motos |
| `GET` | `/motos/disponiveis` | Lista motos não vendidas |
| `GET` | `/clientes` | Lista todos os clientes |
| `GET` | `/vendas` | Lista vendas com cliente, veículo e pagamentos |

---

##  Connection String

Arquivo: `automoveisVendasApi/appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=concessionaria.db"
}
```
