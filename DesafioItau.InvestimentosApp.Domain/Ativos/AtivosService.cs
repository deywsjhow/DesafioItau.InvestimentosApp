

using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;

namespace DesafioItau.InvestimentosApp.Domain.Ativos
{
    public class AtivosService(IAtivosContext ativosContext, ICotacoesContext cotacoesContext) : IUsuariosService
    {
        public async Task<AtivosResponse> GetAtivo(string codigo)
        {
            var AtivosResponse = new AtivosResponse();

            if (codigo is null)
                return new AtivosResponse();

            //chama get ativo para recuperar as infos do ativo
            var ret = await ativosContext.GetAtivo(codigo);

            if (ret is null)
                return new AtivosResponse();

            //Recuperado o id do ativo para fazer a chamada a tabela de cotacoes
            int idAtivo = ret.id;

            //Chama tabela de cotação para recuperar os valores a serem devolvidos
            var retCotacoes = await cotacoesContext.GetCotacao(idAtivo);

            if (retCotacoes is null)
                return new AtivosResponse();

            AtivosResponse.Ativo = retCotacoes.id_ativo.ToString();
            AtivosResponse.Preco = retCotacoes.preco_unitario;
            AtivosResponse.DataHora = retCotacoes.data_hora;


            return AtivosResponse;
        }


    }
}
