using DesafioItau.InvestimentosApp.Domain.Ativos;
using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DesafioItau.InvestimentosApp.Controllers
{
    [ApiController]
    [Route("api/ativos/")]
    public class AtivosController : Controller
    {

        [HttpGet("{codigo}/ultima-cotacao")]
        [ProducesResponseType(typeof(AtivosResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(AtivosResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult> ConsultaUltimaCotacaoAtivo(string codigo, IAtivosService ativosService)
        {
            //implementar a chamada do ativosService
            var result = await ativosService.GetAtivo(codigo);

            if (result is null)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
