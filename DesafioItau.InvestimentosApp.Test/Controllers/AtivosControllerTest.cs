using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using DesafioItau.InvestimentosApp.Controllers;
using DesafioItau.InvestimentosApp.Domain.Ativos;
using System.Threading.Tasks;
public class AtivosControllerTests
{
    [Fact]
    public async Task ConsultaUltimaCotacaoAtivo_ReturnsOk_WhenAtivoFound()
    {
        // Arrange
        var mockService = new Mock<IAtivosService>();
        var fakeResponse = new AtivosResponse { Ativo = "ITSA4", Preco = 10.55M };

        mockService.Setup(s => s.GetAtivo("ITSA4"))
                   .ReturnsAsync(fakeResponse);

        var controller = new AtivosController();

        // Act
        var result = await controller.ConsultaUltimaCotacaoAtivo("ITSA4", mockService.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AtivosResponse>(okResult.Value);
        Assert.Equal("ITSA4", value.Ativo);
    }

    [Fact]
    public async Task ConsultaUltimaCotacaoAtivo_ReturnsBadRequest_WhenNotFound()
    {
        // Arrange
        var mockService = new Mock<IAtivosService>();
        mockService.Setup(s => s.GetAtivo("VAZIO"))
                   .ReturnsAsync((AtivosResponse?)null);

        var controller = new AtivosController();

        // Act
        var result = await controller.ConsultaUltimaCotacaoAtivo("VAZIO", mockService.Object);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task ConsultaUltimaCotacaoAtivo_ReturnsBadRequest_WhenCodigoIsNullOrEmpty()
    {
        var mockService = new Mock<IAtivosService>();
        var controller = new AtivosController();

        var resultNull = await controller.ConsultaUltimaCotacaoAtivo(null!, mockService.Object);
        var resultEmpty = await controller.ConsultaUltimaCotacaoAtivo("", mockService.Object);

        Assert.IsType<BadRequestObjectResult>(resultNull);
        Assert.IsType<BadRequestObjectResult>(resultEmpty);
    }

    [Fact]
    public async Task ConsultaUltimaCotacaoAtivo_CallsGetAtivo_Once()
    {
        var mockService = new Mock<IAtivosService>();
        var codigo = "ITSA4";
        mockService.Setup(s => s.GetAtivo(codigo)).ReturnsAsync(new AtivosResponse());

        var controller = new AtivosController();

        await controller.ConsultaUltimaCotacaoAtivo(codigo, mockService.Object);

        mockService.Verify(s => s.GetAtivo(codigo), Times.Once());
    }


}
