# CookieStore — Descrição do Projeto

## 1. Apresentação

O **CookieStore** é um sistema de controle de vendas e produção de cookies desenvolvido como
projeto de pós-graduação. O sistema foi construído com **ASP.NET Core 8** e modelado seguindo
os princípios do **Domain-Driven Design (DDD)**, com persistência em banco de dados relacional
via **Entity Framework Core**.

---

## 2. Problema a ser resolvido

Uma pequena confeitaria artesanal produz cookies em lotes e os vende no balcão ou por encomenda.
O negócio enfrenta três problemas práticos:

| Problema | Impacto |
|---|---|
| Preço de venda não é registrado no momento da transação | Mudanças posteriores no preço do cardápio distorcem relatórios históricos |
| Custo de produção por sabor não é rastreado | Impossível saber se um sabor é lucrativo |
| Não há visão consolidada de margem por período | Decisões de precificação são tomadas sem dados |

### O que o sistema resolve

- **Registrar vendas** com o preço capturado no momento exato da transação (snapshot imutável)
- **Registrar lotes de produção** com custo total e rendimento, calculando automaticamente o custo por unidade
- **Calcular margem de lucro por sabor** cruzando receita de vendas e custo de lotes em qualquer período
- **Gerenciar o cardápio** (sabores ativos, preços de tabela) sem afetar o histórico de vendas

---

## 3. Escopo do sistema

### Dentro do escopo

- Cadastro e gestão de sabores (nome, preço de tabela, ativo/inativo)
- Registro de vendas com snapshot do preço e quantidade
- Registro de lotes de produção com rendimento e custo total
- Relatório de lucro por sabor e resumo geral por período de datas
- API RESTful para todas as operações acima

### Fora do escopo

- Gestão de estoque em tempo real
- Controle de ingredientes ou receitas
- Emissão de notas fiscais ou integração com sistemas de pagamento
- Autenticação e autorização de usuários

---

## 4. Arquitetura e conceitos DDD aplicados

O projeto demonstra os seguintes conceitos de DDD:

### 4.1 Ubiquitous Language

Os nomes do domínio refletem o vocabulário da confeitaria acordado com os stakeholders:
`Sabor`, `Venda`, `Lote`, `Rendimento`, `PrecoTabela`. O código-fonte em inglês mapeia
diretamente esses conceitos: `Sabor → Sabor`, `PrecoTabela → CatalogPrice`, `Venda → Venda`.

### 4.2 Bounded Contexts

| Bounded Context | Tipo | Responsabilidade |
|---|---|---|
| **SharedKernel** | Shared Kernel | Define `Sabor` — o cardápio compartilhado por Vendas e Producao |
| **Vendas** | Core Domain | Registra e consulta vendas; calcula receita e margem |
| **Producao** | Supporting Domain | Registra lotes de produção; calcula custo por unidade |

### 4.3 Aggregates e Aggregate Roots

Cada BC possui um único Aggregate Root:

- **`Sabor`** — identidade própria, controla preço e status ativo
- **`Venda`** — criada exclusivamente via `VendaFactory`; preço unitário é snapshot imutável
- **`Lote`** — criado exclusivamente via `LoteFactory`; custo por unidade é valor calculado

Todos herdam de `Entity` (classe base abstrata), que fornece identidade (`Guid Id`) e
igualdade baseada em identidade.

### 4.4 Value Objects

Representam conceitos descritivos sem identidade própria, definidos pelo valor e imutáveis:

| Value Object | Invariante |
|---|---|
| `PrecoTabela` | Deve ser positivo |
| `Quantidade` | Deve ser positivo |
| `CustoTotal` | Deve ser positivo |
| `Rendimento` | Deve ser positivo (evita divisão por zero em `CostPerUnit`) |

Todos herdam de `ValueObject` (classe base abstrata com `GetEqualityComponents()`).

### 4.5 Domain Services

`RelatorioService` — justificado como Domain Service porque o cálculo de lucro por sabor
cruza os BCs Vendas e Producao. A lógica não pertence a nenhum dos dois aggregates
individualmente, pois exige dados de ambos.

### 4.6 Factories

| Factory | Justificativa |
|---|---|
| `VendaFactory` | Captura o snapshot do preço no momento da criação — invariante de negócio crítico |
| `LoteFactory` | Valida `Rendimento` e `CustoTotal` antes de instanciar o aggregate |

### 4.7 Repositories

Uma interface por Aggregate Root, definida no Domain, implementada na Infrastructure:

- `ISaborRepository` / `SaborRepository`
- `IVendaRepository` / `VendaRepository`
- `ILoteRepository` / `LoteRepository`

### 4.8 Context Map e Anti-Corruption Layer

- **Padrão Shared Kernel:** `Sabor` e `SaborId` são compartilhados entre os três BCs
- **Padrão Customer-Supplier:** Producao (upstream) fornece `SaborId` para Vendas (downstream)
- **Anti-Corruption Layer (ACL):** O BC Vendas não depende do aggregate `Sabor` diretamente.
  A interface `ISaborAcl` e o record `SaborSnapshot` protegem o BC Vendas de mudanças no
  modelo interno do `Sabor`. O `SaborAclAdapter` (na camada Application) realiza a tradução.

---

## 5. Stack tecnológica

| Componente | Tecnologia |
|---|---|
| Linguagem | C# (.NET 8) |
| ORM | Entity Framework Core |
| Banco de dados | SQL Server (configurável via connection string) |
| Testes unitários | xUnit + FluentAssertions + NSubstitute |
| Testes arquiteturais | NetArchTest.Rules |
| API | ASP.NET Core Web API |

---

## 6. Estrutura de camadas

```
CookieStore.Domain          ← Domínio puro (sem dependências externas)
    SharedKernel/           ← Entity, ValueObject, Sabor, ISaborRepository
    Vendas/                 ← Venda, VendaFactory, RelatorioService, ISaborAcl
    Producao/               ← Lote, LoteFactory, ILoteRepository

CookieStore.Application     ← Orquestração de casos de uso
    Sabores/                ← SaborAppService
    Vendas/                 ← VendaAppService, SaborAclAdapter
    Producao/               ← LoteAppService
    Relatorio/              ← RelatorioAppService

CookieStore.Infrastructure  ← Persistência EF Core
    Repositories/           ← SaborRepository, VendaRepository, LoteRepository
    Mapping/                ← Configurações IEntityTypeConfiguration

CookieStore.API             ← Endpoints HTTP (a implementar)
CookieStore.Tests           ← Testes unitários e arquiteturais
```
