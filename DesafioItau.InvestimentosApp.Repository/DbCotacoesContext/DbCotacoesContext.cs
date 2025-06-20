﻿using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<bool> InsereNovaCotacaoNaBaseInsUpd(int idAtivo, decimal precoUnitario, DateTime data)
        {
            const string sql = @"
                                
                                UPDATE xx
                                	SET xx.id_ativo		    = @IdAtivo,
                                	    xx.preco_unitario   = @Preco,
                                	    xx.data_hora		= @Data
                                	FROM [dbo].[Cotacoes] xx
                                		WITH (ROWLOCK, INDEX(Ind_Cotacoes_01))
                                	WHERE xx.id_ativo = @IdAtivo                              
                               
                                INSERT INTO [dbo].[Cotacoes] (id_ativo, preco_unitario, data_hora)
                                	SELECT @IdAtivo, @Preco, @Data                                		
                                	WHERE NOT EXISTS (SELECT TOP 1 1
                                					     FROM [dbo].[Cotacoes] xx
                                					     	WITH (NOLOCK, INDEX(Ind_Cotacoes_01))
                                					     WHERE xx.id_ativo = @IdAtivo)
                                ";

            try
            {
                using var connection = CreateConnection();
                await connection.ExecuteAsync(sql, new { IdAtivo = idAtivo, Preco = precoUnitario, Data = data });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao inserir cotação na base: {Id}", idAtivo);
                return false;
            }
        }
    }
}
    