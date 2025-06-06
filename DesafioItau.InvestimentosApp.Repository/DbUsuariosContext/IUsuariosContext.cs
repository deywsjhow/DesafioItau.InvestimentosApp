using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Repository.DbUsuariosContext
{
    public interface IUsuariosContext
    {
        Task<UsuarioPrecoMedioResponse> GetPrecoMedioAsync(int UsuarioId, string AtivoId);
        Task<IEnumerable<PosicaoResponse>> ObterPosicoesAsync(int UsuarioId);
        Task<CorretagemTotalResponse> GetCorretagemTotal(int UsuarioId);
        Task<IEnumerable<PosicaoTotalResponse>> GetPosicoesTotal();
        Task<IEnumerable<CorretagemTotalResponse>> GetCorretagemTotal();
    }
}
