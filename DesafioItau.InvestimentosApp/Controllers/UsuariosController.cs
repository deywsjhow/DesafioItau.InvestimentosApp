using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using DesafioItau.InvestimentosApp.Domain.Usuarios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

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
            //implementar a chamada do usuariosService
            var result = await usuariosService.GetPrecoMedioAsync(usuarioId, codigo);

            if (result is null)
                return BadRequest("Não foi encontrado o ativo informado para este usuário para retorno do cálculo.");

            return Ok(result);
        }

        [HttpGet("{usuarioId}/posicoes")]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ConsultaPosicoesAsync(int usuarioId, IUsuariosService usuariosService)
        {
            var result = await usuariosService.GetPosicao(usuarioId);

            if (result is null)
                return BadRequest("Não foi encontrada posicoes para este usuário");

            return Ok(result);  
        }

        [HttpGet("{usuarioId}/corretagem")]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ConsultaCorretagemTotal(int usuarioId, IUsuariosService usuariosService)
        {
            var result = await usuariosService.GetCorretagemTotal(usuarioId);

            if (result is null)
                return BadRequest("Não foi possivel calcular o valor das corretagens desse usuário");

            return Ok(result);
        }

        [HttpGet("total/posicoes")]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetPosicoesTotal(IUsuariosService usuariosService)
        {
            var result = await usuariosService.GetPosicaoTotal();

            if (result is null)
                return BadRequest("Não foi encontrada posicoes para calcular o total");

            return Ok(result);
        }


        [HttpGet("total/corretagem")]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(PosicaoResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetCorretagemTotal(IUsuariosService usuariosService)
        {
            var result = await usuariosService.GetCorretagemTotal();

            if (result is null)
                return BadRequest("Não foi encontrada corretagem para calcular o total");

            return Ok(result);
        }

    }
}
