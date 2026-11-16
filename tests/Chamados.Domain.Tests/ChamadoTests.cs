using System;
using System.Collections.Generic;
using System.Text;

using Chamados.Domain;

namespace Chamados.Domain.Tests;

public class ChamadoTests
{
    [Fact]
    public void ChamadoNovo_NasceComStatusAberto()
    {
        var chamado = new Chamado("Trocar lâmpada do corredor");

        Assert.Equal(StatusChamado.Aberto, chamado.Status);
    }

    [Fact]
    public void TituloEmBranco_LancaArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new Chamado(" "));
    }
}