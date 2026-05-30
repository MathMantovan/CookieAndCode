# CookieStore — Trabalho de Conclusão do modulo .NET

Sistema de controle de vendas e produção de cookies, desenvolvido com **C# .NET 8** e modelado seguindo os princípios de **Domain-Driven Design (DDD)**.

---

## Estrutura do projeto

```
src/
├── CookieStore.Domain/          ← Domínio puro (sem dependências externas)
│   ├── SharedKernel/            ← Entity, ValueObject, Sabor, ISaborRepository
│   ├── Vendas/                  ← Venda, VendaFactory, RelatorioService, ISaborAcl, SaborSnapshot
│   └── Producao/                ← Lote, LoteFactory, ILoteRepository
├── CookieStore.Application/     ← Orquestração de casos de uso
│   ├── Sabores/                 ← SaborAppService
│   ├── Vendas/                  ← VendaAppService, SaborAclAdapter
│   ├── Producao/                ← LoteAppService
│   └── Relatorio/               ← RelatorioAppService
├── CookieStore.Infrastructure/  ← Persistência EF Core (SQL Server)
│   ├── Repositories/
│   └── Mapping/
docs/
├── projeto.md                   ← Parte 1: descrição, escopo e problema
├── dominio.md                   ← Parte 1: diagramas Mermaid do domínio
└── oop-solid-grasp.md           ← Partes 2 e 3: OOP, SOLID e GRASP aplicados
src/CookieStore.Tests/           ← Parte 4: 136 testes (xUnit + NetArchTest)
```

---

## Parte 1 — Modelagem com Domain-Driven Design

> **Entrega:** Texto descrevendo o projeto e imagem da modelagem do domínio.

### Onde encontrar

| Documento | Conteúdo |
|---|---|
| [`docs/projeto.md`](docs/projeto.md) | Descrição do sistema, problema resolvido e escopo |
| [`docs/dominio.md`](docs/dominio.md) | Diagramas Mermaid: hierarquia de classes, Bounded Contexts, Context Map e fluxo das Factories |

### Requisitos cumpridos

**Ubiquitous Language, Entities, Value Objects e Repositories**

O vocabulário do negócio é refletido diretamente no código: `Sabor`, `Venda`, `Lote`, `Rendimento`, `PrecoTabela`. Entities têm identidade (`Guid Id` herdado de `Entity`); Value Objects são imutáveis e definidos pelo valor (`PrecoTabela`, `Quantidade`, `CustoTotal`, `Rendimento`). Cada Aggregate Root possui uma interface de Repository definida no Domain:

- `ISaborRepository` → `src/CookieStore.Domain/SharedKernel/ISaborRepository.cs`
- `IVendaRepository` → `src/CookieStore.Domain/Vendas/IVendaRepository.cs`
- `ILoteRepository` → `src/CookieStore.Domain/Producao/ILoteRepository.cs`

**Aggregates, Bounded Contexts e Domain Services**

| Bounded Context | Tipo | Aggregate Root |
|---|---|---|
| SharedKernel | Shared Kernel | `Sabor` |
| Vendas | Core Domain | `Venda` |
| Producao | Supporting Domain | `Lote` |

`RelatorioService` é o único Domain Service do projeto — justificado porque o cálculo de lucro cruza os BCs Vendas e Producao, e essa lógica não pertence a nenhum dos dois Aggregates individualmente.

**Domain Services versus Factories**

| Tipo | Classe | Justificativa |
|---|---|---|
| Domain Service | `RelatorioService` | Opera sobre dados de dois BCs distintos; não cria objetos |
| Factory | `VendaFactory` | Encapsula a criação de `Venda` com captura de snapshot do preço |
| Factory | `LoteFactory` | Valida `Rendimento` e `CustoTotal` antes de instanciar o Aggregate |

**Anti-Corruption Layer e Context Map**

O BC Vendas não depende diretamente do Aggregate `Sabor` (SharedKernel). A integração é feita via ACL:

- `ISaborAcl` — interface de tradução, definida no BC Vendas (`src/CookieStore.Domain/Vendas/ISaborAcl.cs`)
- `SaborSnapshot` — read model imutável do Sabor para o BC Vendas (`src/CookieStore.Domain/Vendas/SaborSnapshot.cs`)
- `SaborAclAdapter` — implementação do ACL na camada Application (`src/CookieStore.Application/Vendas/SaborAclAdapter.cs`)

O diagrama completo do Context Map está em [`docs/dominio.md`](docs/dominio.md).

---

## Parte 2 — Orientação a Objetos com C#

> **Entrega:** Código-fonte C# em conformidade com a modelagem DDD.

### Onde encontrar

[`docs/oop-solid-grasp.md`](docs/oop-solid-grasp.md) — seção **Parte 2**, com trechos de código destacados, objetivo de cada princípio e explicação do ponto de aplicação.

### Requisitos cumpridos

**Encapsulamento**

- Propriedades com `private set` em todos os Aggregates (`Sabor`, `Venda`, `Lote`) — estado mutável apenas via métodos nomeados (`UpdatePrice`, `Deactivate`)
- Construtores `private` nos Value Objects — criação obrigatória via `Create(...)` com validação
- Construtor `internal` em `Venda` — apenas `VendaFactory` (mesmo assembly) pode instanciar

Arquivo de referência: `src/CookieStore.Domain/SharedKernel/Sabor.cs`, `src/CookieStore.Domain/SharedKernel/PrecoTabela.cs`

**Abstração**

- `Entity` — classe `abstract` com contrato de identidade e igualdade
- `ValueObject` — classe `abstract` com método `abstract GetEqualityComponents()`
- Interfaces de Repository abstraem completamente a tecnologia de persistência

Arquivo de referência: `src/CookieStore.Domain/SharedKernel/Entity.cs`, `src/CookieStore.Domain/SharedKernel/ValueObject.cs`

**Herança**

- `Sabor`, `Venda` e `Lote` herdam de `Entity`
- `PrecoTabela`, `Quantidade`, `CustoTotal` e `Rendimento` herdam de `ValueObject`
- `SaborAclAdapter` implementa `ISaborAcl`

**Polimorfismo**

- Método `virtual Describe()` em `Entity`, sobrescrito com `override` em cada Aggregate Root
- Método `abstract GetEqualityComponents()` em `ValueObject`, implementado com `override` em cada Value Object
- Injeção de dependência via interfaces (`ISaborAcl`, `IVendaRepository`) — comportamento diferente em produção e nos testes, sem alteração do código consumidor

---

## Parte 3 — Padrões de Projeto: SOLID e GRASP

> **Entrega:** Implementação de dois ou mais princípios SOLID e GRASP, com destaque do trecho, nome, objetivo e explicação.

### Onde encontrar

[`docs/oop-solid-grasp.md`](docs/oop-solid-grasp.md) — seção **Parte 3**, com cada princípio apresentando:
- Nome e objetivo
- Trecho de código com o ponto de aplicação destacado em comentário
- Explicação do porquê aquele trecho aplica o princípio

### Princípios SOLID aplicados (todos os 5)

| Princípio | Onde se aplica | Arquivo |
|---|---|---|
| **S** — Single Responsibility | `RelatorioService` calcula relatórios e nada mais; `SaborAppService` orquestra casos de uso e nada mais | `RelatorioService.cs`, `SaborAppService.cs` |
| **O** — Open/Closed | `Entity.Describe()` é `virtual` — novos Aggregates estendem sem modificar a base | `Entity.cs` |
| **L** — Liskov Substitution | `SaborRepository` implementa `ISaborRepository` completamente; mocks substituem sem quebrar os testes | `SaborRepository.cs` |
| **I** — Interface Segregation | Três interfaces de Repository separadas, cada uma com apenas os métodos do seu Aggregate | `ISaborRepository.cs`, `IVendaRepository.cs`, `ILoteRepository.cs` |
| **D** — Dependency Inversion | `VendaAppService` depende de `ISaborAcl` e `IVendaRepository` (abstrações), nunca de implementações concretas | `VendaAppService.cs` |

### Padrões GRASP aplicados (4 padrões)

| Padrão | Onde se aplica | Arquivo |
|---|---|---|
| **Low Coupling** | `VendaFactory` recebe `SaborSnapshot` em vez de `Sabor` — elimina acoplamento entre BCs | `VendaFactory.cs` |
| **High Cohesion** | `Lote` calcula `CostPerUnit` internamente porque possui os dois operandos (`TotalCost` e `Yield`) | `Lote.cs` |
| **Information Expert** | `Venda` calcula `Total` porque possui `UnitPrice` e `Quantity`; `RelatorioService` é o expert cross-BC | `Venda.cs`, `RelatorioService.cs` |
| **Creator** | `VendaFactory` cria `Venda` porque detém o conhecimento de criação (snapshot + ID + timestamp) | `VendaFactory.cs`, `LoteFactory.cs` |

---

## Parte 4 — Testes Unitários e TDD

> **Entrega:** Cenários de teste, código-fonte dos testes e relatório de cobertura.

### Onde encontrar

- Testes: `src/CookieStore.Tests/`
- Cobertura: gerada com `dotnet test --collect:"XPlat Code Coverage"`

### Resultado dos testes

```
Aprovado! — Com falha: 0, Aprovado: 136, Ignorado: 0, Total: 136
```

### Cobertura do código de domínio

| Pacote | Cobertura de linhas | Cobertura de branches |
|---|---|---|
| `CookieStore.Domain` | **96%** | 76% |
| `CookieStore.Application` | **100%** | 100% |

Requisito mínimo: **> 80% do código de domínio**. ✅

### Princípios de testes unitários aplicados

| Princípio | Como está implementado |
|---|---|
| **Isolamento** | Testes de domínio não usam banco nem I/O; testes de Application usam mocks (NSubstitute) |
| **Repetibilidade** | Nenhum teste depende de estado externo ou ordem de execução |
| **Rapidez** | 136 testes executam em menos de 200 ms |
| **Auto-verificação** | FluentAssertions com mensagens descritivas; falha sem intervenção manual |
| **Abrangência** | Testes positivos, negativos e de arquitetura por camada |

### Arquivos de teste por classe testada

| Classe | Arquivo de teste |
|---|---|
| `Sabor` | `SharedKernel/SaborTests.cs` |
| `Entity` (base) | `SharedKernel/EntityTests.cs` |
| `ValueObject` (base) | `SharedKernel/ValueObjectTests.cs` |
| `PrecoTabela` | `SharedKernel/PrecoTabelaTests.cs` |
| `SaborId` | `SharedKernel/SaborIdTests.cs` |
| `DomainException` | `SharedKernel/DomainExceptionTests.cs` |
| `Venda` | `Vendas/VendaTests.cs` |
| `VendaFactory` | `Vendas/VendaFactoryTests.cs` |
| `Quantidade` | `Vendas/QuantidadeTests.cs` |
| `VendaId` | `Vendas/VendaIdTests.cs` |
| `RelatorioService` | `Vendas/RelatorioServiceTests.cs` |
| `VendaAppService` | `Vendas/VendaAppServiceTests.cs` |
| `SaborAclAdapter` | `Vendas/SaborAclAdapterTests.cs` |
| `Lote` | `Producao/LoteTests.cs` |
| `LoteFactory` | `Producao/LoteFactoryTests.cs` |
| `Rendimento` | `Producao/RendimentoTests.cs` |
| `CustoTotal` | `Producao/CustoTotalTests.cs` |
| `LoteId` | `Producao/LoteIdTests.cs` |
| `LoteAppService` | `Producao/LoteAppServiceTests.cs` |
| `RelatorioAppService` | `Relatorio/RelatorioAppServiceTests.cs` |
| Regras de arquitetura DDD | `Architecture/ArchitectureTests.cs` |

### Testes negativos (DomainException)

Todos os invariantes de domínio possuem cobertura negativa:

- `Sabor` com preço zero ou negativo → `DomainException`
- `Venda` com quantidade zero → `DomainException`
- `Lote` com rendimento zero → `DomainException` (evita divisão por zero)
- `Lote` com custo negativo → `DomainException`
- `PrecoTabela`, `Quantidade`, `CustoTotal`, `Rendimento` com valores inválidos → `DomainException`

### Testes de arquitetura (NetArchTest)

Validam automaticamente as fronteiras DDD a cada build:

1. `Domain` não referencia `Infrastructure`
2. `Domain` não referencia `Application`
3. `Application` não referencia `Infrastructure`

---

## Como executar

```bash
# Build
dotnet build

# Testes
dotnet test

# Testes com relatório de cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## Stack tecnológica

| Componente | Tecnologia |
|---|---|
| Linguagem | C# (.NET 8) |
| ORM | Entity Framework Core |
| Banco de dados | SQL Server |
| Testes unitários | xUnit + FluentAssertions + NSubstitute |
| Testes arquiteturais | NetArchTest.Rules |
| Cobertura | coverlet |
