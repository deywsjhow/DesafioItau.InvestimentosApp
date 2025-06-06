using DesafioItau.InvestimentosApp.Common.Models.AtivosModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;

namespace DesafioItau.InvestimentosApp.Repository.DbAtivosContext
{
    public class DbCotacoesContext(IConfiguration configuration) : IAtivosContext
    {
        private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

        public IDbConnection Connection()
        {
            return new SqlConnection(_connectionString);
        }


        public async Task<RetornoAtivosBD> GetAtivo(string codigo)
        {
            var result = new RetornoAtivosBD();

            try
            {
                using var connectionDB = this.Connection();
                connectionDB.Open();

                string sql = @"SELECT id, codigo, nome
                                  FROM [dbo].[Ativos]
                                     WITH (NOLOCK, INDEX (Ind_Ativos_01))
                                  WHERE codigo = @Codigo
                                  LIMIT 1";

                var ativo = await connectionDB.QueryFirstOrDefaultAsync<RetornoAtivosBD>(sql, new {Codigo = codigo});

                if (ativo != null) 
                    result = ativo;     

            }
            catch (Exception ex) { 
            }

            return result;
        }

    }
}
