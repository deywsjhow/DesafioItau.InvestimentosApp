using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Domain.Usuarios
{
    public interface IUsuariosService
    {
        Task<UsuarioPrecoMedioResponse> GetPrecoMedioAsync(int UsuarioId, string AtivoId);
        Task<IEnumerable<PosicaoResponse>> GetPosicao(int usuarioId);
        Task<CorretagemTotalResponse> GetCorretagemTotal(int UsuarioId);
        Task<IEnumerable<PosicaoTotalResponse>> GetPosicaoTotal();
        Task<IEnumerable<CorretagemTotalResponse>> GetCorretagemTotal();
    }
}
