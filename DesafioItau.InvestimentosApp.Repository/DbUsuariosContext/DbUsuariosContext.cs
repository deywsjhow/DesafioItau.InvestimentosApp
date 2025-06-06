using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;

namespace DesafioItau.InvestimentosApp.Repository.DbUsuariosContext
{
    public class DbUsuariosContext(IConfiguration configuration) : IUsuariosContext
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        public IDbConnection Connection()
        {
            return new SqlConnection(_connectionString);
        }


        public async Task<UsuarioPrecoMedioResponse> GetPrecoMedioAsync(int UsuarioId, string AtivoId)
        {
            var result = new UsuarioPrecoMedioResponse();

            try
            {
                using var connectionDB = this.Connection();
                connectionDB.Open();

                string sql = @"SELECT us.nome 'NomUser', av.nome 'NomAtivo', sum (op.quantidade * (op.preco_unitario + op.Corretagem)) / nullif (sum (op.quantidade), 0) 'PrecoMedio'
	                              FROM [dbo].[Operacoes] op 
	                              	WITH (NOLOCK, INDEX(Ind_Operacoes_01))
	                              INNER JOIN [dbo].[Usuarios] us
	                              	WITH (NOLOCK INDEX (1))
	                              	ON us.id	= op.id_usuario
	                              INNER JOIN [dbo].[Ativos] av
	                              	WITH (NOLOCK, INDEX (Ind_Ativos_01))
	                              	ON av.codigo	     = op.id_ativo
	                              WHERE op.id_usuario    = @usuarioId
	                                AND op.id_ativo      = @ativoId 
	                                AND op.tipo_operacao = 'c'
	                                AND op.data_compra  >= dateadd (day, -30, getdate ())";

                var ativo = await connectionDB.QueryFirstOrDefaultAsync<UsuarioPrecoMedioResponse>(sql, new {usuarioId = UsuarioId, ativoId = AtivoId});

                if (ativo != null) 
                    result = ativo;     

            }
            catch (Exception ex) { 
            }

            return result;
        }

        public async Task<IEnumerable<PosicaoResponse>> ObterPosicoesAsync(int UsuarioId)
        {
            IEnumerable<PosicaoResponse> result = new List<PosicaoResponse>();

            try
            {
                using var connectionDB = this.Connection();
                connectionDB.Open();

                string @sql = @"SELECT av.Codigo 'CodigoAtivo',  av.Nome 'NomeAtivo',  ps.quantidade 'Quantidade',  ps.preco_medio 'PrecoMedio',  
		                                round ((isnull (cs.preco_unitario, 0) - ps.preco_medio) * ps.Quantidade, 2)  'PL'
	                                FROM [dbo].[Posicoes] ps
		                                WITH (NOLOCK, INDEX (1))
                                    INNER JOIN [dbo].[Ativos] av
		                                WITH (NOLOCK, INDEX (Ind_Ativos_01))
                                        ON av.codigo		  = ps.id_ativo
                                    OUTER APPLY (SELECT TOP 1 ct.Preco_Unitario
                                        FROM [dbo].[Cotacoes] ct
                                            WITH (NOLOCK, INDEX (Ind_Cotacoes_01))
                                        WHERE  ct.Id_Ativo = av.Codigo
                                        ORDER BY ct.data_hora DESC ) cs
	                                WHERE ps.id_usuario	  = @usuarioId";

                var posicao = await connectionDB.QueryAsync<PosicaoResponse>(sql, new { usuarioId = UsuarioId });

                return posicao;
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public async Task<CorretagemTotalResponse> GetCorretagemTotal(int UsuarioId)
        {
            var result = new CorretagemTotalResponse();

            try
            {
                using var connectionDB = this.Connection();
                connectionDB.Open();

                string sql = @"SELECT us.nome 'NomUser', sum (op.corretagem) 'TotalCorretagem'
                                  FROM [dbo].[Operacoes] op
                                     WITH (NOLOCK, INDEX(Ind_Operacoes_01))
                                  INNER JOIN [dbo].[Usuarios] us
                                     WITH (NOLOCK, INDEX (1))
                                     ON us.id      = op.id_usuario
                                  WHERE id_usuario = @usuarioId";

                var ValorToTal = await connectionDB.ExecuteScalarAsync<CorretagemTotalResponse>(sql, new { usuarioId = UsuarioId});

                if (ValorToTal != null)
                    result = ValorToTal;

            }
            catch (Exception ex)
            {
            }

            return result;
        }

        public async Task<IEnumerable<PosicaoTotalResponse>> GetPosicoesTotal()
        {
            IEnumerable<PosicaoTotalResponse> result = new List<PosicaoTotalResponse>();

            try
            {
                using var connectionDB = this.Connection();
                connectionDB.Open();

                string @sql = @"SELECT ps.id_usuario, SUM(ps.quantidade * ca.preco_unitario) 'PosicaoTotal'
	                               FROM [dbo].[posicoes] ps
	                               	WITH (NOLOCK)
	                               CROSS APPLY (SELECT TOP 1 cs.preco_unitario
	                               				    FROM [dbo].[Cotacoes] cs
	                               					    WITH (NOLOCK INDEX(Ind_Cotacoes_01))
	                               					 WHERE cs.id_ativo = ps.id_ativo) ca                                         
                                    GROUP BY ps.id_usuario
                                    ORDER BY 'PosicaoTotal' DESC
                                    OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";

                var posicaoTotal = await connectionDB.QueryAsync<PosicaoTotalResponse>(sql);

                return posicaoTotal;
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        public async Task<IEnumerable<CorretagemTotalResponse>> GetCorretagemTotal()
        {
            IEnumerable<CorretagemTotalResponse> result = new List<CorretagemTotalResponse>();

            try
            {
                using var connectionDB = this.Connection();
                connectionDB.Open();

                string @sql = @"SELECT us.nome 'NomUser', sum (op.corretagem) 'TotalCorretagem'
                                  FROM [dbo].[Operacoes] op
                                     WITH (NOLOCK, INDEX(Ind_Operacoes_01))
                                  INNER JOIN [dbo].[Usuarios] us
                                     WITH (NOLOCK, INDEX (1))
                                     ON us.id      = op.id_usuario
                                  GROUP BY op.id_usuario
                                  ORDER BY TotalCorretagem
                                  OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY";

                var posicaoTotal = await connectionDB.QueryAsync<CorretagemTotalResponse>(sql);

                return posicaoTotal;
            }
            catch (Exception ex)
            {

            }

            return result;
        }
    }
}
