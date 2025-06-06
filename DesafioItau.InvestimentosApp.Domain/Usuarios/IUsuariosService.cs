using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Domain.Usuarios
{
    public interface IUsuariosService
    {
        Task<ServiceResult<UsuarioPrecoMedioResponse>> GetPrecoMedioAsync(int usuarioId, string ativoId);
        Task<ServiceResult<IEnumerable<PosicaoResponse>>> GetPosicao(int usuarioId);
        Task<ServiceResult<CorretagemTotalResponse>> GetCorretagemTotal(int usuarioId);
        Task<ServiceResult<IEnumerable<PosicaoTotalResponse>>> GetPosicaoTotal();
        Task<ServiceResult<IEnumerable<CorretagemTotalResponse>>> GetCorretagemTotal();
    }
}
