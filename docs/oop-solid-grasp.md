# CookieStore — Princípios OOP, SOLID e GRASP aplicados

Este documento mapeia cada princípio aplicado no projeto ao trecho de código correspondente.
Os trechos destacados em **negrito** identificam o ponto exato de aplicação.

---

## Parte 2 — Orientação a Objetos (OOP)

### OOP 1 — Abstração

> **Objetivo:** Ocultar detalhes de implementação e expor apenas contratos essenciais.

**Trecho 1 — `Entity` (classe abstrata)**
Arquivo: `src/CookieStore.Domain/SharedKernel/Entity.cs`

```csharp
public abstract class Entity          // ← ABSTRAÇÃO: não pode ser instanciada diretamente
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id) { Id = id; }

    public virtual string Describe() => $"{GetType().Name} [Id={Id}]";  // ← contrato extensível
    // ...
}
```

A palavra-chave **`abstract`** impede que `Entity` seja instanciada diretamente.
Apenas subclasses concretas (`Sabor`, `Venda`, `Lote`) são criadas — o detalhe de como a
identidade é gerenciada e como a igualdade funciona fica oculto dos chamadores.

---

**Trecho 2 — `ValueObject` (classe abstrata)**
Arquivo: `src/CookieStore.Domain/SharedKernel/ValueObject.cs`

```csharp
public abstract class ValueObject     // ← ABSTRAÇÃO: contrato de igualdade estrutural
{
    protected abstract IEnumerable<object> GetEqualityComponents();  // ← contrato obrigatório
    // ...
}
```

O método **`abstract GetEqualityComponents()`** declara o contrato sem implementá-lo.
Cada Value Object decide quais componentes definem sua igualdade — os chamadores usam
`Equals` e `==` sem conhecer os campos internos.

---

**Trecho 3 — Interfaces de Repository**
Arquivo: `src/CookieStore.Domain/SharedKernel/ISaborRepository.cs`

```csharp
public interface ISaborRepository
{
    Task<Sabor?> GetByIdAsync(SaborId id);
    Task<IEnumerable<Sabor>> GetAllActiveAsync();
    Task AddAsync(Sabor sabor);
    Task UpdateAsync(Sabor sabor);
}
```

A **interface** `ISaborRepository` abstrai completamente a tecnologia de persistência (EF Core,
SQL Server). O Domain e o Application nunca sabem que existe um banco de dados.

---

### OOP 2 — Encapsulamento

> **Objetivo:** Proteger o estado interno e expor apenas o que é necessário.

**Trecho 1 — Private setters nos Aggregates**
Arquivo: `src/CookieStore.Domain/SharedKernel/Sabor.cs`

```csharp
public class Sabor : Entity
{
    public string Name { get; private set; }           // ← ENCAPSULAMENTO: setter privado
    public PrecoTabela CatalogPrice { get; private set; } // ← setter privado
    public bool IsActive { get; private set; }         // ← setter privado

    public void UpdatePrice(PrecoTabela newPrice)      // ← mutação apenas via método
    {
        CatalogPrice = newPrice;
    }

    public void Deactivate()                           // ← mutação apenas via método
    {
        IsActive = false;
    }
}
```

Nenhuma propriedade tem setter público. O estado do `Sabor` só muda via métodos de domínio
nomeados, garantindo que as invariantes sejam sempre verificadas.

---

**Trecho 2 — Private constructor nos Value Objects**
Arquivo: `src/CookieStore.Domain/SharedKernel/PrecoTabela.cs`

```csharp
public sealed class PrecoTabela : ValueObject
{
    public decimal Value { get; }                      // ← sem setter — imutável

    private PrecoTabela(decimal value) => Value = value; // ← ENCAPSULAMENTO: ctor privado

    public static PrecoTabela Create(decimal value)   // ← único ponto de criação
    {
        if (value <= 0) throw new DomainException("Catalog price must be positive.");
        return new PrecoTabela(value);
    }
}
```

O construtor privado garante que `PrecoTabela` só pode ser criado via `Create()`, onde a
validação é obrigatória. É impossível construir um `PrecoTabela` inválido.

---

**Trecho 3 — Internal constructor no Aggregate Root `Venda`**
Arquivo: `src/CookieStore.Domain/Vendas/Venda.cs`

```csharp
internal Venda(Guid id, SaborId saborId, Quantidade quantity, decimal unitPrice, DateTime soldAt)
    : base(id)
// ← ENCAPSULAMENTO: internal — apenas VendaFactory (mesmo assembly) pode criar Venda
```

O modificador **`internal`** restringe a criação de `Venda` à `VendaFactory`, que está no
mesmo assembly (`CookieStore.Domain`). Código da camada Application não pode usar `new Venda(...)`.

---

### OOP 3 — Herança

> **Objetivo:** Criar hierarquias de classes que compartilham comportamento comum.

**Trecho 1 — Aggregates herdam de `Entity`**

```csharp
// src/CookieStore.Domain/SharedKernel/Sabor.cs
public class Sabor : Entity { ... }

// src/CookieStore.Domain/Vendas/Venda.cs
public class Venda : Entity { ... }

// src/CookieStore.Domain/Producao/Lote.cs
public class Lote : Entity { ... }
```

Os três Aggregate Roots **herdam** de `Entity`, recebendo gratuitamente:
- `Guid Id` com `protected set`
- `Equals()` baseado em identidade
- `GetHashCode()` e operadores `==`/`!=`

---

**Trecho 2 — Value Objects herdam de `ValueObject`**

```csharp
// src/CookieStore.Domain/SharedKernel/PrecoTabela.cs
public sealed class PrecoTabela : ValueObject { ... }

// src/CookieStore.Domain/Vendas/Quantidade.cs
public sealed class Quantidade : ValueObject { ... }

// src/CookieStore.Domain/Producao/CustoTotal.cs
public sealed class CustoTotal : ValueObject { ... }

// src/CookieStore.Domain/Producao/Rendimento.cs
public sealed class Rendimento : ValueObject { ... }
```

Os quatro Value Objects **herdam** de `ValueObject`, recebendo a implementação de `Equals`,
`GetHashCode` e os operadores de comparação sem duplicação de código.

---

**Trecho 3 — `SaborAclAdapter` implementa `ISaborAcl`**
Arquivo: `src/CookieStore.Application/Vendas/SaborAclAdapter.cs`

```csharp
public class SaborAclAdapter : ISaborAcl  // ← HERANÇA via interface
{
    public async Task<SaborSnapshot?> GetSnapshotAsync(SaborId id) { ... }
}
```

---

### OOP 4 — Polimorfismo

> **Objetivo:** Tratar objetos de tipos diferentes de forma uniforme através de uma interface comum.

**Trecho 1 — `Describe()` virtual com override**
Arquivo: `src/CookieStore.Domain/SharedKernel/Entity.cs` e subclasses

```csharp
// Entity — método virtual (comportamento padrão)
public virtual string Describe() => $"{GetType().Name} [Id={Id}]";

// Sabor — override com comportamento específico
public override string Describe() => $"Sabor [{Name}] Id={Id}";

// Venda — override com comportamento específico
public override string Describe() => $"Venda [SaborId={SaborId}] SoldAt={SoldAt:yyyy-MM-dd}";

// Lote — override com comportamento específico
public override string Describe() => $"Lote [SaborId={SaborId}] ProducedAt={ProducedAt:yyyy-MM-dd}";
```

A palavra-chave **`virtual`** em `Entity` e **`override`** nas subclasses demonstram polimorfismo
por sobrescrita. O mesmo método `Describe()` produz resultados diferentes dependendo do tipo
concreto do objeto em tempo de execução.

---

**Trecho 2 — `GetEqualityComponents()` abstract com override**
Arquivo: `src/CookieStore.Domain/SharedKernel/ValueObject.cs` e VOs

```csharp
// ValueObject — método abstrato (POLIMORFISMO: força override em cada subclasse)
protected abstract IEnumerable<object> GetEqualityComponents();

// PrecoTabela — override que define a igualdade pelo Value decimal
protected override IEnumerable<object> GetEqualityComponents()
{
    yield return Value;
}

// Quantidade — override que define a igualdade pelo Value int
protected override IEnumerable<object> GetEqualityComponents()
{
    yield return Value;
}
```

O método **`abstract`** em `ValueObject` força cada subclasse a declarar `override`.
O método `Equals` da classe base chama `GetEqualityComponents()` — que em tempo de execução
despacha para a versão correta de cada VO. Isso é **polimorfismo via dispatch virtual**.

---

**Trecho 3 — Polimorfismo via interface (DI)**
Arquivo: `src/CookieStore.Application/Vendas/VendaAppService.cs`

```csharp
public class VendaAppService
{
    private readonly ISaborAcl _acl;        // ← POLIMORFISMO: referência à interface
    private readonly IVendaRepository _vendas;

    // Em produção: SaborAclAdapter + EfVendaRepository
    // Em testes: NSubstitute.For<ISaborAcl>() + NSubstitute.For<IVendaRepository>()
    // O VendaAppService não sabe qual implementação recebe
}
```

---

## Parte 3 — Princípios SOLID

### S — Single Responsibility Principle (SRP)

> **Objetivo:** Cada classe deve ter exatamente uma razão para mudar.

**Aplicação 1 — `RelatorioService`**
Arquivo: `src/CookieStore.Domain/Vendas/RelatorioService.cs`

A classe tem **uma única responsabilidade**: calcular lucro e resumo de vendas por período.
Não persiste, não valida entrada HTTP, não mapeia DTOs.

```csharp
public class RelatorioService : IRelatorioService
{
    // Única responsabilidade: calcular relatórios cruzando Vendas e Producao
    public async Task<IEnumerable<LucroPorSabor>> GetProfitBySaborAsync(DateTime from, DateTime to) { ... }
    public async Task<ResumoVendas> GetSummaryAsync(DateTime from, DateTime to) { ... }
}
```

**Aplicação 2 — `SaborAppService`**
Arquivo: `src/CookieStore.Application/Sabores/SaborAppService.cs`

A classe tem **uma única responsabilidade**: orquestrar os casos de uso do `Sabor`.
Não contém lógica de negócio — delega ao domínio.

---

### O — Open/Closed Principle (OCP)

> **Objetivo:** Classes abertas para extensão, fechadas para modificação.

**Aplicação — Novos sabores sem modificar o domínio**
Arquivo: `src/CookieStore.Domain/SharedKernel/Entity.cs`

```csharp
public abstract class Entity
{
    public virtual string Describe() => $"{GetType().Name} [Id={Id}]";  // ← ABERTA para extensão
}
```

Adicionar um novo Aggregate Root (ex: `Encomenda`) requer apenas **criar uma nova subclasse**
de `Entity` com `override Describe()`. A classe `Entity` nunca precisa ser modificada.

O mesmo vale para `ValueObject`: novos VOs são criados herdando da base — sem alterar a base.

---

### L — Liskov Substitution Principle (LSP)

> **Objetivo:** Implementações devem ser substituíveis por seus contratos sem quebrar o sistema.

**Aplicação — Repositories**
Arquivo: `src/CookieStore.Infrastructure/Repositories/SaborRepository.cs`

```csharp
public class SaborRepository : ISaborRepository
{
    public async Task<Sabor?> GetByIdAsync(SaborId id) => ...;    // implementa contrato
    public async Task<IEnumerable<Sabor>> GetAllActiveAsync() => ...; // implementa contrato
    public async Task AddAsync(Sabor sabor) { ... }               // implementa contrato
    public async Task UpdateAsync(Sabor sabor) { ... }            // implementa contrato
}
```

`SaborRepository` implementa **todos** os métodos de `ISaborRepository` sem exceções ou
comportamentos inesperados. Em testes, `NSubstitute.For<ISaborRepository>()` substitui
`SaborRepository` sem que nenhum código de produção perceba a diferença.

---

### I — Interface Segregation Principle (ISP)

> **Objetivo:** Interfaces não devem forçar implementadores a depender de métodos que não usam.

**Aplicação — Três interfaces separadas de Repository**

```csharp
// ISaborRepository — apenas operações de Sabor
public interface ISaborRepository
{
    Task<Sabor?> GetByIdAsync(SaborId id);
    Task<IEnumerable<Sabor>> GetAllActiveAsync();
    Task AddAsync(Sabor sabor);
    Task UpdateAsync(Sabor sabor);
}

// IVendaRepository — apenas operações de Venda (sem UpdateAsync — Venda é imutável)
public interface IVendaRepository
{
    Task<IEnumerable<Venda>> GetByPeriodAsync(DateTime from, DateTime to);
    Task<IEnumerable<Venda>> GetBySaborAsync(SaborId saborId);
    Task AddAsync(Venda venda);
}

// ILoteRepository — apenas operações de Lote (sem UpdateAsync — Lote é imutável)
public interface ILoteRepository
{
    Task<IEnumerable<Lote>> GetByPeriodAsync(DateTime from, DateTime to);
    Task<IEnumerable<Lote>> GetBySaborAsync(SaborId saborId);
    Task AddAsync(Lote lote);
}
```

Cada interface contém **apenas os métodos necessários** para o aggregate correspondente.
Nenhuma implementação é forçada a implementar métodos que não fazem sentido para ela.

---

### D — Dependency Inversion Principle (DIP)

> **Objetivo:** Módulos de alto nível devem depender de abstrações, não de concreções.

**Aplicação 1 — `VendaAppService` depende de abstrações**
Arquivo: `src/CookieStore.Application/Vendas/VendaAppService.cs`

```csharp
public class VendaAppService
{
    private readonly ISaborAcl _acl;            // ← abstração (DIP)
    private readonly IVendaRepository _vendas;  // ← abstração (DIP)
    private readonly VendaFactory _factory;     // ← objeto de domínio puro (sem infra)

    public VendaAppService(ISaborAcl acl, IVendaRepository vendas, VendaFactory factory)
    {
        _acl = acl;
        _vendas = vendas;
        _factory = factory;
    }
}
```

`VendaAppService` **nunca referencia** `SaborRepository`, `VendaRepository` ou qualquer
classe do EF Core. Isso permite trocar a persistência sem tocar na camada Application.

**Aplicação 2 — `RelatorioAppService` depende de `IRelatorioService`**
Arquivo: `src/CookieStore.Application/Relatorio/RelatorioAppService.cs`

```csharp
public class RelatorioAppService
{
    private readonly IRelatorioService _relatorio;  // ← abstração (DIP)
    // nunca depende de RelatorioService diretamente
}
```

---

## Parte 3 — Padrões GRASP

### GRASP 1 — Low Coupling (Baixo Acoplamento)

> **Objetivo:** Minimizar as dependências entre classes para reduzir o impacto de mudanças.

**Aplicação — `VendaFactory` desacoplada de `Sabor`**
Arquivo: `src/CookieStore.Domain/Vendas/VendaFactory.cs`

```csharp
public class VendaFactory
{
    public Venda Create(SaborSnapshot snapshot, int quantity)  // ← recebe SaborSnapshot, não Sabor
    {
        var qty = Quantidade.Create(quantity);
        return new Venda(Guid.NewGuid(), snapshot.Id, qty, snapshot.CurrentPrice, DateTime.UtcNow);
    }
}
```

Antes da implementação do ACL, `VendaFactory` dependia de `Sabor` (SharedKernel).
Agora depende apenas de `SaborSnapshot` (Vendas BC) — o acoplamento entre BCs foi eliminado.

---

### GRASP 2 — High Cohesion (Alta Coesão)

> **Objetivo:** Cada classe agrupa apenas responsabilidades fortemente relacionadas.

**Aplicação — `Lote` calcula `CostPerUnit` internamente**
Arquivo: `src/CookieStore.Domain/Producao/Lote.cs`

```csharp
public class Lote : Entity
{
    public Rendimento Yield { get; private set; }
    public CustoTotal TotalCost { get; private set; }

    public decimal CostPerUnit => TotalCost.Value / Yield.Value;  // ← calculado internamente
}
```

`CostPerUnit` é calculado por `Lote` porque `Lote` possui os dois operandos (`TotalCost` e
`Yield`). Mover esse cálculo para fora quebraria a coesão — um serviço externo precisaria
conhecer a estrutura interna do `Lote`.

---

### GRASP 3 — Information Expert

> **Objetivo:** Atribuir responsabilidades à classe que possui as informações necessárias.

**Aplicação 1 — `Venda` calcula `Total`**
Arquivo: `src/CookieStore.Domain/Vendas/Venda.cs`

```csharp
public decimal Total => UnitPrice * Quantity.Value;  // ← Venda tem ambos os operandos
```

`Venda` é a especialista em informação para calcular o total, pois possui `UnitPrice` e
`Quantity`. Nenhuma outra classe precisa conhecer esses detalhes para obter o total.

**Aplicação 2 — `RelatorioService` como especialista cross-BC**
Arquivo: `src/CookieStore.Domain/Vendas/RelatorioService.cs`

```csharp
public class RelatorioService : IRelatorioService
{
    private readonly IVendaRepository _vendaRepository;  // acesso a receitas
    private readonly ILoteRepository _loteRepository;    // acesso a custos
    // ← única classe com acesso a ambos os BCs — especialista legítima para calcular lucro
}
```

---

### GRASP 4 — Creator

> **Objetivo:** Atribuir a criação de objetos à classe que os agrega, contém ou usa de perto.

**Aplicação — `VendaFactory` cria `Venda`**
Arquivo: `src/CookieStore.Domain/Vendas/VendaFactory.cs`

```csharp
public class VendaFactory
{
    public Venda Create(SaborSnapshot snapshot, int quantity)  // ← CREATOR: cria Venda
    {
        var qty = Quantidade.Create(quantity);
        return new Venda(Guid.NewGuid(), snapshot.Id, qty, snapshot.CurrentPrice, DateTime.UtcNow);
    }
}
```

`VendaFactory` é a Creator de `Venda` porque:
1. Tem o conhecimento de criação (captura de snapshot do preço)
2. O construtor de `Venda` é `internal` — só a factory pode instanciá-la
3. Encapsula a lógica de criação complexa (geração de ID, timestamp, snapshot)

O mesmo padrão se aplica a `LoteFactory` para `Lote`.

---

## Resumo de cobertura

| Requisito | Princípio | Arquivo principal |
|---|---|---|
| Encapsulamento | OOP | `Sabor.cs`, `PrecoTabela.cs`, `Venda.cs` |
| Abstração | OOP | `Entity.cs`, `ValueObject.cs`, `ISaborRepository.cs` |
| Herança | OOP | `Sabor/Venda/Lote : Entity`, `PrecoTabela/.../Rendimento : ValueObject` |
| Polimorfismo | OOP | `Describe()` virtual/override, `GetEqualityComponents()` abstract/override |
| SRP | SOLID-S | `RelatorioService.cs`, `SaborAppService.cs` |
| OCP | SOLID-O | `Entity.cs` (`virtual Describe()`), `ValueObject.cs` |
| LSP | SOLID-L | `SaborRepository.cs`, `VendaRepository.cs`, `LoteRepository.cs` |
| ISP | SOLID-I | `ISaborRepository.cs`, `IVendaRepository.cs`, `ILoteRepository.cs` |
| DIP | SOLID-D | `VendaAppService.cs`, `RelatorioAppService.cs` |
| Low Coupling | GRASP | `VendaFactory.cs` (usa `SaborSnapshot`, não `Sabor`) |
| High Cohesion | GRASP | `Lote.cs` (`CostPerUnit`), `Venda.cs` (`Total`) |
| Information Expert | GRASP | `Venda.cs` (`Total`), `RelatorioService.cs` |
| Creator | GRASP | `VendaFactory.cs`, `LoteFactory.cs` |
