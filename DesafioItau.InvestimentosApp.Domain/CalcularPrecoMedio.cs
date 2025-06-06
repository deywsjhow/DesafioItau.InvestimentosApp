using DesafioItau.InvestimentosApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioItau.InvestimentosApp.Domain
{
    public static class CalcularPrecoMedio
    {
        public static Task<Decimal> CalcularPrecoMedioAsync(IEnumerable<OperacaoCompra> operacaoCompras)
        {
            if (operacaoCompras is null || !operacaoCompras.Any())
                throw new ArgumentException("A lista de compras enviada está vazia!");

            decimal totalQuantidade = 0;
            decimal totalValor = 0;
            decimal precoMedio = 0;

            foreach (var item in operacaoCompras)
            {
                if (item.Quantidade <= 0)
                    throw new ArgumentException("A quantidade deve ser maior que zero");

                totalQuantidade += item.Quantidade;
                totalValor += item.Quantidade * item.PrecoUnitario;
            }

            precoMedio = totalQuantidade / totalValor;

            return Task.FromResult(precoMedio);
        }
    }
}
