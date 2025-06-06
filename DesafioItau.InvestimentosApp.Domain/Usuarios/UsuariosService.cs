

using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using DesafioItau.InvestimentosApp.Repository.DbUsuariosContext;

namespace DesafioItau.InvestimentosApp.Domain.Usuarios
{
    public class UsuariosService(IUsuariosContext usuariosContext) : IUsuariosService
    {
        public async Task<UsuarioPrecoMedioResponse> GetPrecoMedioAsync(int UsuarioId, string AtivoId)
        {

            if (UsuarioId.ToString() is null && AtivoId is null || UsuarioId <= 0)
                return new UsuarioPrecoMedioResponse();

            //chama get precomedio para recuperar as infos
            var ret = await usuariosContext.GetPrecoMedioAsync(UsuarioId, AtivoId);

            if (ret is null)
                return new UsuarioPrecoMedioResponse();


            return ret;
        }

        public async Task<IEnumerable<PosicaoResponse>> GetPosicao(int usuarioId)
        {
            IEnumerable<PosicaoResponse> result = new List<PosicaoResponse>();

            if (usuarioId.ToString() is null || usuarioId <= 0)
                return result;

            var ret = await usuariosContext.ObterPosicoesAsync(usuarioId);

            if (ret is null)
                return result;

            return ret;
        }

        public async Task<CorretagemTotalResponse> GetCorretagemTotal(int UsuarioId)
        {

            if (UsuarioId.ToString() is null || UsuarioId <= 0)
                return new CorretagemTotalResponse();

            //chama get precomedio para recuperar as infos
            var ret = await usuariosContext.GetCorretagemTotal(UsuarioId);

            if (ret is null)
                return new CorretagemTotalResponse();


            return ret;
        }

        public async Task<IEnumerable<PosicaoTotalResponse>> GetPosicaoTotal()
        {
            IEnumerable<PosicaoTotalResponse> result = new List<PosicaoTotalResponse>();

            var ret = await usuariosContext.GetPosicoesTotal();

            if (ret is null)
                return result;

            return ret;
        }
        public async Task<IEnumerable<CorretagemTotalResponse>> GetCorretagemTotal()
        {
            IEnumerable<CorretagemTotalResponse> result = new List<CorretagemTotalResponse>();

            var ret = await usuariosContext.GetCorretagemTotal();

            if (ret is null)
                return result;

            return ret;
        }
    }
}
