# CookieStore — Projeto de Pós-Graduação (DDD com C# .NET)

---

## PLANO DE AÇÃO — Conformidade com Requisitos do TCC

### Contexto

O projeto tem Domain, Application e Infrastructure implementados com 92 testes passando.
O repositório de referência `ubirajaramnj/CustomerManagement` foi usado para calibrar o
nível esperado pelo avaliador. As etapas abaixo devem ser executadas **em ordem** e cada
uma **obrigatoriamente** passa pelo pipeline TDDAgent → OOPAgent → SolidAgent.

### Etapa 1 — Entity + ValueObject + Conversão de VOs (Parte 2)

**Objetivo:** Criar hierarquia de herança com `abstract`, `virtual`, `override`.

- Criar `src/CookieStore.Domain/SharedKernel/Entity.cs`
  - `abstract class Entity` com `Guid Id`, `Equals`, `GetHashCode`, operadores `==`/`!=`
  - Método `virtual string Describe()` → sobrescrito em cada Aggregate Root
- Criar `src/CookieStore.Domain/SharedKernel/ValueObject.cs`
  - `abstract class ValueObject` com `abstract IEnumerable<object> GetEqualityComponents()`
  - `Equals`, `GetHashCode`, operadores `==`/`!=`
- Modificar `Sabor`, `Venda`, `Lote` → herdar de `Entity`
  - Propriedade calculada de ID tipado: `public SaborId SaborId => new(Id);`
  - `override string Describe()` com descrição específica
- Converter VOs de `record` para `sealed class : ValueObject`
  - `PrecoTabela`, `Quantidade`, `CustoTotal`, `Rendimento`
  - Private constructor + static `Create(...)` com validação
  - `override GetEqualityComponents()` em cada um
- Atualizar todos os testes que usam `new PrecoTabela(x)` → `PrecoTabela.Create(x)`

**Pipeline:** TDDAgent → OOPAgent → SolidAgent

### Etapa 2 — Anti-Corruption Layer (Parte 1)

**Objetivo:** Tornar explícito o padrão ACL no Context Map entre SharedKernel e BC Vendas.

- Criar `src/CookieStore.Domain/Vendas/SaborSnapshot.cs` — read model do Sabor para o BC Vendas
- Criar `src/CookieStore.Domain/Vendas/ISaborAcl.cs` — interface de tradução
- Criar `src/CookieStore.Application/Vendas/SaborAclAdapter.cs` — implementação do ACL
- Modificar `VendaAppService` → injetar `ISaborAcl`; `VendaFactory.Create` recebe `SaborSnapshot`

**Pipeline:** TDDAgent → OOPAgent → SolidAgent

### Etapa 3 — ArchitectureTests (Parte 4)

**Objetivo:** Validar as fronteiras DDD via NetArchTest (pacote já instalado).

- Criar `src/CookieStore.Tests/Architecture/ArchitectureTests.cs`
- 5 regras: Domain não referencia Infrastructure/Application; Application não referencia
  Infrastructure/API; API não referencia Domain diretamente.

**Pipeline:** TDDAgent → SolidAgent

### Etapa 4 — Documentação (Partes 1, 2 e 3)

**Objetivo:** Entregas textuais obrigatórias — sem comentários no código-fonte.

- `docs/projeto.md` — Descrição do sistema, escopo e problema resolvido
- `docs/dominio.md` — Diagrama Mermaid com Aggregates, BCs, Context Map, ACL
- `docs/oop-solid-grasp.md` — Mapeamento de cada princípio (OOP, SOLID, GRASP) apontando
  para o arquivo e trecho do código correspondente (sem comentários no .cs)

**Pipeline:** Sem agentes — apenas texto e diagrama Mermaid

### Verificação Final

```bash
dotnet build    # zero erros
dotnet test     # todos os testes passando (>= 92 + novos de arquitetura)
```

---

## REGRA GLOBAL — Idioma do Código

**Todo o código-fonte deve ser escrito em inglês**, sem exceção:
- Nomes de classes, métodos, propriedades, variáveis, campos e parâmetros → **inglês**
- Comentários XML (`///`) e comentários inline → **inglês**
- Mensagens de `DomainException` → **inglês**
- Nomes de arquivos `.cs` → **inglês**
- Conteúdo de DTOs, enums, constantes → **inglês**

A única exceção são os nomes de domínio que fazem parte da **Ubiquitous Language** já definida no CLAUDE.md (ex: `Sabor`, `Venda`, `Lote`, `Rendimento`) — esses permanecem em português pois representam o vocabulário do negócio acordado com os stakeholders.

---

## REGRA GLOBAL — Agentes Obrigatórios

**TODA implementação neste projeto deve obrigatoriamente passar pelos três agentes abaixo, nesta ordem:**

| Ordem | Agente | Quando invocar |
|---|---|---|
| 1 | **TDDAgent** | Antes de qualquer implementação — escreve os testes primeiro (Red phase) |
| 2 | **OOPAgent** | Durante a implementação — garante encapsulamento, abstração, herança e polimorfismo corretos |
| 3 | **SolidAgent** | Após a implementação — valida SOLID e GRASP; bloqueia se houver violação |

### Protocolo obrigatório por tarefa

Para **qualquer nova classe, interface, serviço, factory ou repositório**:

```
1. Invocar TDDAgent  → escrever testes que falham (Red)
2. Invocar OOPAgent  → implementar com OOP correto (Green)
3. Invocar SolidAgent → revisar design SOLID/GRASP (Refactor)
4. Só então considerar a tarefa concluída
```

**Nenhum código de produção pode ser entregue sem a aprovação dos três agentes.**
Os agentes são definidos em `.claude/agents/` e estão disponíveis via Agent tool.

---

## Visão geral do projeto

Sistema de controle de vendas de cookies, modelado com Domain-Driven Design (DDD).
Backend em C# (.NET), banco de dados PostgreSQL com Entity Framework Core.
Projeto acadêmico — a modelagem de domínio deve seguir rigorosamente os conceitos de DDD.

---

## Stack tecnológica

- **Linguagem**: C# (.NET 8 ou superior)
- **ORM**: Entity Framework Core
- **Banco**: PostgreSQL
- **Testes**: xUnit
- **Arquitetura**: DDD em camadas (Domain / Application / Infrastructure / API)

---

## Estrutura de pastas

```
src/
├── CookieStore.Domain/
│   ├── SharedKernel/        # Sabor (Aggregate Root compartilhado), ISaborRepository
│   ├── Vendas/              # Venda (Aggregate Root), IVendaRepository, VendaFactory, RelatorioService
│   └── Producao/            # Lote (Aggregate Root), ILoteRepository, LoteFactory
├── CookieStore.Application/
│   ├── Vendas/              # VendaAppService
│   ├── Producao/            # LoteAppService
│   └── Sabores/             # SaborAppService
├── CookieStore.Infrastructure/
│   ├── CookieDbContext.cs
│   └── Repositories/        # Implementações EF Core dos Repositories
├── CookieStore.API/
│   └── Controllers/         # SaboresController, VendasController, LotesController, RelatorioController
└── CookieStore.Tests/       # Projeto de testes unificado (xUnit + FluentAssertions + NSubstitute + NetArchTest)
    ├── SharedKernel/        # SaborTests, PrecoTabelaTests, DomainExceptionTests
    ├── Vendas/              # VendaTests, VendaFactoryTests, QuantidadeTests, VendaAppServiceTests
    ├── Producao/            # LoteTests, LoteFactoryTests, RendimentoTests, LoteAppServiceTests
    ├── Relatorio/           # RelatorioServiceTests, RelatorioAppServiceTests
    └── Architecture/        # ArchitectureTests.cs — NetArchTest layer dependency rules
```

---

## Modelo de domínio

### Bounded Contexts

| BC | Tipo | Aggregates |
|---|---|---|
| SharedKernel | Shared Kernel | `Sabor` |
| Vendas | Core Domain | `Venda` |
| Producao | Supporting Domain | `Lote` |

### Aggregates e seus membros

**Sabor** (Shared Kernel — Aggregate Root)
- Campos: `SaborId`, `Nome`, `PrecoTabela` (VO), `Ativo`
- Regras: preço sempre positivo; desativar não exclui

**Venda** (BC Vendas — Aggregate Root)
- Campos: `VendaId`, `SaborId`, `Quantidade` (VO), `PrecoUnitario` (snapshot), `VendidoEm`
- Regra crítica: `PrecoUnitario` é sempre um snapshot do preço no momento da venda — nunca buscado dinamicamente
- Criada exclusivamente via `VendaFactory`

**Lote** (BC Producao — Aggregate Root)
- Campos: `LoteId`, `SaborId`, `Rendimento` (VO), `CustoTotal` (VO), `ProduzidoEm`
- Regra crítica: `Rendimento` deve ser positivo (evita divisão por zero em `CustoPorUnidade`)
- `CustoPorUnidade` é calculado (`CustoTotal / Rendimento`) — nunca persiste no banco
- Criado exclusivamente via `LoteFactory`

### Value Objects

Todos os Value Objects são **imutáveis** e usam `record` em C#:
- `PrecoTabela(decimal Valor)` — deve ser positivo
- `Quantidade(int Valor)` — deve ser positivo
- `CustoTotal(decimal Valor)` — deve ser positivo
- `Rendimento(int Valor)` — deve ser positivo (nunca zero)

### Domain Services

- `RelatorioService` — único serviço que cruza os dois BCs; recebe `IVendaRepository` e `ILoteRepository`
  - `LucroPorSaborAsync(DateTime de, DateTime ate)` → `IEnumerable<LucroPorSabor>`
  - `ResumoAsync(DateTime de, DateTime ate)` → `ResumoVendas`

### Application Services

Orquestram casos de uso. Não contêm lógica de negócio — delegam ao domínio.
Recebem DTOs como entrada e retornam DTOs ou primitivos como saída.

**`SaborAppService`** (`CookieStore.Application/Sabores/`)
- `CriarAsync(string nome, decimal preco)` → `SaborId`
- `AtualizarPrecoAsync(SaborId id, decimal novoPreco)` → `void`
- `DesativarAsync(SaborId id)` → `void`
- `ListarAtivosAsync()` → `IEnumerable<SaborDto>`

**`VendaAppService`** (`CookieStore.Application/Vendas/`)
- `RegistrarAsync(SaborId saborId, int quantidade)` → `VendaId`
- `ListarPorPeriodoAsync(DateTime de, DateTime ate)` → `IEnumerable<VendaDto>`

**`LoteAppService`** (`CookieStore.Application/Producao/`)
- `RegistrarAsync(SaborId saborId, int rendimento, decimal custoTotal)` → `LoteId`
- `ListarPorPeriodoAsync(DateTime de, DateTime ate)` → `IEnumerable<LoteDto>`

### Factories

- `VendaFactory.Criar(Sabor sabor, int quantidade)` — captura snapshot do preço
- `LoteFactory.Criar(SaborId id, int rendimento, decimal custoTotal)` — valida invariantes antes de instanciar

### Repositories (interfaces no domínio)

- `ISaborRepository`: `ObterPorIdAsync`, `ListarAtivosAsync`, `AdicionarAsync`, `AtualizarAsync`
- `IVendaRepository`: `ListarPorPeriodoAsync`, `ListarPorSaborAsync`, `AdicionarAsync`
- `ILoteRepository`: `ListarPorPeriodoAsync`, `ListarPorSaborAsync`, `AdicionarAsync`

---

## Regras de implementação

### Obrigatórias (DDD)

- **Nunca** expor setters públicos em Entities ou Aggregates — toda mudança via método
- **Nunca** injetar `DbContext` diretamente no Domain — apenas nas implementações de Repository
- **Nunca** colocar lógica de negócio em Controllers ou Application Services
- Toda criação de Aggregate Root deve passar pela Factory correspondente
- Value Objects usam `record` — nunca `class` mutável
- Lançar `DomainException` (custom) para violações de invariantes — nunca retornar null silenciosamente
- Repositories: interface no Domain, implementação apenas na Infrastructure

### Nomenclatura

- IDs fortemente tipados: `SaborId`, `VendaId`, `LoteId` (records wrapping `Guid`)
- Exceções do domínio: `DomainException` em `CookieStore.Domain`
- Application Services terminam em `AppService` (ex: `VendaAppService`)
- Domain Services terminam em `Service` dentro da pasta do BC (ex: `RelatorioService`)

### Estilo de código

- Indentação: 4 espaços
- `async/await` em todos os métodos que tocam repositório
- Prefixo `_` para campos privados (ex: `_vendaRepository`)
- Usar `IEnumerable` para listas somente-leitura; `List` apenas internamente nos Aggregates

---

## TDD — Test-Driven Development

### Princípios

- Escrever o teste **antes** da implementação (Red → Green → Refactor)
- Cada teste cobre **uma única regra de negócio ou invariante**
- Testes de domínio são **puros** — sem banco, sem I/O, sem mocks desnecessários
- Testes de Application Services usam **mocks** dos Repositories (NSubstitute)
- Testes de arquitetura usam **NetArchTest** para garantir que as camadas não violem as dependências do DDD

### Pacotes de teste

```xml
<!-- CookieStore.Domain.Tests e CookieStore.Application.Tests -->
<PackageReference Include="xunit" Version="2.*" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.*" />
<PackageReference Include="FluentAssertions" Version="6.*" />
<PackageReference Include="NSubstitute" Version="5.*" />

<!-- CookieStore.Architecture.Tests -->
<PackageReference Include="NetArchTest.Rules" Version="1.*" />
```

### Convenção de nomenclatura dos testes

```
NomeDoSujeito_Cenario_ResultadoEsperado

Exemplos:
  Venda_ComQuantidadeZero_DeveLancarDomainException
  VendaFactory_AoCriar_DeveUsarSnapshotDoPreco
  RelatorioService_SemVendasNoPeriodo_DeveRetornarReceitaZero
  Lote_ComRendimentoZero_DeveLancarDomainException
```

### Cobertura mínima esperada por camada

| Projeto de teste | O que cobre | Tipo de teste |
|---|---|---|
| `CookieStore.Domain.Tests` | Invariantes de Aggregates, VOs e Factories | Unitário puro (sem mock) |
| `CookieStore.Application.Tests` | Fluxo dos App Services e RelatorioService | Unitário com mock de Repository |
| `CookieStore.Architecture.Tests` | Regras de dependência entre camadas DDD | Arquitetural (estático) |

### Casos de teste obrigatórios por classe

**`SaborTests`**
- Criar sabor com preço válido → deve persistir
- Criar sabor com preço zero ou negativo → `DomainException`
- Atualizar preço para valor positivo → deve atualizar
- Atualizar preço para zero → `DomainException`
- Desativar sabor ativo → `Ativo` deve ser `false`

**`VendaTests` + `VendaFactoryTests`**
- Criar venda com quantidade positiva → deve persistir
- Criar venda com quantidade zero → `DomainException`
- Factory deve copiar `PrecoTabela` do Sabor como snapshot
- Snapshot deve ser preservado mesmo após mudança de preço do Sabor

**`LoteTests` + `LoteFactoryTests`**
- Criar lote com rendimento e custo positivos → deve persistir
- Criar lote com rendimento zero → `DomainException` (divisão por zero)
- Criar lote com custo negativo → `DomainException`
- `CustoPorUnidade` deve ser calculado corretamente

**`RelatorioServiceTests`**
- Período com vendas e lotes → deve calcular margem corretamente
- Período sem vendas → `Receita` deve ser zero
- Período sem lotes → `Custo` deve ser zero
- Múltiplos sabores → deve agrupar por `SaborId` corretamente

**`ArchitectureTests`**
- `Domain` não deve referenciar `Infrastructure`
- `Domain` não deve referenciar `Application`
- `Application` não deve referenciar `Infrastructure`
- `Application` não deve referenciar `API`
- `API` não deve referenciar `Domain` diretamente (apenas via `Application`)

### Exemplo de teste de domínio

```csharp
public class VendaFactoryTests
{
    [Fact]
    public void Criar_DeveUsarSnapshotDoPrecoAtual()
    {
        var sabor = new Sabor("Chocolate", new PrecoTabela(10.00m));
        var factory = new VendaFactory();

        var venda = factory.Criar(sabor, quantidade: 3);

        venda.PrecoUnitario.Should().Be(10.00m);
    }

    [Fact]
    public void Criar_ComQuantidadeZero_DeveLancarDomainException()
    {
        var sabor = new Sabor("Chocolate", new PrecoTabela(10.00m));
        var factory = new VendaFactory();

        var act = () => factory.Criar(sabor, quantidade: 0);

        act.Should().Throw<DomainException>();
    }
}
```

### Exemplo de teste de Application Service

```csharp
public class VendaAppServiceTests
{
    private readonly ISaborRepository _sabores = Substitute.For<ISaborRepository>();
    private readonly IVendaRepository _vendas = Substitute.For<IVendaRepository>();
    private readonly VendaFactory _factory = new();
    private readonly VendaAppService _sut;

    public VendaAppServiceTests()
        => _sut = new VendaAppService(_sabores, _vendas, _factory);

    [Fact]
    public async Task RegistrarAsync_SaborAtivo_DevePersistirVenda()
    {
        var saborId = new SaborId(Guid.NewGuid());
        var sabor = new Sabor("Morango", new PrecoTabela(8.00m));
        _sabores.ObterPorIdAsync(saborId).Returns(sabor);

        await _sut.RegistrarAsync(saborId, quantidade: 2);

        await _vendas.Received(1).AdicionarAsync(Arg.Any<Venda>());
    }
}
```

### Comandos de teste

```bash
# Rodar todos os testes
dotnet test

# Rodar apenas testes de domínio
dotnet test tests/CookieStore.Domain.Tests

# Rodar com cobertura (requer coverlet)
dotnet test --collect:"XPlat Code Coverage"

# Rodar testes arquiteturais
dotnet test tests/CookieStore.Architecture.Tests
```

---

## Comandos úteis

```bash
# Restaurar dependências
dotnet restore

# Build
dotnet build

# Rodar a API
dotnet run --project src/CookieStore.API

# Testes
dotnet test

# Criar migration (rodar da raiz do projeto)
dotnet ef migrations add <NomeDaMigration> --project src/CookieStore.Infrastructure --startup-project src/CookieStore.API

# Aplicar migrations
dotnet ef database update --project src/CookieStore.Infrastructure --startup-project src/CookieStore.API
```

---

## Contexto acadêmico

Este projeto deve demonstrar explicitamente os seguintes conceitos de DDD:

1. **Ubiquitous Language** — todos os nomes de classes, métodos e variáveis refletem o vocabulário acima
2. **Entities e Value Objects** — distinção clara: Entities têm identidade, VOs são imutáveis e sem identidade
3. **Aggregates** — `Sabor`, `Venda` e `Lote` são Aggregate Roots; nenhum outro objeto externo referencia membros internos diretamente
4. **Repositories** — uma interface por Aggregate Root; implementações isoladas na Infrastructure
5. **Bounded Contexts** — SharedKernel, Vendas e Producao com fronteiras explícitas
6. **Domain Services** — `RelatorioService` justificado: lógica que cruza dois Aggregates não pertence a nenhum dos dois
7. **Factories** — `VendaFactory` e `LoteFactory` justificadas: encapsulam criação complexa e garantem invariantes iniciais
8. **Context Map** — integração via Shared Kernel (`Sabor`/`SaborId`); padrão Customer-Supplier entre Producao (upstream) e Vendas (downstream)
9. **TDD** — testes escritos antes da implementação; testes de domínio puros; mocks apenas na camada de Application

Ao gerar código, sempre justificar brevemente em comentário XML (`///`) qual conceito DDD está sendo aplicado quando não for óbvio.

Ao gerar testes, seguir sempre o ciclo Red → Green → Refactor: primeiro o teste falhando, depois a implementação mínima, depois o refinamento.
