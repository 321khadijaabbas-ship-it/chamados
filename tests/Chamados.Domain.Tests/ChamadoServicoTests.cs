using System;
using System.Collections.Generic;
using System.Text;

namespace Chamados.Domain.Tests;

public class ChamadoServicoTests
{
    private static readonly DateTimeOffset Inicio = new(2026, 1, 1, 8, 0, 0, TimeSpan.Zero);

    private static (ChamadoServico servico, RelogioFixo relogio) Criar()
    {
        var relogio = new RelogioFixo(Inicio);
        var repo = new RepositorioChamadosEmMemoria(relogio);
        return (new ChamadoServico(repo, relogio), relogio);
    }

    [Fact]
    public async Task Listar_OrdenaPorPrioridadeDepoisPorAbertura()
    {
        var (s, r) = Criar();
        await s.AbrirAsync(new NovoChamado("A", Prioridade.Baixa));
        r.Avancar(TimeSpan.FromMinutes(1));
        await s.AbrirAsync(new NovoChamado("B", Prioridade.Alta));
        r.Avancar(TimeSpan.FromMinutes(1));
        await s.AbrirAsync(new NovoChamado("C", Prioridade.Alta));

        var lista = await s.ListarAsync(FiltroChamados.Todos);

        Assert.Equal(["B", "C", "A"], lista.Select(x => x.Titulo));
    }

    [Fact]
    public async Task Listar_FiltraTextoNaDescricaoSemDiferenciarMaiusculas()
    {
        var (s, _) = Criar();
        await s.AbrirAsync(new NovoChamado("Ar", Prioridade.Alta, "Sala 204"));
        await s.AbrirAsync(new NovoChamado("Luz"));

        var lista = await s.ListarAsync(new FiltroChamados(Texto: "SALA"));

        Assert.Single(lista);
        Assert.Equal("Ar", lista[0].Titulo);
    }

    [Fact]
    public async Task Listar_CombinaStatusEPrioridadeMinima()
    {
        var (s, _) = Criar();
        var a = await s.AbrirAsync(new NovoChamado("A", Prioridade.Alta));
        var b = await s.AbrirAsync(new NovoChamado("B", Prioridade.Baixa));
        await s.AbrirAsync(new NovoChamado("C", Prioridade.Alta));
        await s.IniciarAtendimentoAsync(a.Id, "Equipe");
        await s.IniciarAtendimentoAsync(b.Id, "Equipe");

        var lista = await s.ListarAsync(
            new FiltroChamados(StatusChamado.EmAtendimento, Prioridade.Alta));

        Assert.Single(lista);
        Assert.Equal("A", lista[0].Titulo);
    }

    [Fact]
    public async Task Estatisticas_CalculaTempoMedioDeResolucao()
    {
        var (s, r) = Criar();
        var a = await s.AbrirAsync(new NovoChamado("A"));
        var b = await s.AbrirAsync(new NovoChamado("B"));
        await s.IniciarAtendimentoAsync(a.Id, "X");
        await s.IniciarAtendimentoAsync(b.Id, "X");
        r.Avancar(TimeSpan.FromHours(1));
        await s.ResolverAsync(a.Id, "ok");
        r.Avancar(TimeSpan.FromHours(2));
        await s.ResolverAsync(b.Id, "ok");

        var est = await s.ObterEstatisticasAsync();

        Assert.Equal(TimeSpan.FromHours(2), est.TempoMedioResolucao);
    }

    [Fact]
    public async Task Resolver_IdInexistente_LancaChamadoNaoEncontrado()
    {
        var (s, _) = Criar();

        await Assert.ThrowsAsync<ChamadoNaoEncontradoException>(
            () => s.ResolverAsync(Guid.NewGuid(), "ok"));
    }

    [Fact]
    public async Task Listar_TokenJaCancelado_LancaOperationCanceled()
    {
        var (s, _) = Criar();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => s.ListarAsync(FiltroChamados.Todos, cts.Token));
    }
}