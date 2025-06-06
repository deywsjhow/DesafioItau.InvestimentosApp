using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DesafioItau.InvestimentosApp.Repository.DbAtivosContext
{
    public class DbAtivosContext : IAtivosContext
    {
        private readonly string _connectionString;
        private readonly ILogger<DbAtivosContext> _logger;

        public DbAtivosContext(IConfiguration configuration, ILogger<DbAtivosContext> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _logger = logger;
        }

        private SqlConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<RetornoAtivosBD?> GetAtivo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                _logger.LogWarning("Código do ativo não informado.");
                return null;
            }

            const string sql = @"
                SELECT TOP 1 id, codigo, nome
                    FROM [dbo].[Ativos] 
                        WITH (NOLOCK, INDEX (Ind_Ativos_01))
                WHERE codigo = @Codigo";

            try
            {
                using var connection = CreateConnection();
                return await connection.QueryFirstOrDefaultAsync<RetornoAtivosBD>(sql, new { Codigo = codigo });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ativo com código {Codigo}", codigo);
                return null;
            }
        }
    }
}
