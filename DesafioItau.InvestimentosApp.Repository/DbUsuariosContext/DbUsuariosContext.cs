using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using DesafioItau.InvestimentosApp.Common.Models.UsuariosModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using Dapper;

namespace DesafioItau.InvestimentosApp.Repository.DbUsuariosContext
{
    public class DbUsuariosContext : IUsuariosContext
    {
        private readonly string _connectionString;
        private readonly ILogger<DbUsuariosContext> _logger;

        public DbUsuariosContext(IConfiguration configuration, ILogger<DbUsuariosContext> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private SqlConnection Connection() => new SqlConnection(_connectionString);

        public async Task<UsuarioPrecoMedioResponse?> GetPrecoMedioAsync(int UsuarioId, string AtivoId)
        {
            const string sql = @"SELECT us.nome 'NomUser', av.nome 'NomAtivo',
                                        sum (op.quantidade * (op.preco_unitario + op.Corretagem)) / nullif (sum (op.quantidade), 0) 'PrecoMedio'
                                   FROM [dbo].[Operacoes] op 
                                       WITH (NOLOCK, INDEX(Ind_Operacoes_01))
                                   INNER JOIN [dbo].[Usuarios] us
                                       WITH (NOLOCK INDEX (1)) 
                                       ON us.id = op.id_usuario
                                   INNER JOIN [dbo].[Ativos] av
                                       WITH (NOLOCK, INDEX (Ind_Ativos_01)) 
                                       ON av.id = op.id_ativo
                                   WHERE op.id_usuario = @usuarioId
                                     AND op.id_ativo = @ativoId 
                                     AND op.tipo_operacao = 'c'
                                     AND op.data_compra >= dateadd (day, -30, getdate ())
                                     GROUP BY us.nome, av.nome
                                     ORDER BY av.nome";

            try
            {
                using var connection = Connection();
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<UsuarioPrecoMedioResponse>(sql, new { usuarioId = UsuarioId, ativoId = AtivoId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter PrecoMedio para usuario {UsuarioId} e ativo {AtivoId}", UsuarioId, AtivoId);
                return null;
            }
        }

        public async Task<IEnumerable<PosicaoResponse>> ObterPosicoesAsync(int UsuarioId)
        {
            const string sql = @"SELECT av.Codigo 'CodigoAtivo', av.Nome 'NomeAtivo', ps.quantidade 'Quantidade', ps.preco_medio 'PrecoMedio',
                                        round ((isnull (cs.preco_unitario, 0) - ps.preco_medio) * ps.Quantidade, 2) 'PL'
                                   FROM [dbo].[Posicoes] ps
                                       WITH (NOLOCK, INDEX (1))
                                   INNER JOIN [dbo].[Ativos] av
                                       WITH (NOLOCK, INDEX (Ind_Ativos_01)) 
                                       ON av.id = ps.id_ativo
                                   OUTER APPLY (SELECT TOP 1 ct.Preco_Unitario
                                                  FROM [dbo].[Cotacoes] ct
                                                      WITH (NOLOCK, INDEX (Ind_Cotacoes_01))
                                                  WHERE ct.Id_Ativo = av.id
                                                  ORDER BY ct.data_hora DESC ) cs
                                   WHERE ps.id_usuario = @usuarioId";

            try
            {
                using var connection = Connection();
                connection.Open();
                return await connection.QueryAsync<PosicaoResponse>(sql, new { usuarioId = UsuarioId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter posicoes do usuario {UsuarioId}", UsuarioId);
                return Enumerable.Empty<PosicaoResponse>();
            }
        }

        public async Task<CorretagemTotalResponse?> GetCorretagemTotal(int UsuarioId)
        {
            const string sql = @"SELECT us.nome 'NomUser', sum (op.corretagem) 'TotalCorretagem'
                                   FROM [dbo].[Operacoes] op
                                       WITH (NOLOCK, INDEX(Ind_Operacoes_01))
                                   INNER JOIN [dbo].[Usuarios] us
                                       WITH (NOLOCK, INDEX (1)) 
                                       ON us.id = op.id_usuario
                                   WHERE op.id_usuario = @usuarioId
                                   GROUP BY us.nome
								   ORDER BY 'TotalCorretagem'";

            try
            {
                using var connection = Connection();
                connection.Open();
                return await connection.QueryFirstOrDefaultAsync<CorretagemTotalResponse>(sql, new { usuarioId = UsuarioId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter corretagem total do usuario {UsuarioId}", UsuarioId);
                return null;
            }
        }

        public async Task<IEnumerable<PosicaoTotalResponse>> GetPosicoesTotal()
        {
            const string sql = @"SELECT us.nome 'NomUser', SUM(ps.quantidade * ca.preco_unitario) 'TotalPosicao'
                                   FROM [dbo].[Posicoes] ps 
                                      WITH (NOLOCK)
                                   INNER JOIN [dbo].[Usuarios] us
								      WITH (NOLOCK, INDEX(1))
								      ON us.id = ps.id
                                   CROSS APPLY (SELECT TOP 1 cs.preco_unitario
                                                  FROM [dbo].[Cotacoes] cs
                                                      WITH (NOLOCK INDEX(Ind_Cotacoes_01))
                                                  WHERE cs.id_ativo = ps.id_ativo) ca                                          
                                GROUP BY us.nome
                                ORDER BY 'TotalPosicao' DESC
                               ";

            try
            {
                using var connection = Connection();
                connection.Open();
                return await connection.QueryAsync<PosicaoTotalResponse>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter posicoes totais");
                return Enumerable.Empty<PosicaoTotalResponse>();
            }
        }

        public async Task<IEnumerable<CorretagemTotalResponse>> GetCorretagemTotal()
        {
            const string sql = @"SELECT us.nome 'NomUser', sum (op.corretagem) 'TotalCorretagem'
                                   FROM [dbo].[Operacoes] op
                                       WITH (NOLOCK, INDEX(Ind_Operacoes_01))
                                   INNER JOIN [dbo].[Usuarios] us
                                       WITH (NOLOCK, INDEX (1)) 
                                       ON us.id = op.id_usuario
                                GROUP BY us.nome
                                ORDER BY 'TotalCorretagem' DESC";

            try
            {
                using var connection = Connection();
                connection.Open();
                return await connection.QueryAsync<CorretagemTotalResponse>(sql);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter corretagem total geral");
                return Enumerable.Empty<CorretagemTotalResponse>();
            }
        }
    }
}
