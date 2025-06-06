using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Common.Models.UsuariosModel
{
    public class PosicaoTotalResponse
    {
        public string NomUser { get; set; } = string.Empty;
        public decimal TotalPosicao { get; set; }
    }
}
