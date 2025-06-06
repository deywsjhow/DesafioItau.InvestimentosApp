using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using DesafioItau.InvestimentosApp.Controllers;
using DesafioItau.InvestimentosApp.Domain.Usuarios;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Test.Controllers
{
    public class UsuariosControllerTest
    {
        #region TesteConsultaPrecoMedioAtivoPorUsuario
        [Fact]
        public async Task ConsultaPrecoMedioAtivoPorUsuario_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<IUsuariosService>();
            var fakeData = new UsuarioPrecoMedioResponse { NomUser = "User1", NomAtivo = "ITSA4", PrecoMedio = 10.5M };
            mockService.Setup(s => s.GetPrecoMedioAsync(1, "ITSA4"))
                       .ReturnsAsync(ServiceResult<UsuarioPrecoMedioResponse>.Ok(fakeData));

            var controller = new UsuariosController();

            // Act
            var result = await controller.ConsultaPrecoMedioAtivoPorUsuario(1, "ITSA4", mockService.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UsuarioPrecoMedioResponse>(okResult.Value);
            Assert.Equal("User1", returnValue.NomUser);
            Assert.Equal("ITSA4", returnValue.NomAtivo);
            Assert.Equal(10.5M, returnValue.PrecoMedio);
        }

        [Fact]
        public async Task ConsultaPrecoMedioAtivoPorUsuario_ReturnsBadRequest_WhenFailWithMessage()
        {
            // Arrange
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetPrecoMedioAsync(1, "ITSA4"))
                       .ReturnsAsync(ServiceResult<UsuarioPrecoMedioResponse>.Fail("Erro ao buscar"));

            var controller = new UsuariosController();

            // Act
            var result = await controller.ConsultaPrecoMedioAtivoPorUsuario(1, "ITSA4", mockService.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro ao buscar", badRequestResult.Value);
        }

        [Fact]
        public async Task ConsultaPrecoMedioAtivoPorUsuario_ReturnsBadRequest_WhenFailWithoutMessage()
        {
            // Arrange
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetPrecoMedioAsync(1, "ITSA4"))
                       .ReturnsAsync(new ServiceResult<UsuarioPrecoMedioResponse> { Success = false, ErrorMessage = null });

            var controller = new UsuariosController();

            // Act
            var result = await controller.ConsultaPrecoMedioAtivoPorUsuario(1, "ITSA4", mockService.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Null(badRequestResult.Value);
        }

        [Fact]
        public async Task ConsultaPrecoMedioAtivoPorUsuario_CallsServiceWithCorrectParameters()
        {
            // Arrange
            var mockService = new Mock<IUsuariosService>();
            var fakeData = new UsuarioPrecoMedioResponse();
            mockService.Setup(s => s.GetPrecoMedioAsync(It.IsAny<int>(), It.IsAny<string>()))
                       .ReturnsAsync(ServiceResult<UsuarioPrecoMedioResponse>.Ok(fakeData));

            var controller = new UsuariosController();

            // Act
            var _ = await controller.ConsultaPrecoMedioAtivoPorUsuario(123, "CODE", mockService.Object);

            // Assert
            mockService.Verify(s => s.GetPrecoMedioAsync(123, "CODE"), Times.Once);
        }
        #endregion

        #region TesteConsultaPosicoesAsync
        [Fact]
        public async Task ConsultaPosicoesAsync_ReturnsOk_WhenSuccess()
        {
            var mockService = new Mock<IUsuariosService>();
            var fakeData = new List<PosicaoResponse> { new PosicaoResponse() };
            mockService.Setup(s => s.GetPosicao(1))
                       .ReturnsAsync(ServiceResult<IEnumerable<PosicaoResponse>>.Ok(fakeData));

            var controller = new UsuariosController();

            var result = await controller.ConsultaPosicoesAsync(1, mockService.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<PosicaoResponse>>(okResult.Value);
            Assert.NotEmpty(value);
        }

        [Fact]
        public async Task ConsultaPosicoesAsync_ReturnsBadRequest_WhenFailWithMessage()
        {
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetPosicao(1))
                       .ReturnsAsync(ServiceResult<IEnumerable<PosicaoResponse>>.Fail("Erro ao buscar"));

            var controller = new UsuariosController();

            var result = await controller.ConsultaPosicoesAsync(1, mockService.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro ao buscar", badRequest.Value);
        }

        [Fact]
        public async Task ConsultaPosicoesAsync_ReturnsBadRequest_WhenFailWithoutMessage()
        {
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetPosicao(1))
                       .ReturnsAsync(new ServiceResult<IEnumerable<PosicaoResponse>> { Success = false, ErrorMessage = null });

            var controller = new UsuariosController();

            var result = await controller.ConsultaPosicoesAsync(1, mockService.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Null(badRequest.Value);
        }

        [Fact]
        public async Task ConsultaPosicoesAsync_CallsServiceWithCorrectParameters()
        {
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetPosicao(It.IsAny<int>()))
                       .ReturnsAsync(ServiceResult<IEnumerable<PosicaoResponse>>.Ok(new List<PosicaoResponse>()));

            var controller = new UsuariosController();

            await controller.ConsultaPosicoesAsync(99, mockService.Object);

            mockService.Verify(s => s.GetPosicao(99), Times.Once);
        }
        #endregion

        #region TesteConsultaCorregatemTotal
        [Fact]
        public async Task ConsultaCorretagemTotal_ReturnsOk_WhenSuccess()
        {
            var mockService = new Mock<IUsuariosService>();
            var fakeData = new CorretagemTotalResponse { /* preencher propriedades se quiser */ };
            mockService.Setup(s => s.GetCorretagemTotal(1))
                       .ReturnsAsync(ServiceResult<CorretagemTotalResponse>.Ok(fakeData));

            var controller = new UsuariosController();

            var result = await controller.ConsultaCorretagemTotal(1, mockService.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<CorretagemTotalResponse>(okResult.Value);
            Assert.NotNull(value);
        }

        [Fact]
        public async Task ConsultaCorretagemTotal_ReturnsBadRequest_WhenFailWithMessage()
        {
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetCorretagemTotal(1))
                       .ReturnsAsync(ServiceResult<CorretagemTotalResponse>.Fail("Erro no cálculo"));

            var controller = new UsuariosController();

            var result = await controller.ConsultaCorretagemTotal(1, mockService.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro no cálculo", badRequest.Value);
        }

        [Fact]
        public async Task ConsultaCorretagemTotal_ReturnsBadRequest_WhenFailWithoutMessage()
        {
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetCorretagemTotal(1))
                       .ReturnsAsync(new ServiceResult<CorretagemTotalResponse> { Success = false, ErrorMessage = null });

            var controller = new UsuariosController();

            var result = await controller.ConsultaCorretagemTotal(1, mockService.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Null(badRequest.Value);
        }

        [Fact]
        public async Task ConsultaCorretagemTotal_CallsServiceWithCorrectParameters()
        {
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetCorretagemTotal(It.IsAny<int>()))
                       .ReturnsAsync(ServiceResult<CorretagemTotalResponse>.Ok(new CorretagemTotalResponse()));

            var controller = new UsuariosController();

            await controller.ConsultaCorretagemTotal(77, mockService.Object);

            mockService.Verify(s => s.GetCorretagemTotal(77), Times.Once);
        }
        #endregion

        #region TesteGetPosicoesTotal
        [Fact]
        public async Task GetPosicoesTotal_ReturnsOk_WhenSuccess()
        {
            var mockService = new Mock<IUsuariosService>();
            var fakeData = new List<PosicaoTotalResponse> { new PosicaoTotalResponse() };
            mockService.Setup(s => s.GetPosicaoTotal())
                       .ReturnsAsync(ServiceResult<IEnumerable<PosicaoTotalResponse>>.Ok(fakeData));

            var controller = new UsuariosController();

            var result = await controller.GetPosicoesTotal(mockService.Object);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<PosicaoTotalResponse>>(okResult.Value);
            Assert.NotEmpty(value);
        }

        [Fact]
        public async Task GetPosicoesTotal_ReturnsBadRequest_WhenFailWithMessage()
        {
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetPosicaoTotal())
                       .ReturnsAsync(ServiceResult<IEnumerable<PosicaoTotalResponse>>.Fail("Erro ao buscar total"));

            var controller = new UsuariosController();

            var result = await controller.GetPosicoesTotal(mockService.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro ao buscar total", badRequest.Value);
        }

        [Fact]
        public async Task GetPosicoesTotal_ReturnsBadRequest_WhenFailWithoutMessage()
        {
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetPosicaoTotal())
                       .ReturnsAsync(new ServiceResult<IEnumerable<PosicaoTotalResponse>> { Success = false, ErrorMessage = null });

            var controller = new UsuariosController();

            var result = await controller.GetPosicoesTotal(mockService.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Null(badRequest.Value);
        }

        [Fact]
        public async Task GetPosicoesTotal_CallsService()
        {
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetPosicaoTotal())
                       .ReturnsAsync(ServiceResult<IEnumerable<PosicaoTotalResponse>>.Ok(new List<PosicaoTotalResponse>()));

            var controller = new UsuariosController();

            await controller.GetPosicoesTotal(mockService.Object);

            mockService.Verify(s => s.GetPosicaoTotal(), Times.Once);
        }
        #endregion

        #region TesteGetCorretagemTotal
        [Fact]
        public async Task GetCorretagemTotal_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<IUsuariosService>();
            var fakeData = new List<CorretagemTotalResponse> { new CorretagemTotalResponse() };
            mockService.Setup(s => s.GetCorretagemTotal())
                       .ReturnsAsync(ServiceResult<IEnumerable<CorretagemTotalResponse>>.Ok(fakeData));

            var controller = new UsuariosController();

            // Act
            var result = await controller.GetCorretagemTotal(mockService.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<CorretagemTotalResponse>>(okResult.Value);
            Assert.NotEmpty(value);
        }

        [Fact]
        public async Task GetCorretagemTotal_ReturnsBadRequest_WhenFailWithMessage()
        {
            // Arrange
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetCorretagemTotal())
                       .ReturnsAsync(ServiceResult<IEnumerable<CorretagemTotalResponse>>.Fail("Erro ao buscar corretagem total"));

            var controller = new UsuariosController();

            // Act
            var result = await controller.GetCorretagemTotal(mockService.Object);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erro ao buscar corretagem total", badRequest.Value);
        }

        [Fact]
        public async Task GetCorretagemTotal_ReturnsBadRequest_WhenFailWithoutMessage()
        {
            // Arrange
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetCorretagemTotal())
                       .ReturnsAsync(new ServiceResult<IEnumerable<CorretagemTotalResponse>> { Success = false, ErrorMessage = null });

            var controller = new UsuariosController();

            // Act
            var result = await controller.GetCorretagemTotal(mockService.Object);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Null(badRequest.Value);
        }

        [Fact]
        public async Task GetCorretagemTotal_CallsService()
        {
            // Arrange
            var mockService = new Mock<IUsuariosService>();
            mockService.Setup(s => s.GetCorretagemTotal())
                       .ReturnsAsync(ServiceResult<IEnumerable<CorretagemTotalResponse>>.Ok(new List<CorretagemTotalResponse>()));

            var controller = new UsuariosController();

            // Act
            await controller.GetCorretagemTotal(mockService.Object);

            // Assert
            mockService.Verify(s => s.GetCorretagemTotal(), Times.Once);
        }
        #endregion
    }
}
