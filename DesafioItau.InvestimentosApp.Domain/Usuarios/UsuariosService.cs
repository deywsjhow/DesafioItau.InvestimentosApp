using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using DesafioItau.InvestimentosApp.Repository.DbUsuariosContext;

namespace DesafioItau.InvestimentosApp.Domain.Usuarios
{
    public class UsuariosService(IUsuariosContext usuariosContext) : IUsuariosService
    {
        public async Task<ServiceResult<UsuarioPrecoMedioResponse>> GetPrecoMedioAsync(int usuarioId, string ativoId)
        {
            if (usuarioId <= 0 || string.IsNullOrEmpty(ativoId))
                return ServiceResult<UsuarioPrecoMedioResponse>.Fail("ID do usuário ou código do ativo inválido.");

            var ret = await usuariosContext.GetPrecoMedioAsync(usuarioId, ativoId);

            if (ret is null)
                return ServiceResult<UsuarioPrecoMedioResponse>.Fail("Nenhum preço médio encontrado para o usuário e ativo informados.");

            return ServiceResult<UsuarioPrecoMedioResponse>.Ok(ret);
        }

        public async Task<ServiceResult<IEnumerable<PosicaoResponse>>> GetPosicao(int usuarioId)
        {
            if (usuarioId <= 0)
                return ServiceResult<IEnumerable<PosicaoResponse>>.Fail("ID do usuário inválido.");

            var ret = await usuariosContext.ObterPosicoesAsync(usuarioId);

            if (ret is null || !ret.Any())
                return ServiceResult<IEnumerable<PosicaoResponse>>.Fail("Nenhuma posição encontrada para o usuário.");

            return ServiceResult<IEnumerable<PosicaoResponse>>.Ok(ret);
        }

        public async Task<ServiceResult<CorretagemTotalResponse>> GetCorretagemTotal(int usuarioId)
        {
            if (usuarioId <= 0)
                return ServiceResult<CorretagemTotalResponse>.Fail("ID do usuário inválido.");

            var ret = await usuariosContext.GetCorretagemTotal(usuarioId);

            if (ret is null)
                return ServiceResult<CorretagemTotalResponse>.Fail("Nenhuma corretagem encontrada para o usuário.");

            return ServiceResult<CorretagemTotalResponse>.Ok(ret);
        }

        public async Task<ServiceResult<IEnumerable<PosicaoTotalResponse>>> GetPosicaoTotal()
        {
            var ret = await usuariosContext.GetPosicoesTotal();

            if (ret is null || !ret.Any())
                return ServiceResult<IEnumerable<PosicaoTotalResponse>>.Fail("Nenhuma posição total encontrada.");

            return ServiceResult<IEnumerable<PosicaoTotalResponse>>.Ok(ret);
        }

        public async Task<ServiceResult<IEnumerable<CorretagemTotalResponse>>> GetCorretagemTotal()
        {
            var ret = await usuariosContext.GetCorretagemTotal();

            if (ret is null || !ret.Any())
                return ServiceResult<IEnumerable<CorretagemTotalResponse>>.Fail("Nenhuma corretagem total encontrada.");

            return ServiceResult<IEnumerable<CorretagemTotalResponse>>.Ok(ret);
        }
    }
}
