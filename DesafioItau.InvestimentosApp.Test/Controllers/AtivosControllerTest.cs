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
                   .ReturnsAsync(ServiceResult<AtivosResponse>.Ok(fakeResponse));
        var controller = new AtivosController();

        // Act
        var result = await controller.ConsultaUltimaCotacaoAtivo("ITSA4", mockService.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<AtivosResponse>(okResult.Value);
        Assert.Equal("ITSA4", value.Ativo);
        Assert.Equal(10.55M, value.Preco);
    }

    [Fact]
    public async Task ConsultaUltimaCotacaoAtivo_ReturnsBadRequest_WhenNotFound()
    {
        // Arrange
        var mockService = new Mock<IAtivosService>();
        mockService.Setup(s => s.GetAtivo("VAZIO"))
                   .ReturnsAsync(ServiceResult<AtivosResponse>.Fail("Não encontrado"));
        var controller = new AtivosController();

        // Act
        var result = await controller.ConsultaUltimaCotacaoAtivo("VAZIO", mockService.Object);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Não encontrado", badRequest.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ConsultaUltimaCotacaoAtivo_ReturnsBadRequest_WhenCodigoIsNullOrEmpty(string? codigo)
    {
        // Arrange
        var mockService = new Mock<IAtivosService>();
        mockService.Setup(s => s.GetAtivo(It.IsAny<string>()))
                   .ReturnsAsync(ServiceResult<AtivosResponse>.Fail("Código inválido"));

        var controller = new AtivosController();

        // Act
        var result = await controller.ConsultaUltimaCotacaoAtivo(codigo!, mockService.Object);

        // Assert
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Código inválido", badRequest.Value);
    }

    [Fact]
    public async Task ConsultaUltimaCotacaoAtivo_CallsGetAtivo_Once()
    {
        // Arrange
        var mockService = new Mock<IAtivosService>();
        var codigo = "ITSA4";
        mockService.Setup(s => s.GetAtivo(codigo))
                   .ReturnsAsync(ServiceResult<AtivosResponse>.Ok(new AtivosResponse()));

        var controller = new AtivosController();

        // Act
        await controller.ConsultaUltimaCotacaoAtivo(codigo, mockService.Object);

        // Assert
        mockService.Verify(s => s.GetAtivo(codigo), Times.Once());
    }
}
