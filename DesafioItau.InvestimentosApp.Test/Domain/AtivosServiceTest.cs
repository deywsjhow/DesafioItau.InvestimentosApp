using Xunit;
using Moq;
using System.Threading.Tasks;
using DesafioItau.InvestimentosApp.Domain.Ativos;
using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;

public class AtivosServiceTests
{
    private readonly Mock<IAtivosContext> _mockAtivosContext = new();
    private readonly Mock<ICotacoesContext> _mockCotacoesContext = new();
    private readonly AtivosService _service;

    public AtivosServiceTests()
    {
        _service = new AtivosService(_mockAtivosContext.Object, _mockCotacoesContext.Object);
    }

    [Fact]
    public async Task GetAtivo_ReturnsSuccess_WhenDataIsValid()
    {
        // Arrange
        var codigo = "ITSA4";
        var ativo = new RetornoAtivosBD { codigo = "ITSA4" };
        var cotacao = new RetornoCotacoesBD { id_ativo = 1, preco_unitario = 10.5m, data_hora = DateTime.Now };

        _mockAtivosContext.Setup(a => a.GetAtivo(codigo)).ReturnsAsync(ativo);
        _mockCotacoesContext.Setup(c => c.GetCotacao(ativo.id)).ReturnsAsync(cotacao);

        // Act
        var result = await _service.GetAtivo(codigo);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(codigo, result.Data?.Ativo);
        Assert.Equal(10.5m, result.Data?.Preco);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetAtivo_ReturnsFail_WhenCodigoIsNullOrEmpty(string codigo)
    {
        // Act
        var result = await _service.GetAtivo(codigo);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Código do ativo não informado.", result.ErrorMessage);
    }

    [Fact]
    public async Task GetAtivo_ReturnsFail_WhenAtivoNotFound()
    {
        // Arrange
        _mockAtivosContext.Setup(a => a.GetAtivo("PETR4")).ReturnsAsync((RetornoAtivosBD?)null);

        // Act
        var result = await _service.GetAtivo("PETR4");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Ativo não encontrado.", result.ErrorMessage);
    }

    [Fact]
    public async Task GetAtivo_ReturnsFail_WhenCotacaoNotFound()
    {
        // Arrange
        var ativo = new RetornoAtivosBD { id = 10 };
        _mockAtivosContext.Setup(a => a.GetAtivo("VALE3")).ReturnsAsync(ativo);
        _mockCotacoesContext.Setup(c => c.GetCotacao(10)).ReturnsAsync((RetornoCotacoesBD?)null);

        // Act
        var result = await _service.GetAtivo("VALE3");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Nenhuma cotação encontrada para o ativo informado.", result.ErrorMessage);
    }

    [Fact]
    public async Task GetAtivo_CallsContextsOnce()
    {
        // Arrange
        var codigo = "ABEV3";
        var ativo = new RetornoAtivosBD { id = 20 };
        var cotacao = new RetornoCotacoesBD { id_ativo = 20, preco_unitario = 15m, data_hora = DateTime.Now };

        _mockAtivosContext.Setup(a => a.GetAtivo(codigo)).ReturnsAsync(ativo);
        _mockCotacoesContext.Setup(c => c.GetCotacao(20)).ReturnsAsync(cotacao);

        // Act
        await _service.GetAtivo(codigo);

        // Assert
        _mockAtivosContext.Verify(a => a.GetAtivo(codigo), Times.Once);
        _mockCotacoesContext.Verify(c => c.GetCotacao(20), Times.Once);
    }
}
