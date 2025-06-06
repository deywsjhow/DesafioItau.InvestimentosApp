using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Repository.DbAtivosContext
{
    public interface IAtivosContext
    {
        Task<RetornoAtivosBD> GetAtivo(string codigo);
    }
}
