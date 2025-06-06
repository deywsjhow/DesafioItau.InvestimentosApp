using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Common.Models.AtivosModels
{
    public class RetornoCotacoesBD
    {
        public Int128 id { get; set; }
        public int id_ativo { get; set; }
        public decimal preco_unitario { get; set; }
        public DateTime data_hora { get; set; }
    }
}
