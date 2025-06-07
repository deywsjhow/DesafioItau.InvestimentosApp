
using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;

namespace DesafioItau.InvestimentosApp.Domain.Ativos
{
    public class AtivosService(IAtivosContext ativosContext, ICotacoesContext cotacoesContext) : IAtivosService
    {
        public async Task<ServiceResult<AtivosResponse>> GetAtivo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return ServiceResult<AtivosResponse>.Fail("Código do ativo não informado.");

            var ativo = await ativosContext.GetAtivo(codigo);
            if (ativo is null)
                return ServiceResult<AtivosResponse>.Fail("Ativo não encontrado.");

            var cotacao = await cotacoesContext.GetCotacao(ativo.id);
            if (cotacao is null)
                return ServiceResult<AtivosResponse>.Fail("Nenhuma cotação encontrada para o ativo informado.");

            var response = new AtivosResponse
            {
                Ativo = ativo.codigo,
                Preco = cotacao.preco_unitario,
                DataHora = cotacao.data_hora
            };

            return ServiceResult<AtivosResponse>.Ok(response);
        }
    }
}
