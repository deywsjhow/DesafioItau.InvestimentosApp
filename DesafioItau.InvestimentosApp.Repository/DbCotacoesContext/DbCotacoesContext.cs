using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DesafioItau.InvestimentosApp.Repository.DbCotacoesContext
{
    public class DbCotacoesContext : ICotacoesContext
    {
        private readonly string _connectionString;
        private readonly ILogger<DbCotacoesContext> _logger;

        public DbCotacoesContext(IConfiguration configuration, ILogger<DbCotacoesContext> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _logger = logger;
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<RetornoCotacoesBD?> GetCotacao(int id)
        {
            const string sql = @"
                SELECT TOP 1 id, id_ativo, preco_unitario, data_hora
                   FROM [dbo].[Cotacoes] 
                      WITH (NOLOCK, INDEX (1))
                WHERE id_ativo = @Id";

            try
            {
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<RetornoCotacoesBD>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ID inválido para cotação: {Id}", id);
                return null;
            }
        }

        public async Task<RetornoCotacoesBD?> GetCotacaoByAtivoAndDateTime(int idAtivo, DateTime date)
        {
            const string sql = @"
                SELECT TOP 1 id, id_ativo, preco_unitario, data_hora
                   FROM [dbo].[Cotacoes] 
                      WITH (NOLOCK, INDEX (Ind_Cotacoes_01))
                WHERE id_ativo  = @IdAtivo
                  AND data_hora = @Data";

            try
            {
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<RetornoCotacoesBD>(sql, new { IdAtivo = idAtivo, Data = date });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ID inválido para cotação: {Id}", idAtivo);
                return null;
            }
        }

        public async Task<bool> InsereNovaCotacaoNaBase(int idAtivo, decimal precoUnitario, DateTime data)
        {
            return true;
        }
    }
}
