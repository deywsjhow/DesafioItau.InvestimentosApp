using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using DesafioItau.InvestimentosApp.Repository.DbAtivosContext;

namespace DesafioItau.InvestimentosApp.Repository.DbCotacoesContext
{
    public class DbCotacoesContext(IConfiguration configuration) : ICotacoesContext
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        public IDbConnection Connection()
        {
            return new SqlConnection(_connectionString);
        }


        public async Task<RetornoCotacoesBD> GetCotacao(int id)
        {
            var result = new RetornoCotacoesBD();

            try
            {
                using var connectionDB = this.Connection();
                connectionDB.Open();

                string sql = @"SELECT id, id_ativo, preco_unitario, data_hora
                                  FROM [dbo].[Cotacoes]
                                     WITH (NOLOCK, INDEX (Ind_Cotacoes_01))
                                  WHERE id_ativo = @Id
                                  LIMIT 1";

                var cotacao = await connectionDB.QueryFirstOrDefaultAsync<RetornoCotacoesBD>(sql, new {Id = id});

                if (cotacao != null) 
                    result = cotacao;     

            }
            catch (Exception ex) { 
            }

            return result;
        }

    }
}
