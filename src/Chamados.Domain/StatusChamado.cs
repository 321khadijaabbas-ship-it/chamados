namespace Chamados.Domain;

/// <summary>
/// Situação do chamado no ciclo de vida.
/// Resolvido e Cancelado são estados finais.
/// </summary>
public enum StatusChamado
{
    Aberto,
    EmAtendimento,
    Resolvido,
    Cancelado
}
