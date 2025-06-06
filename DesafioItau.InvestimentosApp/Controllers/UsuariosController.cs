using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using DesafioItau.InvestimentosApp.Domain.Usuarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Controllers
{
    [ApiController]
    [Route("api/usuarios/")]
    public class UsuariosController : Controller
    {
        [HttpGet("{usuarioId}/ativos/{codigo}/preco-medio")]
        [ProducesResponseType(typeof(UsuarioPrecoMedioResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UsuarioPrecoMedioResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ConsultaPrecoMedioAtivoPorUsuario(int usuarioId, string codigo, IUsuariosService usuariosService)
        {
            var result = await usuariosService.GetPrecoMedioAsync(usuarioId, codigo);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{usuarioId}/posicoes")]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ConsultaPosicoesAsync(int usuarioId, IUsuariosService usuariosService)
        {
            var result = await usuariosService.GetPosicao(usuarioId);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("{usuarioId}/corretagem")]
        [ProducesResponseType(typeof(CorretagemTotalResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CorretagemTotalResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ConsultaCorretagemTotal(int usuarioId, IUsuariosService usuariosService)
        {
            var result = await usuariosService.GetCorretagemTotal(usuarioId);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("total/posicoes")]
        [ProducesResponseType(typeof(PosicaoTotalResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PosicaoTotalResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetPosicoesTotal(IUsuariosService usuariosService)
        {
            var result = await usuariosService.GetPosicaoTotal();

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }

        [HttpGet("total/corretagem")]
        [ProducesResponseType(typeof(CorretagemTotalResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CorretagemTotalResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCorretagemTotal(IUsuariosService usuariosService)
        {
            var result = await usuariosService.GetCorretagemTotal();

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
    }
}
