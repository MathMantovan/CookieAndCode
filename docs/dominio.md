# CookieStore — Diagrama de Domínio

## 1. Hierarquia de classes (OOP)

```mermaid
classDiagram
    direction TB

    class Entity {
        <<abstract>>
        +Guid Id
        +virtual Describe() string
        +Equals(obj) bool
        +GetHashCode() int
        +operator ==(a, b) bool
        +operator !=(a, b) bool
    }

    class ValueObject {
        <<abstract>>
        #GetEqualityComponents()* IEnumerable~object~
        +Equals(obj) bool
        +GetHashCode() int
        +operator ==(a, b) bool
        +operator !=(a, b) bool
    }

    class Sabor {
        +string Name
        +PrecoTabela CatalogPrice
        +bool IsActive
        +SaborId SaborId
        +UpdatePrice(newPrice)
        +Deactivate()
        +override Describe() string
    }

    class Venda {
        +SaborId SaborId
        +Quantidade Quantity
        +decimal UnitPrice
        +DateTime SoldAt
        +decimal Total
        +VendaId VendaId
        +override Describe() string
    }

    class Lote {
        +SaborId SaborId
        +Rendimento Yield
        +CustoTotal TotalCost
        +DateTime ProducedAt
        +decimal CostPerUnit
        +LoteId LoteId
        +override Describe() string
    }

    class PrecoTabela {
        +decimal Value
        +Create(value)$  PrecoTabela
        #override GetEqualityComponents()
    }

    class Quantidade {
        +int Value
        +Create(value)$  Quantidade
        #override GetEqualityComponents()
    }

    class CustoTotal {
        +decimal Value
        +Create(value)$  CustoTotal
        #override GetEqualityComponents()
    }

    class Rendimento {
        +int Value
        +Create(value)$  Rendimento
        #override GetEqualityComponents()
    }

    Entity <|-- Sabor
    Entity <|-- Venda
    Entity <|-- Lote
    ValueObject <|-- PrecoTabela
    ValueObject <|-- Quantidade
    ValueObject <|-- CustoTotal
    ValueObject <|-- Rendimento

    Sabor *-- PrecoTabela
    Venda *-- Quantidade
    Lote  *-- Rendimento
    Lote  *-- CustoTotal
```

---

## 2. Bounded Contexts e Context Map

```mermaid
graph TB
    subgraph SK["SharedKernel (Shared Kernel)"]
        Sabor["Sabor (Aggregate Root)"]
        ISaborRepo["ISaborRepository"]
        PrecoTabela["PrecoTabela (VO)"]
        SaborId["SaborId (ID)"]
        EntityBase["Entity (abstract)"]
        VOBase["ValueObject (abstract)"]
    end

    subgraph V["Vendas BC (Core Domain)"]
        VendaAgg["Venda (Aggregate Root)"]
        VendaFactory["VendaFactory"]
        RelSvc["RelatorioService (Domain Service)"]
        ISaborAcl["ISaborAcl (ACL interface)"]
        SaborSnap["SaborSnapshot (ACL read model)"]
        IVendaRepo["IVendaRepository"]
        Quantidade["Quantidade (VO)"]
    end

    subgraph P["Producao BC (Supporting Domain)"]
        LoteAgg["Lote (Aggregate Root)"]
        LoteFactory["LoteFactory"]
        ILoteRepo["ILoteRepository"]
        Rendimento["Rendimento (VO)"]
        CustoTotal["CustoTotal (VO)"]
    end

    subgraph APP["Application Layer"]
        SaborSvc["SaborAppService"]
        VendaSvc["VendaAppService"]
        LoteSvc["LoteAppService"]
        RelAppSvc["RelatorioAppService"]
        AclAdapter["SaborAclAdapter (ACL impl)"]
    end

    subgraph INFRA["Infrastructure Layer"]
        SaborRepo["SaborRepository"]
        VendaRepo["VendaRepository"]
        LoteRepo["LoteRepository"]
        DbCtx["CookieDbContext"]
    end

    %% Shared Kernel usage
    Sabor --> SaborId
    Sabor --> PrecoTabela
    VendaAgg --> SaborId
    LoteAgg --> SaborId

    %% ACL boundary
    AclAdapter -->|"usa"| ISaborRepo
    AclAdapter -->|"produz"| SaborSnap
    AclAdapter -.->|"implementa"| ISaborAcl
    VendaFactory -->|"recebe"| SaborSnap
    VendaSvc -->|"injeta"| ISaborAcl

    %% RelatorioService crosses BCs
    RelSvc -->|"usa"| IVendaRepo
    RelSvc -->|"usa"| ILoteRepo

    %% Application wiring
    SaborSvc --> ISaborRepo
    LoteSvc --> ILoteRepo
    RelAppSvc --> RelSvc

    %% Infrastructure implements Domain interfaces
    SaborRepo -.->|"implementa"| ISaborRepo
    VendaRepo -.->|"implementa"| IVendaRepo
    LoteRepo -.->|"implementa"| ILoteRepo
    SaborRepo --> DbCtx
    VendaRepo --> DbCtx
    LoteRepo --> DbCtx
```

---

## 3. Factories e criação de Aggregates

```mermaid
sequenceDiagram
    participant App as VendaAppService
    participant ACL as ISaborAcl
    participant Adapter as SaborAclAdapter
    participant Repo as ISaborRepository
    participant Factory as VendaFactory
    participant VO as Quantidade.Create()
    participant Agg as Venda (internal ctor)

    App->>ACL: GetSnapshotAsync(saborId)
    ACL->>Adapter: GetSnapshotAsync(saborId)
    Adapter->>Repo: GetByIdAsync(saborId)
    Repo-->>Adapter: Sabor
    Adapter-->>ACL: SaborSnapshot(Id, CurrentPrice, IsActive)
    ACL-->>App: SaborSnapshot
    App->>Factory: Create(snapshot, quantity)
    Factory->>VO: Quantidade.Create(quantity)
    VO-->>Factory: Quantidade (validado)
    Factory->>Agg: new Venda(Guid.NewGuid(), snapshot.Id, qty, snapshot.CurrentPrice, UtcNow)
    Agg-->>Factory: Venda
    Factory-->>App: Venda
```

---

## 4. Testes e cobertura

| Projeto de teste | O que cobre | Testes |
|---|---|---|
| `CookieStore.Tests/SharedKernel` | Sabor, PrecoTabela, Entity, ValueObject | Unitários puros |
| `CookieStore.Tests/Vendas` | Venda, VendaFactory, RelatorioService, VendaAppService, SaborAclAdapter | Unitários + mocks |
| `CookieStore.Tests/Producao` | Lote, LoteFactory, LoteAppService | Unitários + mocks |
| `CookieStore.Tests/Relatorio` | RelatorioAppService | Unitários + mocks |
| `CookieStore.Tests/Architecture` | Regras de dependência entre camadas DDD | Arquiteturais (NetArchTest) |
