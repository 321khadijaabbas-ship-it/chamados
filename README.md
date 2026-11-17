# Chamados

Sistema de chamados (ordens de serviço) em .NET.

## Requisitos

- .NET SDK 10.0.115 (fixado no global.json)

## Como executar

    dotnet build
    dotnet run --project src/Chamados.Web

## Como testar

    dotnet test

## Decisões

- **.NET 10 LTS como alvo:** versão de suporte de longo prazo, mantida até 14/11/2028, o que cobre todo o curso.
- **Formato .slnx:** o SDK 10 gera a solução em .slnx (XML curto, sem GUIDs) em vez do .sln antigo.
- ## Decisões

### Módulo 2

- **Interface de repositório (`IChamadoRepositorio`).** O armazenamento
  atual é em memória e será substituído pelo Entity Framework Core no
  Módulo 7. A interface permite trocar a implementação sem alterar o
  serviço, e os testes usam a versão em memória sem precisar de banco.

- **`TimeProvider` injetado.** O serviço e o repositório recebem o relógio
  pelo construtor em vez de chamar `DateTimeOffset.UtcNow`. Em produção
  usa-se `TimeProvider.System`; nos testes, um `RelogioFixo`, o que torna
  datas e durações previsíveis.

- **Casos de uso no Domain, sem camada Application.** Com poucos casos de
  uso e um único modelo de dados, uma camada Application só adicionaria
  indireção. A decisão será revista no Módulo 7.