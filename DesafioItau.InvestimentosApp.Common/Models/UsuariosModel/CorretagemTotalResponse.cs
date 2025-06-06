using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Common.Models.UsuariosModel
{
    public class CorretagemTotalResponse
    {
        public string NomUser { get; set; } = string.Empty;
        public decimal TotalCorretagem { get; set; }
    }
}
