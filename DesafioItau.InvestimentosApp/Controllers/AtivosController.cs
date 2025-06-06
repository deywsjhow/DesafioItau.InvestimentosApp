using DesafioItau.InvestimentosApp.Domain.Ativos;
using Microsoft.AspNetCore.Http;using Microsoft.AspNetCore.Mvc;

namespace DesafioItau.InvestimentosApp.Controllers
{
    [ApiController]
    [Route("api/ativos")]
    public class AtivosController : ControllerBase
    {
        [HttpGet("{codigo}/ultima-cotacao")]
        [ProducesResponseType(typeof(AtivosResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ConsultaUltimaCotacaoAtivo(string codigo, IAtivosService ativosService)
        {
            var result = await ativosService.GetAtivo(codigo);

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result.Data);
        }
    }
}
