using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Common.Models.UsuariosModel
{
    public class UsuarioPrecoMedioResponse
    {
        public string NomUser { get; set; } = string.Empty;
        public string NomAtivo { get; set; } = string.Empty;
        public decimal PrecoMedio { get; set; }
    }
}
