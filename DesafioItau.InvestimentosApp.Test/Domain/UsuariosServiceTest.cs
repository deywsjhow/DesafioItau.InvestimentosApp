
using Xunit;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using DesafioItau.InvestimentosApp.Domain.Usuarios;
using DesafioItau.InvestimentosApp.Repository.DbUsuariosContext;

public class UsuariosServiceTest
{
    private readonly Mock<IUsuariosContext> _mockUsuariosContext = new();
    private readonly UsuariosService _service;

    public UsuariosServiceTest()
    {
        _service = new UsuariosService(_mockUsuariosContext.Object);
    }

    #region TestesPrecoMedio
    [Fact]
    public async Task GetPrecoMedioAsync_ReturnsSuccess_WhenDataIsValid()
    {
        var response = new UsuarioPrecoMedioResponse { NomUser = "João", NomAtivo = "PETR4", PrecoMedio = 20.5m };
        _mockUsuariosContext.Setup(x => x.GetPrecoMedioAsync(1, "PETR4")).ReturnsAsync(response);

        var result = await _service.GetPrecoMedioAsync(1, "PETR4");

        Assert.True(result.Success);
        Assert.Equal("João", result.Data?.NomUser);
    }

    [Theory]
    [InlineData(0, "PETR4")]
    [InlineData(1, "")]
    [InlineData(-5, null)]
    public async Task GetPrecoMedioAsync_ReturnsFail_WhenInputInvalid(int usuarioId, string ativoId)
    {
        var result = await _service.GetPrecoMedioAsync(usuarioId, ativoId);

        Assert.False(result.Success);
        Assert.Equal("ID do usuário ou código do ativo inválido.", result.ErrorMessage);
    }

    [Fact]
    public async Task GetPrecoMedioAsync_ReturnsFail_WhenNoDataFound()
    {
        _mockUsuariosContext.Setup(x => x.GetPrecoMedioAsync(1, "ITSA4")).ReturnsAsync((UsuarioPrecoMedioResponse?)null);

        var result = await _service.GetPrecoMedioAsync(1, "ITSA4");

        Assert.False(result.Success);
        Assert.Equal("Nenhum preço médio encontrado para o usuário e ativo informados.", result.ErrorMessage);
    }
    #endregion

    #region TestesPosicao
    [Fact]
    public async Task GetPosicao_ReturnsSuccess_WhenDataIsValid()
    {
        var lista = new List<PosicaoResponse> { new PosicaoResponse { CodigoAtivo = "VALE3" } };
        _mockUsuariosContext.Setup(x => x.ObterPosicoesAsync(1)).ReturnsAsync(lista);

        var result = await _service.GetPosicao(1);

        Assert.True(result.Success);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetPosicao_ReturnsFail_WhenUserIdInvalid()
    {
        var result = await _service.GetPosicao(0);

        Assert.False(result.Success);
        Assert.Equal("ID do usuário inválido.", result.ErrorMessage);
    }

    [Fact]
    public async Task GetPosicao_ReturnsFail_WhenResultIsEmpty()
    {
        _mockUsuariosContext.Setup(x => x.ObterPosicoesAsync(5)).ReturnsAsync(new List<PosicaoResponse>());

        var result = await _service.GetPosicao(5);

        Assert.False(result.Success);
        Assert.Equal("Nenhuma posição encontrada para o usuário.", result.ErrorMessage);
    }
    #endregion

    #region TestesCorretagemTotalComParametro
    [Fact]
    public async Task GetCorretagemTotal_ReturnsSuccess_WhenDataIsValid()
    {
        var corretagem = new CorretagemTotalResponse { NomUser = "Carlos", TotalCorretagem = 50 };
        _mockUsuariosContext.Setup(x => x.GetCorretagemTotal(1)).ReturnsAsync(corretagem);

        var result = await _service.GetCorretagemTotal(1);

        Assert.True(result.Success);
        Assert.Equal("Carlos", result.Data?.NomUser);
    }

    [Fact]
    public async Task GetCorretagemTotal_ReturnsFail_WhenUserIdInvalid()
    {
        var result = await _service.GetCorretagemTotal(0);

        Assert.False(result.Success);
        Assert.Equal("ID do usuário inválido.", result.ErrorMessage);
    }

    [Fact]
    public async Task GetCorretagemTotal_ReturnsFail_WhenNoData()
    {
        _mockUsuariosContext.Setup(x => x.GetCorretagemTotal(3)).ReturnsAsync((CorretagemTotalResponse?)null);

        var result = await _service.GetCorretagemTotal(3);

        Assert.False(result.Success);
        Assert.Equal("Nenhuma corretagem encontrada para o usuário.", result.ErrorMessage);
    }
    #endregion

    #region TestesPosicaoTotal
    [Fact]
    public async Task GetPosicaoTotal_ReturnsSuccess_WhenDataIsValid()
    {
        var lista = new List<PosicaoTotalResponse> { new PosicaoTotalResponse { NomUser = "user1", TotalPosicao = 100 } };
        _mockUsuariosContext.Setup(x => x.GetPosicoesTotal()).ReturnsAsync(lista);

        var result = await _service.GetPosicaoTotal();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetPosicaoTotal_ReturnsFail_WhenNoData()
    {
        _mockUsuariosContext.Setup(x => x.GetPosicoesTotal()).ReturnsAsync(new List<PosicaoTotalResponse>());

        var result = await _service.GetPosicaoTotal();

        Assert.False(result.Success);
        Assert.Equal("Nenhuma posição total encontrada.", result.ErrorMessage);
    }
    #endregion

    #region TestesCorretagemTotal
    [Fact]
    public async Task GetCorretagemTotalAllUsers_ReturnsSuccess_WhenDataIsValid()
    {
        var lista = new List<CorretagemTotalResponse> { new CorretagemTotalResponse { NomUser = "Ana", TotalCorretagem = 30 } };
        _mockUsuariosContext.Setup(x => x.GetCorretagemTotal()).ReturnsAsync(lista);

        var result = await _service.GetCorretagemTotal();

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task GetCorretagemTotalAllUsers_ReturnsFail_WhenNoData()
    {
        _mockUsuariosContext.Setup(x => x.GetCorretagemTotal()).ReturnsAsync(new List<CorretagemTotalResponse>());

        var result = await _service.GetCorretagemTotal();

        Assert.False(result.Success);
        Assert.Equal("Nenhuma corretagem total encontrada.", result.ErrorMessage);
    }
    #endregion
}
