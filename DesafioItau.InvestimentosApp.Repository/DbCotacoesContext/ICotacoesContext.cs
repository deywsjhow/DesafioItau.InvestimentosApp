using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Repository.DbAtivosContext
{
    public interface ICotacoesContext
    {
        Task<RetornoCotacoesBD?> GetCotacao(int id);
        Task<RetornoCotacoesBD?> GetCotacaoByAtivoAndDateTime(int idAtivo, DateTime date);
        Task<bool> InsereNovaCotacaoNaBase(int idAtivo, decimal precoUnitario, DateTime data);
    }
}
