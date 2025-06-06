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
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<RetornoCotacoesBD?> GetCotacao(int id)
        {
            const string sql = @"
                SELECT TOP 1 id, id_ativo, preco_unitario, data_hora
                   FROM [dbo].[Cotacoes] 
                      WITH (NOLOCK, INDEX (Ind_Cotacoes_01))
                WHERE id_ativo = @Id";

            try
            {
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<RetornoCotacoesBD>(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogWarning("ID inválido para cotação: {Id}", id);
                return null;
            }
        }
    }
}
