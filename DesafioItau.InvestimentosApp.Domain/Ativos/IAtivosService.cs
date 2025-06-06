using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Domain.Ativos
{
    public interface IAtivosService
    {
        Task<ServiceResult<AtivosResponse>> GetAtivo(string codigo);
    }
}
