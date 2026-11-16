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