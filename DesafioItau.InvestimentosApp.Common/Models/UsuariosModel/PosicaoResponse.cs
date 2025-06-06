using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Common.Models.UsuariosModel
{
    public class PosicaoResponse
    {
        public string CodigoAtivo { get; set; } = string.Empty;
        public string NomeAtivo { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public decimal PrecoMedio { get; set; }
        public decimal PL { get; set; }
    }

}
