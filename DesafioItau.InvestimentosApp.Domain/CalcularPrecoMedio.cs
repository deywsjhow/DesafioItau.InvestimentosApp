using DesafioItau.InvestimentosApp.Common.Models;

namespace DesafioItau.InvestimentosApp.Domain
{
    public static class CalcularPrecoMedio
    {
        public static decimal Calcular(IEnumerable<OperacaoCompra> operacaoCompras)
        {
            if (operacaoCompras is null)
                throw new ArgumentNullException(nameof(operacaoCompras), "A lista de compras não pode ser nula.");

            if (!operacaoCompras.Any())
                throw new ArgumentException("A lista de compras enviada está vazia!", nameof(operacaoCompras));

            decimal totalQuantidade = 0;
            decimal totalValor = 0;

            foreach (var item in operacaoCompras)
            {
                if (item.Quantidade <= 0)
                    throw new ArgumentException("A quantidade deve ser maior que zero");

                if (item.PrecoUnitario < 0)
                    throw new ArgumentException("Preço unitário da operação não pode ser negativo.");

                totalQuantidade += item.Quantidade;
                totalValor += item.Quantidade * item.PrecoUnitario;
            }

            if (totalQuantidade == 0)
                throw new DivideByZeroException("Não é possível calcular o preço médio com quantidade total igual a zero.");

            return totalValor / totalQuantidade;
        }
    }
}
